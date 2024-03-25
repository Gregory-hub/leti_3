import numpy as np
import matplotlib.pyplot as plt
import seaborn as sb
# import statistics as stats

li = np.array([1.64, 2.07, 2.41, 1.73, 2.48, 9.49, 1.61, 2.17, 9.22, 1.26, -1.02, -1.78,
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

# a, b = min(li), max(li)
# num_of_points = 50
# step = (b - a) / num_of_points

# x = a
# probability = {}
# for i in range(num_of_points):
#     count = 0
#     for j in range(len(li)):
#         if li[j] < x:
#             count += 1

#     probability[x] = count / len(li)
#     x += step

print("Mean:", np.mean(li))
print("Median:", np.median(li))
print("Variance:", np.var(li))   # дисперсия
print("Standard deviation:", np.std(li))      # среднеквадратичное

sb.histplot(li, stat="probability", bins=50, kde=True)
# plt.plot(probability.keys(), probability.values())
# plt.plot(probability.keys(), np.gradient(list(probability.values())), 'r')
plt.grid()
plt.show()
