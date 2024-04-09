using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    using System;

    class QRDecomposition
    {
        static void Main()
        {
            double[,] A = { { 10, -7, 0 },
                         { -3, 6, 2 },
                         { 5, 1, 5 } };

            int m = A.GetLength(0);
            int n = A.GetLength(1);

            double[,] Q = new double[m, m];
            double[,] R = A.Clone() as double[,];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (i != j)
                    {
                        Q[i, j] = 0;
                    }
                    else
                    {
                        Q[i, j] = 1;
                    }
                }
            }

            // Применяем отражения Хаусхолдера к R и собираем Q
            for (int j = 0; j < n; j++)
            {
                double[] v = new double[m - j];
                for (int i = j, k = 0; i < m; i++, k++)
                {
                    v[k] = R[i, j];
                }

                double[] u = new double[m - j];
                u[0] = v[0] + Math.Sign(v[0]) * Norm(v);

                for (int i = 1; i < u.Length; i++)
                {
                    u[i] = v[i];
                }

                double norm_u = Norm(u);
                double[,] H = new double[m, m];
                for (int i = 0; i < m; i++)
                {
                    H[i, i] = 1;
                }

                for (int i = j; i < m; i++)
                {
                    for (int k = j; k < m; k++)
                    {
                        H[i, k] -= 2 * (u[i - j] * u[k - j]) / (norm_u * norm_u);
                    }
                }

                R = Multiply(H, R);
                Q = Multiply(Q, H);
            }

            Console.WriteLine("Q:");
            PrintMatrix(Q);
            Console.WriteLine("R:");
            PrintMatrix(R);
            Console.WriteLine("Проверка QR == A: " + CheckEquality(A, Multiply(Q, R)));
        }

        static double Norm(double[] v)
        {
            double sum = 0;
            foreach (double val in v)
            {
                sum += val * val;
            }
            return Math.Sqrt(sum);
        }

        static double[,] Multiply(double[,] A, double[,] B)
        {
            int n = A.GetLength(0);
            int m = B.GetLength(1);
            int p = A.GetLength(1);

            double[,] C = new double[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < p; k++)
                    {
                        sum += A[i, k] * B[k, j];
                    }
                    C[i, j] = sum;
                }
            }
            return C;
        }

        static double[,] Transpose(double[,] A)
        {
            int m = A.GetLength(0);
            int n = A.GetLength(1);

            double[,] B = new double[n, m];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    B[j, i] = A[i, j];
                }
            }
            return B;
        }

        static bool CheckEquality(double[,] A, double[,] B)
        {
            int m = A.GetLength(0);
            int n = A.GetLength(1);

            if (m != B.GetLength(0) || n != B.GetLength(1))
                return false;

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (Math.Abs(A[i, j] - B[i, j]) > 1e-6)
                        return false;
                }
            }
            return true;
        }

        static void PrintMatrix(double[,] A)
        {
            int m = A.GetLength(0);
            int n = A.GetLength(1);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(A[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }

}
