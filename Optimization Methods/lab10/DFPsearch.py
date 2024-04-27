import numpy as np
import sys
from numpy.linalg import norm
#np.seterr(divide='ignore', invalid='ignore')


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


def zoom(phi, dphi, alo, ahi, c1, c2):
    j = 1
    jmax = 1000
    while j < jmax:
        a = cinterp(phi, dphi, alo, ahi)
        if phi(a) > phi(0) + c1 * a * dphi(0) or phi(a) >= phi(alo):
            ahi = a
        else:
            if abs(dphi(a)) <= -c2 * dphi(0):
                return a  # a is found
            if dphi(a) * (ahi - alo) >= 0:
                ahi = alo
            alo = a
        j += 1
    return a


def cinterp(phi, dphi, a0, a1):
    if np.isnan(dphi(a0) + dphi(a1) - 3 * (phi(a0) - phi(a1))) or (a0 - a1) == 0:
        a = a0
        return a
 
    d1 = dphi(a0) + dphi(a1) - 3 * (phi(a0) - phi(a1)) / (a0 - a1)
    if np.isnan(np.sign(a1 - a0) * np.sqrt(d1 ** 2 - dphi(a0) * dphi(a1))):
        a = a0
        return a
    d2 = np.sign(a1 - a0) * np.sqrt(d1 ** 2 - dphi(a0) * dphi(a1))
    a = a1 - (a1 - a0) * (dphi(a1) + d2 - d1) / (dphi(a1) - dphi(a0) + 2 * d2)

    return a


def wolfesearch(f, df, x0, p0, amax, c1, c2):
    a = amax
    aprev = 0
    phi = lambda x: f(x0 + x * p0)
    dphi = lambda x: np.dot(p0.transpose(), df(x0 + x * p0))

    phi0 = phi(0)
    dphi0 = dphi(0)
    i = 1
    imax = 1000
    while i < imax:
        if (phi(a) > phi0 + c1 * a * phi0) or ((phi(a) >= phi(aprev)) and (i > 1)):
            a = zoom(phi, dphi, aprev, a, c1, c2)
            return a

        if abs(dphi(a)) <= -c2 * dphi0:
            return a  # a is found already

        if dphi(a) >= 0:
            a = zoom(phi, dphi, a, aprev, c1, c2)
            return a

        a = cinterp(phi, dphi, a, amax)
        i += 1

    return a


def dfpsearch(f, df, x0, tol):
    # DFPSEARCH searches for minimum using DFP method
# 	answer_ = dfpsearch(f, df, x0, tol)
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
    kmax = 1000
    coords = []
    neval = 1

    x = np.array([x0[0][0], x0[1][0]], dtype="float64")
    coords.append(x)

    H = np.matrix([[1, 0], [0, 1]], dtype="float64")
    g = np.matrix(df(x), dtype="float64").T

    d = np.inf

    k = 0
    while (norm(d) >= tol) and (k < kmax):
        p = -H * g
        a = wolfesearch(f, df, x, np.asarray(p.T)[0], amax=3, c1=tol, c2=0.1)
        d = a * p

        x = x + np.asarray(d.T)[0]
        coords.append(x)

        g_prev = g
        g = np.matrix(df(x), dtype="float64").T
        y = g - g_prev

        H += (d * d.T) / (d.T * y) - (H * y * y.T * H) / (y.T * H * y)

        neval += 2
        k += 1

    xmin = x
    fmin = f(x)

    answer_ = [xmin, fmin, neval, coords]
    return answer_


if __name__ == "__main__":
    x0 = np.array([[-3], [-3]])
    tol = 1e-9
    [xmin, f, neval, coords] = dfpsearch(fR, dfR, x0, tol)  # функция Розенброка
    print(xmin, f, neval)