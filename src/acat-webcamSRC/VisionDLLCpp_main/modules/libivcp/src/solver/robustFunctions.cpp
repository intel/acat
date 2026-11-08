/********************************************************************
robustFunctions.c - provides robust functions to be applied element by 
										by element to Intel IPP images (type Ipp32f).
										All functions are applied "in-place" (the input image
										is also the output image).
										Provides the robust function, its first and second derivatives, 
										and the weight function (first derivative divided by e).
										Also provides functions to estimate the median, and a robust version
										of the scale parameter of the data.

NOTES: 
1) All the functions have a similar interface:
func(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)

	input - input image
	inputStep - step of the input image
	inputSize - size of the input image
	sigma - scale parameter
	C - tuning constant of the specific robust function (some of them might not use it)
			if set to 0, the function must choose a default value.

2) There are also functions that return pointers to the appropriate function based on the value of
	 a string (the generic name of the function). This provides flexibility to external modules, permiting
	 to choose quickly different functions based on the values of a string.

3) Adding new functions: new robust functions can be added. The functions returning pointers to the functions
   must be updated to include the new robust function.
*********************************************************************/

////////////
// INCLUDES
////////////

#include "libivcp/solver/robustFunctions.h"

#ifndef MSVC
extern "C" { __m128 vmlsLn4(__m128 a); }
#pragma warning(disable: 4996)
#endif

////////////
// DEFINES
////////////
#define HUBERC95 1.345 // default constant multiplying the scale (sigma) in huber function (1.345 for 95% efficiency in Gaussian data)
#define TUKEYC95 4.685 // default constant multiplying the scale (sigma) in tukey function (4.685 for 95% efficiency in Gaussian data)
#define CAUCHYC95 2.385 // default constant multiplying the scale (sigma) in cauchy function (4.685 for 95% efficiency in Gaussian data)

////////////////////////////
// GLOBAL MODULE VARIABLES
////////////////////////////
/* This variables are used as temporary buffers of the same size than the images to hold intermediate computations.
They are initialized by initMask function, which takes as argument the MAXIMUM size of the images used. They are actually
vectors holding as many temporary images as the maximum number of threads, such that the functions use the temporary
variable indexed by its thread number.
NOTE: initMask MUST BE CALLED BEFORE USING ANY FUNCTION IN THIS MODULE.
*/
Ipp8u **mask; // auxiliar mask for computations 
Ipp32f **aux; // auxiliar image (same size as mask)
int maskStep, auxStep;
int MaskInitialized = 0; // flag to indicate if the global module variables have been initialized


////////////
// FUNCTIONS
////////////


/*
EstimaSigmaRob - estimates (robustly) the scale factor sigma using the MAD (MAD = 1.4826 * (median(abs(buffer-median(buffer)))

	buffer - buffer containing the elements in sequential order
	Nelements - number of elements

NOTES:
1) buffer is overwritten. If the original data is needed, call
this function with a copy of the data.
*/
Ipp32f EstimaSigmaRob(Ipp32f *buffer, int Nelements, float percentile)
{
  Ipp32f med;

	// compute the median of the buffer elements
	med = median(buffer, Nelements);
	// substract the median to the buffer elements
	ippsSubC_32f_I(med, buffer, Nelements);
	// take the absolute value
	ippsAbs_32f_I(buffer, Nelements);

	return (Ipp32f)(1.4826*0.5/percentile*Percentile(buffer, Nelements,percentile));
}

/*
median - median value of the elements of a buffer

	buffer - buffer containing the elements in sequential order
	Nelements - number of elements

NOTES:
1) Buffer is overwritten by itself, ordered in ascending order.
	 If the original data is needed, call this function with a copy of the data.
*/
Ipp32f median(Ipp32f *buffer, int Nelements)
{
	int mid1;

	// sort the buffer using IPPS function
	ippsSortAscend_32f_I(buffer, Nelements);

	// the median is the element in the middle of the buffer
	mid1 = (int)floor(Nelements/2.0); 

    return buffer[mid1];
}

