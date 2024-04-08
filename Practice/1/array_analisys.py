import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sb
from scipy import stats

from distributions import *


def smooth(y, box_pts):
    box = np.ones(box_pts)/box_pts
    y_smooth = np.convolve(y, box, mode='same')
    return y_smooth


data = np.array([1.64, 2.07, 2.41, 1.73, 2.48, 9.49, 1.61, 2.17, 9.22, 1.26, -1.02, -1.78,
               -7.97, 1.66, 1.7, -1.88, 1.71, -2.85, 2.69, -4, -3.3, -5.34, -4.97, -3.21,
               -6.22, -5.01, -6.09, -6.72, -5.64, -9.05, -7.26, -7.03, -6.19, -6.71, -7.63, -9.09,
               -8.67, -7.73, -7.31, -7.55, -8.44, -7.33, -5.68, -4.47, -5.33, -5.11, -5.54, -3.37,
               -2.32, -4.18, -3.39, -2.73, -3.88, -4.02, -6.33, -5.84, -5.83, -5.59, -7.63, -6.59,
               -7.38, -6.88, -8.89, -8.79, -1.7, -2, -10.8, 10.2, -9.08, -8.81, -11.1, -11.7,
               -11.3, -10.5, -11.8, -10.6, 8.9, -9.07, -8.9, -8.26, -7.31, -8, -8.62, -8.15,
               -8.47, -11, -11.1, -12.8, -12.2, -11.5, -11.1, -12.2, -12.3, -12.9, -12.9, -12.1,
               -12.1, -13.6, -12, -12.2, -13.5, -14.1, -14.9, -12.8, -10.6, -11.3, -12.4, -13.6,
               -14.2, -15.1, -15.2, -14.1, -14.7, -16.7, -15.7, -16.6, -16.4, -17.3, -15.5, -15.2,
               -14, -13.6, -13.2, -15.6, -14.9, -15, -14.8, -15.3, -13, -13.5, -13.6, -13.2,
               -14, -14.3, -15, -17, -17.5, -16.6, -14.9, -14, -12.2, -12.7, -13.7, -12.9])

N = len(data)
M = D = A = E = 0

datasum = sum(data)
M = datasum / N
D = sum([(x - M)**2 for x in data]) / N
M3 = sum([(x - M)**3 for x in data]) / N
M4 = sum([(x - M)**4 for x in data]) / N
stdev = D**(1/2)
A = M3 / stdev**3
E = M4 / stdev**4 - 3

# A = (sum([x**4 for x in data]) - 4 / N * sum([x**4 for x in data]) * 
#     datasum + 6 / N**2 * sum([x**2 for x in data]) * datasum**2 - 
#     4 / N**3 * datasum**4) / N

num_points = 50
x_min, x_max = min(data), max(data)
step = (x_max - x_min) / num_points

dist_x = []
x = x_min
probability = {}
for i in range(num_points):
    dist_x.append(x)
    count = 0
    for j in range(len(data)):
        if data[j] < x:
            count += 1

    probability[x] = count / N
    x += step

dist_y = []
for i in range(len(dist_x)):
    x = dist_x[i]
    dist_y.append(probability[x])

dist_y = np.gradient(dist_y)

normal_d_y = normal_distribution(dist_x, M, stdev)

# k = 2.5
# wiebull_d_y = wiebull_distribution(dist_x, x_min, k, M)

dist = pd.DataFrame({
    "X": dist_x,
    "Distribution": dist_y,
    "Normal distribution": normal_d_y,
    "Normal distribution smoothed": smooth(normal_d_y, 20),
    # "Wiebull distribution": wiebull_d_y    
})

# P = 
# mode = stats.mode(data).mode
# n = 

print("Математическое ожидание:", M)
print("Дисперсия", D)
print("Ассиметрия:", A)
print("Эксцесс:", E)

# sb.histplot(data, stat="probability", bins=num_points, kde=True)
plt.plot(dist["X"], dist["Distribution"], color="c")
plt.plot(dist["X"], dist["Normal distribution smoothed"], color="purple")
plt.plot(dist["X"], dist["Normal distribution"], color="r")
# plt.plot(dist["X"], dist["Wiebull distribution"], color="y")
plt.legend(dist.keys()[1:])
plt.xlabel("x")
plt.ylabel("p(x)")
plt.grid()
plt.show()
