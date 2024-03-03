import numpy as np
def f(x): return (x - 3)**2- 3*x + x**2 - 40

def gsearch(interval, tol):
# GOLDENSECTIONSEARCH searches for minimum using golden section
# 	[xmin, fmin, neval] = GOLDENSECTIONSEARCH(f,interval,tol)
#   INPUT ARGUMENTS
# 	f is a function
# 	interval = [a, b] - search interval
# 	tol - set for bot range and function value
#   OUTPUT ARGUMENTS
# 	xmin is a function minimizer
# 	fmin = f(xmin)
# 	neval - number of function evaluations
#   coords - array of statistics,  coord[i][:] =  [x1,x2, a, b]

    #PLACE YOUR CODE HERE
    neval = 0
    coord = []

    FI = (1 + 5 ** (1/2)) / 2

    a, b = interval

    lam = b - (b - a) / FI
    flam = f(lam)
    mu = a + (b - a) / FI
    fmu = f(mu)

    while np.abs(b - a) > tol:
        coord.append([lam, mu, a, b])
        if flam > fmu:
            a = lam
            xmin = mu
            lam = mu
            flam = fmu
            mu = a + (b - a) / FI
            fmu = f(mu)
        else:
            b = mu
            xmin = lam
            mu = lam
            fmu = flam
            lam = b - (b - a) / FI
            flam = f(lam)

        neval += 1

    fmin = f(xmin)

    answer_ = [xmin, fmin, neval, coord]
    return answer_
