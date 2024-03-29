import numpy as np
import sys
from numpy.linalg import norm


def goldensectionsearch(f, interval, tol):
    #PLACE YOUR CODE HERE
    FI = (1 + 5 ** (1/2)) / 2

    a, b = interval
    lam = b - (b - a) / FI
    flam = f(lam)
    mu = a + (b - a) / FI
    fmu = f(mu)

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

    return xmin


# F_HIMMELBLAU is a Himmelblau function
# 	v = F_HIMMELBLAU(X)
#	INPUT ARGUMENTS:
#	X - is 2x1 vector of input variables
#	OUTPUT ARGUMENTS:
#	v is a function value
def fH(X):
    x = X[0]
    y = X[1]
    v = (x ** 2 + y - 11) ** 2 + (x + y ** 2 - 7) ** 2
    return v


# DF_HIMMELBLAU is a Himmelblau function derivative
# 	v = DF_HIMMELBLAU(X)
#	INPUT ARGUMENTS:
#	X - is 2x1 vector of input variables
#	OUTPUT ARGUMENTS:
#	v is a derivative function value
def dfH(X):
    x = X[0]
    y = X[1]
    v = np.copy(X)
    v[0] = 2 * (x ** 2 + y - 11) * (2 * x) + 2 * (x + y ** 2 - 7)
    v[1] = 2 * (x ** 2 + y - 11) + 2 * (x + y ** 2 - 7) * (2 * y)

    return v


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


def sdsearch(f, df, x0, tol):
# SDSEARCH searches for minimum using steepest descent method
# 	answer_ = sdsearch(f, df, x0, tol)
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
    coords = []
    kmax = 1000
    neval = 0

    x = x0.astype("float64")
    coords.append(x.copy())
    deltaX = np.array([np.inf, np.inf])

    k = 0
    while (norm(deltaX) >= tol) and (k < kmax):
        grad = df(x)
        neval += 1

        f1dim = lambda a: f(x - a * grad)
        a = goldensectionsearch(f1dim, [0, 1], tol)
        deltaX = a * grad
        x -= deltaX
        coords.append(x.copy())

        k += 1

    xmin = x
    fmin = f(x)
    neval += 1

    answer_ = [xmin, fmin, neval, coords]
    return answer_


if __name__ == "__main__":
    print(*sdsearch(fH, dfH, np.array([[1.3], [2.0]]), 1e-3), sep=',\n')
