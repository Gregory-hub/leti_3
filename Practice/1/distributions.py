from math import pi, exp, gamma

def normal_distribution(data: list, M: float, stdev: float) -> list:
    # M is math expectation, stdev is standard deviation
    normal_d_y = [1 / (stdev * (2 * pi)**(1/2)) * exp(-(1 / 2) * ((x - M) / stdev)**2) for x in data]
    normal_d_y /= sum(normal_d_y)
    return normal_d_y

def wiebull_distribution(data: list, xmin: float, k: float, M: float) -> list:
    # M is math expectation
    c = M / (gamma(1 / k + 1))
    weibull_d_y = []
    for x in data:
        if x < c:
            weibull_d_y.append(0)
        else:
            weibull_d_y.append(k / xmin * ((x - c) / xmin)**(k - 1) * exp(-((x - c) / xmin)**k) for x in data)

    return weibull_d_y
