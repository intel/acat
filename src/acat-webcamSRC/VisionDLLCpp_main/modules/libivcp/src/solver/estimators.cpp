/********************************************************************
estimators.c - provides functions for solving linear overdetermined linear systems Ax=b, 
							 using Least-Squares estimator (using IPPM or LAPACK-MKL functions), 
							 and robust M estimators.
*********************************************************************/

#include "libivcp/solver/estimators.h"
#ifdef USE_MKL
#include <mkl.h>
#endif

#define ETHRESHOLD 1e-3 // threshold in the relative update of the parameter vector used to stop the iterations
												// in robustMest()
#define EPSILON 1e-9

#ifndef MSVC
#pragma warning(disable: 4996)
#endif

/*
ippmls - solves least squares problem ||b-Ax||^2 calling IPPM routines.
	width - number of columns in A
	height - number of rows in A
	A - pointer to the A matrix (width x height elements). Compatible with both, ippi and ippm
	    (Astride1 is the actual width in bytes of the rows, equivalent to the Step in ippi).
	b - pointer to the b vector, data ordered sequentially (height elements)
  x - output ls solution, must be alrady allocated with at least of size width

NOTES:
1) For very large matrices lapackls might be more efficient. This function was written as an initial
method to solve least squares without using MKL. However I haven't made a serious study of when LAPACK
is faster than IPPM.
*/
IppStatus ippmls(Ipp32u width, Ipp32u height, Ipp32f *A, Ipp32s Astride1, Ipp32f *b, Ipp32f *x)
{
	Ipp32f *QR, *tmpbuffer;
	Ipp32u QRstride1;

	// creates QR and temporary buffer
	QR = ippsMalloc_32f(width*height);
	tmpbuffer = ippsMalloc_32f(height);
	QRstride1 = width*sizeof(Ipp32f);

	// QR decomposition of A
	IppStatus ippst = ippmQRDecomp_m_32f(A, Astride1, sizeof(Ipp32f), tmpbuffer, QR, QRstride1, sizeof(Ipp32f), width, height);

	if (ippst==ippStsOk)
	{
		// solves least squares using the QR decomposition 
		ippst = ippmQRBackSubst_mv_32f(QR, QRstride1, sizeof(Ipp32f), tmpbuffer, b, sizeof(Ipp32f), x, sizeof(Ipp32f), width, height);
	}

	// free allocated memory for QR and tmpbuffer
	ippsFree(QR);
	ippsFree(tmpbuffer);
	return ippst;
}


/*
lapackls - solves least squares problem ||b-Ax||^2 calling MLK lapack fortran routine sgels
	width - number of columns in A
	height - number of rows in A
	A - pointer to the A matrix (width x height elements). Compatible with both, ippi and ippm
	    (Astride1 is the actual width in bytes of the rows, equivalent to the Step in ippi).
	b - pointer to the b vector, data ordered sequentially (height elements)
		x - output ls solution, must be al least of size width

NOTES:
1) Arrays A and b are overwritten. If data stored in A and b must be conserved, call this function
with copies of them.
2) A is assumed to be stored row-wise. Because fortran routine assumes column-wise ordering, the 
routine is called with a value of "T" for trans (meaning that A is transposed).
3) The fortran routine allows to use a number of rows (m) smaller than the actual dimension of the array.
This feature is used to compensate for the "stride" or "step" (total number of rows is actually
the stride divided by the size of Ipp32f)
*/
#ifdef USE_MKL
int lapackls(Ipp32u width, Ipp32u height, Ipp32f *A, Ipp32s Astride1, Ipp32f *b, Ipp32f *x)
{
	Ipp32f *fwork;
	int m, n, nrhs, lda, ldb, lwork, info;
	char trans[]="T";

	// creates temporary buffer
	lwork = height+width*32;
	fwork = ippsMalloc_32f(lwork);

	// set variables for fortran routine
	m = width; // NOTE: number of colums is width in the transposed matrix
	n = height;// NOTE: number of rows is height in the transposed matrix
	nrhs = 1;
	lda = Astride1/sizeof(Ipp32f); // total dimension in columns for matrix A
	ldb = height;

	// call the fortran routine
	sgels(&trans[0], &m, &n, &nrhs, A, &lda, b, &ldb, fwork, &lwork, &info);

	// copy solution to x
	ippsCopy_32f(b, x, width);

	// free memory
	ippsFree(fwork);

	return info;
}
#endif