Ipp32f Percentile(Ipp32f *buffer, int Nelements, float P)
{
	// sort the buffer using IPPS function
	ippsSortAscend_32f_I(buffer, Nelements);

	return buffer[(int)floor(Nelements*P)];
}


/*
getPsiFunction - returns pointer to the appropriate psi function, based on the input string

	funcstring - string with the name of the robust function (e.g., "tukey", "huber", "linear", etc.)
*/
void (*getPsiFunction(const char *funcstring))(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{

	if (strcmp(funcstring, "linear")==0)
		return &linearPsi;

	if (strcmp(funcstring, "huber")==0)
		return &huberPsi;

	if (strcmp(funcstring, "tukey")==0)
		return &tukeyPsi;

	if (strcmp(funcstring, "cauchy")==0)
		return &cauchyPsi;

	if (strcmp(funcstring, "L1")==0)
		return &L1Psi;

  fprintf(stderr, "getPsiFunction WARNING: unknown function, returning linearPsi\n");
	return &linearPsi;

}

/*
getRhoFunction - returns pointer to the appropriate Rho function, based on the input string

	funcstring - string with the name of the robust function (e.g., "tukey", "huber", "linear", etc.)
*/
void (*getRhoFunction(const char *funcstring))(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{

	if (strcmp(funcstring, "linear")==0)
		return &linearRho;

	if (strcmp(funcstring, "huber")==0)
		return &huberRho;

	if (strcmp(funcstring, "tukey")==0)
		return &tukeyRho;

	if (strcmp(funcstring, "cauchy")==0)
		return &cauchyRho;

	if (strcmp(funcstring, "L1")==0)
		return &L1Rho;


  fprintf(stderr, "getRhoFunction WARNING: unknown function, returning linearRho\n");
	return &linearRho;

}

/*
getDPsiFunction - returns pointer to the appropriate DPsi (derivative of psi) function,	based on the input string

	funcstring - string with the name of the robust function (e.g., "tukey", "huber", "linear", etc.)
*/
void (*getDPsiFunction(const char *funcstring))(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{

	if (strcmp(funcstring, "linear")==0)
		return &linearDPsi;

	if (strcmp(funcstring, "huber")==0)
		return &huberDPsi;

	if (strcmp(funcstring, "tukey")==0)
		return &tukeyDPsi;

	if (strcmp(funcstring, "cauchy")==0)
		return &cauchyDPsi;

	if (strcmp(funcstring, "L1")==0)
		return &L1DPsi;


  fprintf(stderr, "getRhoFunction WARNING: unknown function, returning linearDPsi\n");
	return &linearDPsi;

}


/*
getWFunction - returns pointer to the appropriate W function (Psi/e), based on the input string

	funcstring - string with the name of the robust function (e.g., "tukey", "huber", "linear", etc.)
*/
void (*getWFunction(const char *funcstring))(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{

	if (strcmp(funcstring, "linear")==0)
		return &linearW;

	if (strcmp(funcstring, "huber")==0)
		return &huberW;

	if (strcmp(funcstring, "tukey")==0)
		return &tukeyW;

	if (strcmp(funcstring, "cauchy")==0)
		return &cauchyW;

	if (strcmp(funcstring, "L1")==0)
		return &L1W;


  fprintf(stderr, "getWFunction WARNING: unknown function, returning linearW\n");
	return &linearW;

}

//////////////
// linearRho = (e)^2/(2*sigma^2) (non-robust, here for completeness)
//////////////
void linearRho(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
    (void)C;
	// e^2/(2*sigma^2)
	ippiSqr_32f_C1IR(input, inputStep, inputSize);
	ippiDivC_32f_C1IR((float)2.0*sigma*sigma, input, inputStep, inputSize);
}

//////////////
// linearPsi = e/sigma^2
//////////////
void linearPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
    (void)C;
	// e/sigma^2
    ippiDivC_32f_C1IR(sigma*sigma, input, inputStep, inputSize);
}

//////////////
// linearW = 1/sigma^2
//////////////
void linearW(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
    (void)C;
	// 1/sigma^2
    ippiSet_32f_C1R(1/(sigma*sigma), input, inputStep, inputSize);
}

//////////////
// linearDPsi = 1/sigma^2
//////////////
void linearDPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
    (void)C;
	// 1/sigma^2
    ippiSet_32f_C1R(1/(sigma*sigma), input, inputStep, inputSize);
}

//////////////
// huberRho
//////////////
void huberRho(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
	Ipp32f k;
	int n = 0;
#ifdef _OPENMP
	n = omp_get_thread_num();
#endif

	if (C==0)
		k = (Ipp32f)HUBERC95*sigma;
	else
		k = C*sigma;


	// take absolute value of the input (rho is symmetric)
    ippiAbs_32f_C1IR(input, inputStep, inputSize);

	// define mask
	ippiCompareC_32f_C1R(input, inputStep, k, mask[n], maskStep, inputSize, ippCmpLess);

	// apply x^2/2 to masked region, store in aux
	ippiSet_32f_C1R(0.0, aux[n], auxStep, inputSize);
	ippiAddSquare_32f_C1IMR(input, inputStep, mask[n], maskStep, aux[n], auxStep, inputSize);
	ippiDivC_32f_C1IR(2.0, aux[n], auxStep, inputSize);

	// apply k*abs(x)-1/2k^2
	ippiMulC_32f_C1IR(k, input, inputStep, inputSize);
	ippiSubC_32f_C1IR(k*k/2, input, inputStep, inputSize);

	// copy masked region of aux to output
	ippiCopy_32f_C1MR(aux[n], auxStep, input, inputStep, inputSize, mask[n], maskStep);

	// divide by sigma^2
    ippiDivC_32f_C1IR(sigma*sigma, input, inputStep, inputSize);


}

//////////////
// huberPsi
//////////////
void huberPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
	Ipp32f k;

	if (C==0)
		k = (Ipp32f)HUBERC95*sigma;
	else
		k = C*sigma;

	// set x to k for x>k
	ippiThreshold_GTVal_32f_C1IR(input, inputStep, inputSize, k, k);
	
	// set x to -k for x<-k
	ippiThreshold_LTVal_32f_C1IR(input, inputStep, inputSize, -k, -k);

	// divide by sigma^2
  ippiDivC_32f_C1IR(sigma*sigma, input, inputStep, inputSize);

}

//////////////
// huberW
//////////////
void huberW(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
	Ipp32f k;
	int x, y;
	
	Ipp32f currval, kdivs2, onedivs2;

	if (C==0)
		k = (Ipp32f)HUBERC95*sigma;
	else
		k = C*sigma;

	onedivs2 = 1/(sigma*sigma);
	kdivs2 = onedivs2*k;

	
	for(y=0; y<inputSize.height; y++)
	{
		Ipp32f *tmp = (Ipp32f *)((Ipp8u *)input + y*inputStep);
		for(x=0; x<inputSize.width; x++)
		{
			currval = fabs(*tmp);
			if (currval>k)
				*tmp = kdivs2/currval;
			else
				*tmp = onedivs2;
			tmp++;
		}
	}
}

//////////////
// huberDPsi
//////////////
void huberDPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
	Ipp32f k;

	if (C==0)
		k = (Ipp32f)HUBERC95*sigma;
	else
		k = C*sigma;

	ippiAbs_32f_C1IR(input, inputStep, inputSize);

	// set x to 0 for |x|>k, and to 1/sigma^1 for |x|<k
	ippiThreshold_LTValGTVal_32f_C1IR(input, inputStep, inputSize, k, 1/(sigma*sigma), k, 0.0);

}

