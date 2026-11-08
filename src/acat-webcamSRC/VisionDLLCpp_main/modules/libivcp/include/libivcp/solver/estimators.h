
#ifndef ESTIMATORS_H
#define ESTIMATORS_H

#include "libivcp/solver/robustFunctions.h"

IppStatus ippmls(Ipp32u width, Ipp32u height, Ipp32f *A, Ipp32s Astride1, Ipp32f *b, Ipp32f *x);
int lapackls(Ipp32u width, Ipp32u height, Ipp32f *A, Ipp32s Astride1, Ipp32f *b, Ipp32f *x);
void dlapackls(Ipp32u width, Ipp32u height, Ipp64f *A, Ipp32s Astride1, Ipp64f *b, Ipp64f *x);

int robustMest(Ipp32u width, Ipp32u height, Ipp32f *A, Ipp32s Astride1, Ipp32f *b, Ipp32f *x, Ipp32f *w,
    void (*weightFunction)(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C),
    Ipp32f C, int MaxIter);

#endif