import numpy as np
import sys
from numpy.linalg import norm
np.seterr(all='warn')


# F_HIMMELBLAU is a Himmelblau function
# 	v = F_HIMMELBLAU(X)
#	INPUT ARGUMENTS:
#	X - is 2x1 vector of input variables
#	OUTPUT ARGUMENTS:
#	v is a function value
def fH(X):
    x = X[0]
    y = X[1]
    v = (x**2 + y - 11)**2 + (x + y**2 - 7)**2
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
    v[0] = 2 * (x**2 + y - 11) * (2 * x) + 2 * (x + y**2 - 7)
    v[1] = 2 * (x**2 + y - 11) + 2 * (x + y**2 - 7) * (2 * y)

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
    v = (1 - x)**2 + 100*(y - x**2)**2
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
    v[0] = -2 * (1 - x) + 200 * (y - x**2)*(- 2 * x)
    v[1] = 200 * (y - x**2)
    return v



def H(X, tol, df):
    #PLACE YOUR CODE HERE
    x, y = X
    df_x_plus_tol = df(np.array([x + tol, y]))
    df_x_minus_tol = df(np.array([x - tol, y]))
    df_y_plus_tol = df(np.array([x, y + tol]))
    df_y_minus_tol = df(np.array([x, y - tol]))

    ddfxx = ((df_x_plus_tol[0] - df_x_minus_tol[0]) / (2 * tol))[0]
    ddfxy = ((df_x_plus_tol[1] - df_x_minus_tol[1]) / (2 * tol))[0]
    ddfyx = ddfxy
    ddfyy = ((df_y_plus_tol[1] - df_y_minus_tol[1]) / (2 * tol))[0]

    ddf = np.matrix([
        [ddfxx, ddfxy],
        [ddfyx, ddfyy]
    ])

    return ddf


def nsearch(f, df, x0, tol):
# NSEARCH searches for minimum using Newton method
# 	answer_ = nsearch(f, df, x0, tol)
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

    x = x0
    coords.append(x)
    deltaX = np.array([np.inf, np.inf])

    k = 0
    while (norm(deltaX) >= tol) and (k < kmax):
        g = df(x)
        H0 = H(x, 0.1 * tol, df)

        deltaX = np.linalg.lstsq(-H0, g)[0]
        x = x + deltaX
        coords.append(x)

        neval += 5
        k += 1

    xmin = x
    fmin = f(x)

    answer_ = [xmin, fmin, neval,  coords]
    return answer_


if __name__ == "__main__":
    x0 = np.array([[-1.0], [-1.0]])
    tol = 1e-9
    [xmin, f, neval, coords] = nsearch(fR, dfR, x0, tol)
    print(xmin, f, neval)