//////////////
// L1Rho
//////////////
void L1Rho(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
    (void)C;
	// |e|/(2*sigma^2)
    ippiAbs_32f_C1IR(input, inputStep, inputSize);
	ippiDivC_32f_C1IR((float)2.0*sigma*sigma, input, inputStep, inputSize);

}

//////////////
// L1Psi
//////////////
void L1Psi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
    (void)C;
	// sign(x)
	ippiThreshold_LTValGTVal_32f_C1IR(input, inputStep, inputSize, 0.0, (Ipp32f)(-1.0/(2*sigma*sigma)), 0.0, (Ipp32f)(1.0/(2*sigma*sigma)));

}

//////////////
// L1W
//////////////
void L1W(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
    (void)C;
	int n = 0;
#ifdef _OPENMP
	n = omp_get_thread_num();
#endif

	// sign(x)
	ippiThreshold_LTValGTVal_32f_C1R(input, inputStep, aux[n], auxStep, inputSize, 0.0, (Ipp32f)(-1.0/(2*sigma*sigma)), 0.0, (Ipp32f)(1.0/(2*sigma*sigma)));

	// divide by x
	ippiDiv_32f_C1IR(input, inputStep, aux[n], auxStep, inputSize);

	// copy result to input
	ippiCopy_32f_C1R(aux[n], auxStep, input, inputStep, inputSize);


}

