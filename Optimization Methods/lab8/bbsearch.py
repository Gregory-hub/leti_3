import numpy as np
import sys
from numpy.linalg import norm


def goldensectionsearch(f, interval, tol):
    FI = (1 + 5 ** (1/2)) / 2

    a, b = interval
    lam = b - (b - a) / FI
    flam = f(lam)
    mu = a + (b - a) / FI
    fmu = f(mu)

    neval = 2

    while np.abs(b - a) > tol:
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

    return xmin, neval


# F_ROSENBROCK is a Rosenbrock function
# 	v = F_ROSENBROCK(X)
#	INPUT ARGUMENTS:
#	X - is 2x1 vector of input variables
#	OUTPUT ARGUMENTS:
#	v is a function value
def fR(X):
    x = X[0]
    y = X[1]
    v = (1 - x) ** 2 + 100 * (y - x ** 2) ** 2
    return v


# DF_ROSENBROCK is a Rosenbrock function derivative
# 	v = DF_ROSENBROCK(X)
#	INPUT ARGUMENTS:
#	X - is 2x1 vector of input variables
#	OUTPUT ARGUMENTS:
#	v is a derivative function value
def dfR(X):
    x = X[0]
    y = X[1]
    v = np.copy(X)
    v[0] = -2 * (1 - x) + 200 * (y - x ** 2) * (- 2 * x)
    v[1] = 200 * (y - x ** 2)
    return v


def bbsearch(f, df, x0, tol):
# BBSEARCH searches for minimum using stabilized BB1 method
# 	answer_ = bbsearch(f, df, x0, tol)
#   INPUT ARGUMENTS
#   f  - objective function
#   df - gradient
# 	x0 - start point
# 	tol - set for bot range and function value
#   OUTPUT ARGUMENTS
#   answer_ = [xmin, fmin, neval, coords]
# 	xmin is a function minimizer
# 	fmin = f(xmin)
# 	neval - number of function evaluations
#   coords - array of statistics

    #PLACE YOUR CODE HERE
    d = 0.1
    kmax = 1000
    coords = []

    x = x0.astype("float64")
    coords.append(x.copy())

    g = df(x)
    df1dim = lambda a: norm(-df(x - a * g))
    a, neval = goldensectionsearch(df1dim, [0, 1], tol)
    deltaX = np.array([np.inf, np.inf])

    neval += 1

    k = 0
    while (norm(deltaX) >= tol) and (k < kmax):
        a_stab = d / norm(g)
        a = min(a, a_stab)

        deltaX = a * g
        x -= deltaX
        coords.append(x.copy())

        g_prev = g.copy()
        g = df(x)
        deltaG = g - g_prev

        a = abs(np.dot(deltaX.T, deltaX)[0][0])
        a /= abs(np.dot(deltaX.T, deltaG)[0][0])

        k += 1
        neval += 1

    xmin = x
    fmin = f(x)

    answer_ = [xmin, fmin, neval, coords]
    return answer_


if __name__ == "__main__":
    x0 = np.array([[2], [-1]])
    tol = 1e-9
    [xmin, f, neval, coords] = bbsearch(fR, dfR, x0, tol)
    # print(xmin, f, neval, *coords[:100], sep='\n')
    print(xmin, f, neval, sep='\n')
