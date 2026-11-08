
#ifndef ROBUST_FUNCTIONS_H
#define ROBUST_FUNCTIONS_H

#include <ipp.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <xmmintrin.h>
#include <float.h>

void InitRobustFunctions(IppiSize size);
void ReleaseRobustFunctions();
void (*getWFunction(const char *funcstring))(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void (*getDPsiFunction(const char *funcstring))(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void (*getPsiFunction(const char *funcstring))(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void (*getRhoFunction(const char *funcstring))(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);

Ipp32f EstimaSigma(Ipp32f *buffer, int Nelements);
Ipp32f EstimaSigmaRob(Ipp32f *buffer, int Nelements, float P);
Ipp32f median(Ipp32f *buffer, int Nelements);
Ipp32f Percentile(Ipp32f *buffer, int Nelements, float P);

void linearDPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void L1DPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void huberDPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void tukeyDPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void cauchyDPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void linearPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void L1Psi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void huberPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void tukeyPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void cauchyPsi(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);

void linearRho(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void L1Rho(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void huberRho(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void tukeyRho(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void cauchyRho(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void linearW(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void L1W(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void huberW(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void tukeyW(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);
void cauchyW(Ipp32f *input, int inputStep, IppiSize inputSize, Ipp32f sigma, Ipp32f C);

#endif