//////////////
// L1DPsi
//////////////
void L1DPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
    (void)sigma;
    (void)C;
	// 0.0
	ippiSet_32f_C1R(0.0, input, inputStep, inputSize);

}

//////////////
// tukeyRho
//////////////
void tukeyRho(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
	Ipp32f k;
	int TauxStep;
	Ipp32f *Taux = ippiMalloc_32f_C1(inputSize.width, inputSize.height, &TauxStep);

	if (C==0)
		k = (Ipp32f)TUKEYC95*sigma;
	else
		k = C*sigma;

	
	// set x to k for x>k
	ippiThreshold_GTVal_32f_C1IR(input, inputStep, inputSize, k, k);
	
	// set x to -k for x<-k
	ippiThreshold_LTVal_32f_C1IR(input, inputStep, inputSize, -k, -k);

	// compute  (k^2/6)*(1-(1-(x/k)^2)^3) (note: values set to k will give an output of k^2/6)
	
	// input = input/k
	ippiDivC_32f_C1IR(k, input, inputStep, inputSize);
	// input = input^2 ( (x/k)^2 )
	ippiSqr_32f_C1IR(input, inputStep, inputSize);
	// aux = input-1 ( (x/k)^2-1 )
	ippiSubC_32f_C1R(input, inputStep, 1.0, Taux, TauxStep, inputSize);
  // input = aux^2
	ippiSqr_32f_C1R(Taux, TauxStep, input, inputStep, inputSize);
	// input = input*aux (final result: input = ((x/k)^2-1)^3
	ippiMul_32f_C1IR(Taux, TauxStep, input, inputStep, inputSize);
	// input = 1 + ((x/k)^2-1)^3 (equivalent to 1-(1-(x/k)^2)^3 )
	ippiAddC_32f_C1IR(1.0, input, inputStep, inputSize);
	// apply scaling constant k^2/6
	ippiMulC_32f_C1IR((float)(k*k/6.0), input, inputStep, inputSize);

	ippiFree(Taux);
}

//////////////
// tukeyPsi
//////////////
void tukeyPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
	Ipp32f k;
	int n = 0;
#ifdef _OPENMP
	n = omp_get_thread_num();
#endif

	if (C==0)
		k = (Ipp32f)TUKEYC95*sigma;
	else
		k = C*sigma;

	// set x to 0 for x>k
	ippiThreshold_GTVal_32f_C1IR(input, inputStep, inputSize, k, 0.0);
	
	// set x to 0 for x<-k
	ippiThreshold_LTVal_32f_C1IR(input, inputStep, inputSize, -k, 0.0);

	// compute x*(1-(x/k)^2)^2 (note: values set to 0 will remain 0
	ippiDivC_32f_C1R(input, inputStep, k, aux[n], auxStep, inputSize);
	ippiSqr_32f_C1IR(aux[n], auxStep, inputSize);
	ippiSubC_32f_C1IR(1.0, aux[n], auxStep, inputSize);
	ippiSqr_32f_C1IR(aux[n], auxStep, inputSize);
	ippiMul_32f_C1IR(aux[n], auxStep, input, inputStep, inputSize);

	// divide by sigma^2
	ippiDivC_32f_C1IR(sigma*sigma, input, inputStep, inputSize);

}