/*
robustMest - robust M estimator for the linear problem Ax=b, using Iterative Re-Weighted Least Squares.
	width - number of columns in A
	height - number of rows in A
	A - pointer to the A matrix (width x height elements). Compatible with both, ippi and ippm
	    (Astride1 is the actual width in bytes of the rows, equivalent to the Step in ippi).
	b - pointer to the b vector, data ordered sequentially (height elements)
  x - output ls solution, must be al least of size width
	w - returns the final weights (must be allocated as a sequential array of height number of elements)
	weightFunction - pointer to the robust weight function (see robustFunctions.c)
	C - tuning constant for the weight function (see robustFunctions.c, passing 0 will make the robust function
			to choose the default value).
	MaxIter - maximum number of iterations.

NOTES:
1) Automatically selects ippmls aor lapackls depending on the dimension of the unknown
   (for higher dimensions, say >20, it is more efficient to use lapackls)
2) Added loop exiting condition if the iteration makes the Error to increase
*/
int robustMest(Ipp32u width, Ipp32u height, Ipp32f *A, Ipp32s Astride1, Ipp32f *b, Ipp32f *x, Ipp32f *w,
							 void (*weightFunction)(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C),
							 Ipp32f C, int MaxIter)
{
	Ipp32f alpha, beta;
	Ipp32f *xcurr, *xprev, *Aw, *bw, *r, *aux;
	int variation;
	Ipp32s Awstride1;
	int niter;
	Ipp32f sigma = 1.0;
	IppiSize wSize, roiSize;
	unsigned int k;
	Ipp32f xnorm, incnorm;
	Ipp32f Eprev, Ecurr, numE, denE;
	IppStatus ippst;

	// allocate local, temporary variables
	Aw = ippiMalloc_32f_C1(width, height, &Awstride1);
	bw = ippsMalloc_32f(height);
	r = ippsMalloc_32f(height);
	aux = ippsMalloc_32f(height);
	xcurr = ippsMalloc_32f(width);
	xprev = ippsMalloc_32f(width);
	
	// set variables for fortran routine
	alpha = -1.0;  
	beta = 1.0;

	// initialize iterations
	wSize.width = height;
	wSize.height = 1;
	roiSize.width = 1;
	roiSize.height = height;
	niter = 0;
	variation = 1;
	ippsSet_32f(1.0, w, height); // initialize weights to 1
	ippsSet_32f(0.0, xcurr, width); // initialize xcurr to 0
	Eprev = FLT_MAX;
	Ecurr = Eprev/2;
	while ((variation)&&(niter<MaxIter)&&(Ecurr<Eprev))
	{
		// copy xcurr to xprev
		ippsCopy_32f(xcurr, xprev, width);
		// copy Ecurr to Eant
		Eprev = Ecurr;

		// recompute weights, and weighted matrices
		if (niter>0)
		{
		  // compute new scale factor
		  ippsCopy_32f(r, w, height); // copy r to w, because r will be overwritten
		  sigma = EstimaSigmaRob(r, height, 0.5); // NOTE: r is overwritten
		  // compute weights
		  (*weightFunction)(w, height*sizeof(Ipp32f), wSize, sigma, C);
		  // apply sqrt to w because it will be applied to A and b
		  ippsSqrt_32f_I(w, height);
		}
		
		if (sigma>0)
		{
			// compute weighted matrices
			for(k=0; k<width; k++)
				// multiply A by w column by column 
				ippiMul_32f_C1R(A+k, Astride1, w, sizeof(Ipp32f), Aw+k, Awstride1, roiSize);
			ippsMul_32f(b, w, bw, height);

			// solve weighted least-squares
#ifdef USE_MKL
			if (width>20)
				// use lapack for large system
				ippst = (IppStatus)lapackls(width, height, Aw, Awstride1, bw, xcurr); // LAPACK version
			else
				// use ippm for smaller system
#endif
				ippst = ippmls(width, height, Aw, Awstride1, bw, xcurr); // IPPM version

			if (ippst==ippStsOk)
			{
				// compute new residual, r = b-Ax
				ippsCopy_32f(b, r, height);
				ippsMulC_32f_I(-1.0, xcurr, width);
				ippmMul_mv_32f(A, Astride1, sizeof(Ipp32f), width, height, xcurr, sizeof(Ipp32f), width, r, sizeof(Ipp32f));
				ippsAdd_32f_I(b, r, height);
				ippsMulC_32f_I(-1.0, xcurr, width);

				// compute current total weighted error
				ippsMul_32f(w, r, aux, height); // aux = w.*r
				ippsSqr_32f_I(aux, height);			// aux = aux.^2
				ippsSum_32f(aux, height, &numE, ippAlgHintAccurate);
				ippsSqr_32f(w, aux, height);
				ippsSum_32f(aux, height, &denE, ippAlgHintAccurate);
				Ecurr = numE/denE;

				// check for big variation in x (||xcurr-xprev||/||xcurr||>ETHRESHOLD
				ippsNormDiff_L2_32f(xcurr, xprev, width, &incnorm);
				ippsNorm_L2_32f(xcurr, width, &xnorm);
				variation = ((incnorm/(xnorm+EPSILON)>ETHRESHOLD) && (Ecurr>EPSILON));
				niter++;
			}
			else
			{
				variation = 0;
			}
		}
		else
		{
			variation = 0;
		}
	}

	if (Eprev<Ecurr)
		// copy xprev to x
		ippsCopy_32f(xprev, x, width);
	else
		// copy xcurr to x
		ippsCopy_32f(xcurr, x, width);

	// free local temporary variables
	ippiFree(Aw);
	ippsFree(bw);
	ippsFree(r);
	ippsFree(aux);
	ippsFree(xcurr);
	ippsFree(xprev);

	return niter;
}