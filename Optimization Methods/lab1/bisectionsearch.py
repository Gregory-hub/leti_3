import numpy as np
def f(x): return 2 * (x ** 2) - 9 * x - 31
def df(x): return 4 * x - 9

def bsearch(interval, tol):
# searches for minimum using bisection method
# arguments: bisectionsearch(f,df,interval,tol)
# f - an objective function
# df -  an objective function derivative
# interval = [a, b] - search interval
#tol - tolerance for both range and function value
# output: [xmin, fmin, neval, coords]
# xmin - value of x in fmin
# fmin - minimul value of f
# neval - number of function evaluations
# coords - array of x values found during optimization

    a, b = interval

    coords = []
    neval = 1

    g = df(a)
    while b - a > tol and np.abs(g) > tol:
        x = (a + b) / 2
        df_x = df(x)

        if df_x > 0:
            b = x
        else:
            a = x
            g = df_x    # g is df(a)

        neval += 1
        coords.append(x)

    xmin = x
    fmin = f(x)
    neval += 1

    answer_ = [xmin, fmin, neval, coords]
    return answer_