void tukeyW(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
	Ipp32f k;
	if (C==0)
		k = (Ipp32f)TUKEYC95*sigma;
	else
		k = C*sigma;
	__m128 MMk = _mm_set_ps1(1/k);
	__m128 MMK = _mm_set_ps1((float)(1.0/(sigma*sigma)));
	__m128 MMONE = _mm_set_ps1(1.0f);
	__m128 MMmONE = _mm_set_ps1(-1.0f);
	for(int y=0; y<inputSize.height; y++)
	{
		Ipp32f *tmp = (Ipp32f *)((Ipp8u *)input + y*inputStep);
		for(int x=0; x<inputSize.width; x+=4)
		{
			__m128 data = _mm_load_ps(tmp);
			data = _mm_mul_ps(data, MMk);
			__m128 mask1 = _mm_cmpge_ps(data, MMmONE); // x>-1
			__m128 mask2 = _mm_cmplt_ps(data, MMONE); // x<1
			mask2 = _mm_and_ps(mask1, mask2);  
			data = _mm_mul_ps(data, data);
			data = _mm_add_ps(data, MMmONE);
			data = _mm_mul_ps(data, data);
			data = _mm_mul_ps(data, MMK);
			data = _mm_and_ps(data, mask2);
			_mm_store_ps(tmp, data);
			tmp += 4;
		}
	}

}

//////////////
// tukeyDPsi
//////////////
void tukeyDPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
	Ipp32f k;
	int n = 0;
#ifdef _OPENMP
	n = omp_get_thread_num();
#endif

	if (C==0)
		k = (Ipp32f)TUKEYC95*sigma;
	else
		k = C*sigma;

	// take absolute value of the input (DPsi is symmetric)
	ippiAbs_32f_C1IR(input, inputStep, inputSize);

	// define mask
	ippiCompareC_32f_C1R(input, inputStep, k, mask[n], maskStep, inputSize, ippCmpLess);

	// aux = (x/k)^2, only in masked region (0 outside)
	ippiSet_32f_C1R(0.0, aux[n], auxStep, inputSize);
	ippiDivC_32f_C1IR(k, input, inputStep, inputSize);
	ippiAddSquare_32f_C1IMR(input, inputStep, mask[n], maskStep, aux[n], auxStep, inputSize);

	// compute (1-aux)*(1-5*aux)
	ippiMulC_32f_C1IR(-1.0, aux[n], auxStep, inputSize); 
	ippiMulC_32f_C1R(aux[n], auxStep, 5.0, input, inputStep, inputSize); // input = -5*aux
	ippiAddC_32f_C1IR(1.0, input, inputStep, inputSize); // input = 1-5*aux
	ippiAddC_32f_C1IR(1.0, aux[n], auxStep, inputSize); // aux = 1-aux
	ippiMul_32f_C1IR(input, inputStep, aux[n], auxStep, inputSize); //aux = (1-aux)*(1-5*aux)

	// copy maske portion to input
	ippiSet_32f_C1R(0.0, input, inputStep, inputSize);
	ippiCopy_32f_C1MR(aux[n], auxStep, input, inputStep, inputSize, mask[n], maskStep);

	// divide by sigma^2
	ippiDivC_32f_C1IR(sigma*sigma, input, inputStep, inputSize);


}

