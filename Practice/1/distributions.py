from math import pi, exp


def normal_distribution(data: list, M: float, stdev: float) -> list:
    # M is math expectation, stdev is standard deviation
    normal_d_y = [1 / (stdev * (2 * pi)**(1/2)) * exp(-(1 / 2) * ((x - M) / stdev)**2) for x in data]
    normal_d_y /= sum(normal_d_y)
    return normal_d_y
