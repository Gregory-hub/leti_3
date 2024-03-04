import numpy as np

def f(x): return x**2 -  10*np.cos(0.3*np.pi*x) - 20
def df(x): return 2*x + 3*np.pi*np.sin(0.3*np.pi*x)

def ssearch(interval,tol):
# SSEARCH searches for minimum using secant method
# 	answer_ = ssearch(interval,tol)
#   INPUT ARGUMENTS
# 	interval = [a, b] - search interval
# 	tol - set for bot range and function value
#   OUTPUT ARGUMENTS
#   answer_ = [xmin, fmin, neval, coords]
# 	xmin is a function minimizer
# 	fmin = f(xmin)
# 	neval - number of function evaluations
#   coords - array of x values found during optimization    

	#PLACE YOUR CODE HERE
    coords = []
    a, b = interval

    df_b = df(b)
    x = b - df_b * (b - a) / (df_b - df(a))
    df_x = df(x)

    neval = 3
    while np.abs(df_x) > tol and np.abs(b - a) > tol:
        df_b = df(b)
        x = b - df_b * (b - a) / (df_b - df(a))
        df_x = df(x)
        if df_x > 0:
            b = x
        else:
            a = x

        coords.append([x, a, b])
        neval += 3

    xmin = x
    fmin = f(x)

    answer_ = [xmin, fmin, neval, coords]
    return answer_
