import numpy as np


def f1(x): return x**2 - 10*np.cos(0.5*np.pi*x) - 110


def df1(x): return 2*x + 5*np.pi*np.sin(np.pi*x/2)


def f2(x): return -2*np.sin(np.sqrt(abs(x/2 + 10))) - x*np.sin(np.sqrt(abs(x - 10)))


def df2(x):
    d = 1e-12
    result = (f2(x + d) - f2(x - d)) / (2 * d)
    return result


def f_test(x): return x**2 - 10*np.cos(0.3*np.pi*x) - 20   # [-5, 3] xmin = 0, fmin = -30
def df_test(x): return 2*x + 3*np.pi*np.sin(0.3*np.pi*x)


def msearch(f, df, interval, tol):
    coords = []
    a = interval[0]
    b = interval[1]

    x = a + (b - a) / 2
    coords.append(x)

    k = 0
    deltaX = 1
    g_x = df(x)
    g_a = df(a)
    g_b = df(b)
    neval = 3

    while abs(deltaX) > tol and k < 1000:
        w = (g_x - g_b) / (x - b) + (g_x - g_a) / (x - a) - (g_b - g_a) / (b - a)
        g3 = ((g_x - g_b) / (x - b) - (g_b - g_a) / (b - a)) / (x - a)

        d1 = w + np.sqrt(w**2 - 4 * g_x * g3)
        d2 = w - np.sqrt(w**2 - 4 * g_x * g3)

        if abs(d2) < abs(d1):
            x_new = x - 2 * g_x / d1
        else:
            x_new = x - 2 * g_x / d2

        if x_new < x:
            b = x
            g_b = df(b)
        else:
            a = x
            g_a = df(a)

        deltaX = x_new - x
        x = x_new
        g_x = df(x)
        coords.append(x)
        k += 2

    xmin = x
    fmin = f(xmin)
    neval = neval + k

    answer_ = [xmin, fmin, neval, coords]
    return answer_


if __name__ == '__main__':
    interval = [-2, 9.99]
    tol = 1e-6
    # [xmin, f, neval, coords] = msearch(f1, df1, interval, tol)
    [xmin, f, neval, coords] = msearch(f2, df2, interval, tol)
    # [xmin, f, neval, coords] = msearch(f_test, df_test, [-5, 2], tol)
    print()
    print([xmin, f, neval])
