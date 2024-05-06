import numpy as np
import sys
from numpy.linalg import norm
from numpy.linalg import inv
from numpy import dot


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


def goldensectionsearch(f, interval, tol):
    a = interval[0]
    b = interval[1]
    Phi = (1 + np.sqrt(5)) / 2
    L = b - a
    x1 = b - L / Phi
    x2 = a + L / Phi
    y1 = f(x1)
    y2 = f(x2)
    neval = 2
    xmin = x1
    fmin = y1

    # main loop
    while np.abs(L) > tol:
        if y1 > y2:
            a = x1
            xmin = x2
            fmin = y2
            x1 = x2
            y1 = y2
            L = b - a
            x2 = a + L / Phi
            y2 = f(x2)
            neval += 1
        else:
            b = x2
            xmin = x1
            fmin = y1
            x2 = x1
            y2 = y1
            L = b - a
            x1 = b - L / Phi
            y1 = f(x1)
            neval += 1

    answer_ = [xmin, fmin, neval]
    return answer_


def pparam(pU, pB, tau):
    if (tau <= 1):
        p = dot(tau, pU)
    else:
        p = pU + (tau - 1) * (pB - pU)
    return p


def doglegsearch(mod, g0, B0, Delta, tol):
    # dogleg local search
    xcv = dot(-g0.transpose(), g0) / dot(dot(g0.transpose(), B0), g0)
    pU = xcv *g0
    xcvb = inv(- B0)
    pB = dot(inv(- B0), g0)

    func = lambda x: mod(dot(x, pB))
    al = goldensectionsearch(func, [-Delta / norm(pB), Delta / norm(pB)], tol)[0]
    pB = al * pB
    func_pau = lambda x: mod(pparam(pU, pB, x))
    tau = goldensectionsearch(func_pau, [0, 2], tol)[0]
    pmin = pparam(pU, pB, tau)
    if norm(pmin) > Delta:
        pmin_dop = (Delta / norm(pmin))
        pmin = dot(pmin_dop, pmin)
    return pmin


def trustreg(f, df, x0, tol):
# TRUSTREG searches for minimum using trust region method
# 	answer_ = trustreg(f, df, x0, tol)
#   INPUT ARGUMENTS
#   f  - objective function
#   df - gradient
# 	x0 - start point
# 	tol - set for bot range and function value
#   OUTPUT ARGUMENTS
#   answer_ = [xmin, fmin, neval, coords, radii]
# 	xmin is a function minimizer
# 	fmin = f(xmin)
# 	neval - number of function evaluations
#   coords - array of statistics
#   radii - array of trust regions radii

    #PLACE YOUR CODE HERE
    coords = []
    radii = []
    kmax = 1000

    eta = 0.1

    x = x0
    f_x = f(x)
    g = df(x)
    dx = np.array([[np.inf], [np.inf]])
    H = np.array(np.identity(2))
    delta = 1
    max_delta = 3
    coords.append(x)
    radii.append(delta)
    neval = 1

    k = 0
    while (norm(dx) >= tol) and (k < kmax):
        m = lambda p: f_x + dot(p.transpose(), g) + 0.5 * dot(dot(p.transpose(), H), p)
        p_min = doglegsearch(m, g, H, delta, tol)

        x_new = x + p_min
        f_new = f(x_new)
        ro = ((f_x - f_new) / (m(np.array([[0], [0]])) - m(p_min)))[0, 0]

        if ro > eta:
            dx = x_new - x
            x = x_new

            f_x = f_new

            g_new = df(x)
            neval += 1
            dg = g_new - g
            g = g_new

            H += dot(dg, dg.transpose()) / dot(dg.transpose(), dx) - dot(dot(dot(H, dx), dx.transpose()), H.transpose()) / dot(dot(dx.transpose(), H), dx)

        if ro < 0.25:
            delta *= 0.25
        elif ro > 0.75 and norm(p_min) == delta:
            delta = min(2 * delta, max_delta)

        k += 1
        coords.append(x)
        radii.append(delta)

    xmin = x
    fmin = f(xmin)

    answer_ = [xmin, fmin, neval, coords, radii]
    return answer_


if __name__ == "__main__":
    x0 = np.array([[-2], [0]])
    tol = 1e-3
    [xmin, f, neval, coords, rad] = trustreg(fR, dfR, x0, tol)  # r - функция Розенброка
    print(xmin, f, neval)