void cauchyRho(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
	Ipp32f k;
	if (C==0)
		k = (Ipp32f)CAUCHYC95*sigma;
	else
		k = C*sigma;
	Ipp32f k2 = k*k;
	Ipp32f K = (Ipp32f)(k2/(2.0*sigma*sigma));
  __m128 MMK, MMONE, MMk;
	MMk = _mm_set_ps1(1/k);
	MMK = _mm_set_ps1(K);
	MMONE = _mm_set_ps1(1.0f);
	for(int y=0; y<inputSize.height; y++)
	{
		Ipp32f *tmp = (Ipp32f *)((Ipp8u *)input + y*inputStep);
		for(int x=0; x<inputSize.width; x+=4)
		{
			__m128 MM0 = _mm_load_ps(tmp);
			MM0 = _mm_mul_ps(MM0, MMk);
			MM0 = _mm_mul_ps(MM0, MM0);
			MM0 = _mm_add_ps(MM0, MMONE);
#ifndef MSVC
			float p[4];
			_mm_storeu_ps(p, MM0);
			for(int k=0; k<4; k++)
				p[k] = log(p[k]);
			MM0 = _mm_loadu_ps(p);
#else
			MM0 = vmlsLn4(MM0);
#endif
			MM0 = _mm_mul_ps(MM0, MMK);
			_mm_store_ps(tmp, MM0);
			tmp += 4;
		}
	}
}
//////////////
// cauchyPsi
//////////////
void cauchyPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
	Ipp32f k;
	if (C==0)
		k = (Ipp32f)CAUCHYC95*sigma;
	else
		k = C*sigma;
	Ipp32f K = (Ipp32f)(k/(sigma*sigma));
	__m128 MMK, MMONE, MMk;
	MMk = _mm_set_ps1(1/k);
	MMK = _mm_set_ps1(K);
	MMONE = _mm_set_ps1(1.0f);
	for(int y=0; y<inputSize.height; y++)
	{
		Ipp32f *tmp = (Ipp32f *)((Ipp8u *)input + y*inputStep);
		for(int x=0; x<inputSize.width; x+=4)
		{
			__m128 MM0 = _mm_load_ps(tmp);
		    MM0 = _mm_mul_ps(MM0, MMk);
			__m128 MM1 = _mm_mul_ps(MM0, MMK);
			MM0 = _mm_mul_ps(MM0, MM0);
			MM0 = _mm_add_ps(MM0, MMONE);
			MM0 = _mm_rcp_ps(MM0);
			MM0 = _mm_mul_ps(MM1, MM0);
			_mm_store_ps(tmp, MM0);
			tmp += 4;
		}
	}
}


//////////////
// cauchyDPsi
//////////////
void cauchyDPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
	Ipp32f k;
	if (C==0)
		k = (Ipp32f)CAUCHYC95*sigma;
	else
		k = C*sigma;
	Ipp32f K = (Ipp32f)(1.0/(sigma*sigma));
	__m128 MMK, MMONE, MMk;
	MMk = _mm_set_ps1(1/k);
	MMK = _mm_set_ps1(K);
	MMONE = _mm_set_ps1(1.0f);
	for(int y=0; y<inputSize.height; y++)
	{
		Ipp32f *tmp = (Ipp32f *)((Ipp8u *)input + y*inputStep);
		for(int x=0; x<inputSize.width; x+=4)
		{
			__m128 MM0 = _mm_load_ps(tmp);
			MM0 = _mm_mul_ps(MM0, MMk);
			MM0 = _mm_mul_ps(MM0, MM0);
			__m128 MM1 = _mm_sub_ps(MMONE, MM0);
			MM0 = _mm_add_ps(MMONE, MM0);
			MM0 = _mm_mul_ps(MM0, MM0);
			MM0 = _mm_rcp_ps(MM0);
			MM0 = _mm_mul_ps(MM1, MM0);
			MM0 = _mm_mul_ps(MM0, MMK);
			_mm_store_ps(tmp, MM0);
			tmp += 4;
		}
	}
}


//////////////
// cauchyW
//////////////
void cauchyW(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C)
{
	Ipp32f k;
	int n = 0;
#ifdef _OPENMP
	n = omp_get_thread_num();
#endif

	if (C==0)
		k = (Ipp32f)CAUCHYC95*sigma;
	else
		k = C*sigma;

	ippiDivC_32f_C1IR(k, input, inputStep, inputSize);

	ippiSqr_32f_C1R(input, inputStep, aux[n], auxStep, inputSize);
	ippiAddC_32f_C1IR(1.0, aux[n], auxStep, inputSize);

	ippiCopy_32f_C1R(input, inputStep, aux[n], auxStep, inputSize);
	ippiSet_32f_C1R(1.0, input, inputStep, inputSize);

	ippiDiv_32f_C1IR(aux[n], auxStep, input, inputStep, inputSize);

	ippiMulC_32f_C1IR((Ipp32f)(1.0/(sigma*sigma)), input, inputStep, inputSize);


}