using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QRdecomposition
{
    public interface IMatrixPrinter
    {
        void PrintMatrix(double[,] array);
    }
    abstract class Matrix
    {
        protected double[,] matrix;
        public int sizeMatrix;
        public double[,] GetMatrix()
        {
            return matrix;
        }

        public int GetSizeMatrix()
        {
            return sizeMatrix;
        }

        public virtual void matrixInputFromFile()
        {
            try
            {
                // Путь к файлу для чтения
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Matrix.txt");
                string[] Lines = File.ReadAllLines(filePath);
                int rowCount = Lines.Length;
                int columnCount = Lines[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                sizeMatrix = rowCount;
                if (rowCount != columnCount)
                {
                    Console.WriteLine("Матрица не является квадратной. ");
                    Console.WriteLine("Программа завершена. ");
                    Environment.Exit(0);

                }

                matrix = new double[rowCount, rowCount];

                for (int i = 0; i < rowCount; i++)
                {
                    string[] values = Lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < columnCount; j++)
                    {
                        matrix[i, j] = Convert.ToDouble(values[j]);
                    }
                }


            }
            catch (IOException e)
            {
                Console.WriteLine("Ошибка при чтении файла: " + e.Message);

            }

        }

        public virtual void matrixInput()
        {
            Console.WriteLine("Введите размер квадратной матрицы: ");
            sizeMatrix = Convert.ToInt32(Console.ReadLine());
            matrix = new double[sizeMatrix, sizeMatrix];

            for (int i = 0; i < sizeMatrix; i++)
            {
                for (int j = 0; j < sizeMatrix; j++)
                {
                    Console.WriteLine("Введите " + (i + 1) + " " + (j + 1) + " " + " элемент матрицы");
                    matrix[i, j] = Convert.ToDouble(Console.ReadLine());
                }
            }
        }

        public virtual void printMatrix()
        {
            for (int i = 0; i < sizeMatrix; i++)
            {
                if (i != 0)
                {
                    Console.WriteLine();
                }

                for (int j = 0; j < sizeMatrix; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
            }
            Console.WriteLine();

        }


    }

    class MainMatrix : Matrix
    {
        public void convergenceOfMethods() // проверка на сходимость
        {
            double[,] predominanceOfDiagonalElements = new double[sizeMatrix, 2];
            double sumOfRow = 0;
            int countDiag = 0;
            for (int i = 0; i < sizeMatrix; i++)
            {
                sumOfRow = 0;
                for (int j = 0; j < sizeMatrix; j++)
                {
                    sumOfRow += Math.Abs(matrix[i, j]);

                    if (i == j)
                    {
                        sumOfRow -= Math.Abs(matrix[i, j]);

                        predominanceOfDiagonalElements[i, 0] = Math.Abs(matrix[i, j]);
                    }
                    predominanceOfDiagonalElements[i, 1] = sumOfRow;
                }
            }

            for (int i = 0; i < sizeMatrix; i++)
            {
                if (predominanceOfDiagonalElements[i, 0] >= predominanceOfDiagonalElements[i, 1])
                {
                    countDiag++;
                }
            }
            if (countDiag == sizeMatrix)
            {
                Console.WriteLine("Итерационные методы сходятся. ");
            }
            else
            {
                Console.WriteLine("Нет сходимости итерационных методов, возможно, некоторые методы сходится не будут.");
                Console.WriteLine("Рекомендуется преобразовать матрицу и ввести ее снова.");
            }
        }

        public override void matrixInputFromFile()
        {
            Console.WriteLine("Введенная матрица A: ");
            base.matrixInputFromFile();
        }

    }

    class FreeMatrix : Matrix
    {
        private int rowCount;
        private int columnCount;
        public override void matrixInputFromFile()
        {
            try
            {

                // Путь к файлу для чтения
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FreeMembers.txt");
                string[] Lines = File.ReadAllLines(filePath);
                rowCount = Lines.Length;
                columnCount = Lines[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                sizeMatrix = rowCount;

                matrix = new double[rowCount, columnCount];

                for (int i = 0; i < rowCount; i++)
                {
                    string[] values = Lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < columnCount; j++)
                    {
                        matrix[i, j] = Convert.ToDouble(values[j]);
                    }
                }


            }
            catch (IOException e)
            {
                Console.WriteLine("Ошибка при чтении файла: " + e.Message);

            }
        }

        public override void printMatrix()
        {
            Console.WriteLine("Введенный вектор свободных членов B: ");
            for (int i = 0; i < rowCount; i++)
            {
                if (i != 0)
                {
                    Console.WriteLine();
                }

                for (int j = 0; j < columnCount; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
            }
            Console.WriteLine();
        }
        public void matrixVector(int sizeMatrix)
        {

            Console.WriteLine("Введите вектор свободных значений");

            matrix = new double[sizeMatrix, 1];

            for (int i = 0; i < sizeMatrix; i++)
            {
                {
                    Console.WriteLine("Введите " + (i + 1) + " элемент вектора");
                    matrix[i, 0] = Convert.ToDouble(Console.ReadLine());
                }
            }
        }
    }

    class SLAE : IMatrixPrinter
    {

        MainMatrix mainMatrix = new MainMatrix();
        FreeMatrix freeMembers = new FreeMatrix();
        

        private double[,] Amatrix;
        private double[] Free;
        private double[,] Qmatrix;
        private double[,] Rmatrix;

        private MatrixComputingOperations operations = new MatrixComputingOperations();

        private double determinant;
        int size;
        int round;

        public SLAE(MainMatrix matrix, FreeMatrix freeMembers, int round)
        {

            Amatrix = matrix.GetMatrix();
            this.mainMatrix = matrix;
            this.freeMembers = freeMembers;
            Qmatrix = new double[mainMatrix.sizeMatrix, mainMatrix.sizeMatrix];
            Rmatrix = new double[mainMatrix.sizeMatrix, mainMatrix.sizeMatrix];
            size = mainMatrix.GetSizeMatrix();
            Qmatrix = operations.CreateIdentityMatrix(size);
            this.Free = new double[size];
            double[,] Free = new double[size, 1];
            Free = freeMembers.GetMatrix();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Rmatrix[i, j] = Amatrix[i, j];
                }
            }
            this.round = round;
            for (int i = 0; i < size; i++)
            {
                this.Free[i] = Free[i, 0];
            }
           


        }
        public void PrintMatrix(double[,] array)
        {
            for (int i = 0; i < size; i++)
            {
                if (i != 0)
                {
                    Console.WriteLine();
                }

                for (int j = 0; j < size; j++)
                {
                    Console.Write(Math.Round(array[i, j], round) + " ");
                }
            }
        }


        public void QRdecomposition()
        {
            double[] colA = new double[size];
            double[] hhvector = new double[size];
            double[,] H = new double[size, size];
            double[,] IdentityMatrix = new double[size, size];
            IdentityMatrix = operations.CreateIdentityMatrix(size);
            for (int i = 0; i < size - 1; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    colA[j] = Rmatrix[j, i];
                }
                for (int j = 0; j < i; j++)
                {
                    colA[j] = 0;
                }
                
                hhvector = HouseholderVector(colA, i);
                Console.WriteLine();
               
                H = operations.MatrixMultiplication(hhvector, hhvector);
                
                H = operations.MatrixScalarMultiplicationAndDivision(H, 2, '*');
              
                H = operations.AdditionMatrix(IdentityMatrix, H, '-');
               
                Rmatrix = operations.MatrixMultiplication(H, Rmatrix);
   
                Qmatrix = operations.MatrixMultiplication(Qmatrix,H);
                
            }
            
            double[,] QxR = new double[size, size];
            QxR = operations.MatrixMultiplication(Qmatrix, Rmatrix);
            Console.WriteLine("Matrix Q:");
            PrintMatrix(Qmatrix);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Matrix R:");
            PrintMatrix(Rmatrix);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Multiplication Q x R");
            PrintMatrix(QxR);
            Console.WriteLine();


        }
        private double[] HouseholderVector(double[] column, int iteration)
        {
            int SizeVector = column.GetLength(0);
            double[,] E = new double[SizeVector, SizeVector]; // создаем единичную матрицу
            double[] w = new double[SizeVector];
            E = operations.CreateIdentityMatrix(SizeVector);
            double[] Ecol = new double[SizeVector];
            for (int i = 0; i < SizeVector; i++)
            {
                Ecol[i] = E[i, iteration];
            }
            for (int i = 0; i < column.GetLength(0); i++)
            {
                //Console.Write(column[i] + " ");
            }
            //double[] HouseholderVector = new double[SizeVector];
            w = operations.MatrixScalarMultiplicationAndDivision(Ecol, operations.NormVector(column), '*');
            for (int i = 0; i < column.GetLength(0); i++)
            {
                //Console.WriteLine("w = " + w[i]);
            }
            w = operations.AdditionMatrix(column, w, '-');
            //Console.WriteLine("w = " + NormVector(w));
            for (int i = 0; i < w.GetLength(0); i++)
            {
               //Console.WriteLine("w == " + w[i]);
            }
            //Console.WriteLine("norm " + NormVector(w));
            w = operations.MatrixScalarMultiplicationAndDivision(w, operations.NormVector(w), '/');
            for (int i = 0; i < w.GetLength(0); i++)
            {
                //Console.WriteLine("w == " + w[i]);
            }
            return w;
        }
        public void SLAESolution()
        {
            double[] Xvalues = new double[size]; // находим решение системы Rx = QTb
            double[] QTb = new double[size];
            QTb = operations.MatrixMultiplication(operations.Transpose(Qmatrix), Free);
            double sum = 0;
            for (int i = 0; i < size; i++)
            {
                sum = 0;
                for (int j = 0; j < size; j++)
                {
                    if (i != j)
                    {
                        sum = sum + Rmatrix[size - i - 1, size - j - 1] * Xvalues[size - j - 1];
                    }

                }
                Xvalues[size - i - 1] = (QTb[size - i - 1] - sum) / Rmatrix[size - i - 1, size - i - 1];

            }
            Console.WriteLine();
            for (int i = 0; i < size; i++)
            {
                Console.WriteLine("X" + (i + 1) + " = " + Xvalues[i]);
            }


        }
        public void determinantCalculation()
        {
            double RmatrixDeterminant = 1;
            for (int i = 0; i < size; i++)
            {
                RmatrixDeterminant *= Rmatrix[i, i];
            }
            determinant = RmatrixDeterminant;
            Console.WriteLine(determinant);
        }
        
    }

    class MatrixComputingOperations
    {
        public double[,] MatrixMultiplication(double[,] A, double[,] B)
        {
            int rowsA = A.GetLength(0);
            int colsB = B.GetLength(1);
            int colsA = A.GetLength(1);
            int rowsB = B.GetLength(0);
            if (colsA != rowsB)
            {
                throw new InvalidOperationException("Матрицы таких размерностей не могут быть умножены.");
            }

            double[,] AB = new double[rowsA, colsB];
            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < colsA; k++)
                    {
                        sum += A[i, k] * B[k, j];
                    }
                    AB[i, j] = sum;
                }
            }

            return AB;
        }
        public double[] MatrixScalarMultiplicationAndDivision(double[] Vector, double C, char OperationType)
        {
            int rows = Vector.GetLength(0);
            double[] VectorC = new double[rows];
            for (int i = 0; i < rows; i++)
            {
                if (OperationType == '*')
                {
                    VectorC[i] = Vector[i] * C;
                }
                else if (OperationType == '/')
                {
                    VectorC[i] = Vector[i] / C;
                }
                else
                {
                    throw new InvalidOperationException("Метод поеддерживает только операции умножения и деления");
                }

            }
            return VectorC;
        }
        public double[] AdditionMatrix(double[] A, double[] B, char OperationType)
        {
            int rowsA = A.GetLength(0);

            int rowsB = B.GetLength(0);

            if (rowsA != rowsB)
            {
                throw new InvalidOperationException("Векторы имеют разные размерности.");
            }

            double[] AsumB = new double[rowsA];

            for (int i = 0; i < rowsA; i++)
            {
                if (OperationType == '+')
                {

                    AsumB[i] = A[i] + B[i];
                }
                else if (OperationType == '-')
                {
                    AsumB[i] = A[i] - B[i];
                }
                else
                {
                    throw new InvalidOperationException("Метод поддерживает только операции сложения и вычитания.");
                }
            }
            return AsumB;
        }
        public double NormVector(double[] vector)
        {
            double norm = 0;
            for (int i = 0; i < vector.GetLength(0); i++)
            {
                norm += Math.Pow(vector[i], 2);
                //Console.WriteLine("norm " + Math.Pow(vector[i], 2));
            }
            norm = Math.Sqrt(norm);
            //Console.WriteLine("normsqrt = " + norm);
            return norm;

        }
        public double[,] Transpose(double[,] A)
        {

            int rows = A.GetLength(0);
            int cols = A.GetLength(1);
            double[,] TransposeMatrix = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    TransposeMatrix[j, i] = A[i, j];
                }
            }
            return TransposeMatrix;
        }
        public double[,] MatrixScalarMultiplicationAndDivision(double[,] Matrix, double C, char OperationType)
        {
            int rows = Matrix.GetLength(0);
            int cols = Matrix.GetLength(1);

            double[,] result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = Matrix[i, j] * C;
                }
            }

            return result;
        }
        public double[,] AdditionMatrix(double[,] A, double[,] B, char OperationType)
        {
            int rowsA = A.GetLength(0);
            int colsA = A.GetLength(1);
            int rowsB = B.GetLength(0);
            int colsB = B.GetLength(1);

            if (rowsA != rowsB || colsA != colsB)
            {
                throw new InvalidOperationException("Матрицы имеют разные размерности.");
            }

            double[,] AsumB = new double[rowsA, colsA];

            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsA; j++)
                {
                    if (OperationType == '+')
                        AsumB[i, j] = A[i, j] + B[i, j];
                    else if (OperationType == '-')
                    {
                        AsumB[i, j] = A[i, j] - B[i, j];
                    }
                    else
                    {
                        throw new InvalidOperationException("Метод поддерживает только операции сложения и вычитания.");
                    }
                }
            }
            return AsumB;
        }
        public double[,] MatrixMultiplication(double[] A, double[] B)
        {
            int size = A.GetLength(0);
            double[,] matrix = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = A[i] * B[j];
                }
            }
            return matrix;
        }
        public double[,] CreateIdentityMatrix(int size)
        {
            double[,] E = new double[size, size]; // создаем единичную матрицу

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i != j)
                    {
                        E[i, j] = 0;
                    }
                    else
                    {
                        E[i, j] = 1;
                    }
                }
            }
            return E;
        }
        public double[] MatrixMultiplication(double[,] A, double[] B)
        {
            int m = A.GetLength(0); // Количество строк в матрице A
            int n = A.GetLength(1); // Количество столбцов в матрице A
            int vectorLength = B.Length; // Длина вектора-столбца x

            double[] result = new double[m];
            for (int i = 0; i < m; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                {
                    sum += A[i, j] * B[j];
                }
                result[i] = sum;
            }
            return result;
        }
    }
    internal class Program
    {
       
        static void Main(string[] args)
        {
           
            int choice1;
            int choice2;
            int choice3;
            int roundNumber = 3;

            MainMatrix matrix1 = new MainMatrix();
            Console.WriteLine();
            Console.WriteLine("Матрицы хранятся в папке проекта, для редактирования файлов используйте путь: ");
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine("Выберите предложенный вариант - введите соответствующее варианту число ");
            Console.WriteLine("1. Ввести матрицу вручуню");
            Console.WriteLine("2. Ввести матрицу из файла Matrix.txt");
            choice1 = Convert.ToInt32(Console.ReadLine());
            if (choice1 == 1)
            {
                matrix1.matrixInput();
            }
            if (choice1 == 2)
            {
                matrix1.matrixInputFromFile();
            }

            matrix1.printMatrix();

            FreeMatrix matrix2 = new FreeMatrix();
            Console.WriteLine("Выберите предложенный вариант - введите соответствующее варианту число ");
            Console.WriteLine("1. Ввести матрицу вручуню");
            Console.WriteLine("2. Ввести матрицу из файла Matrix.txt");
            choice2 = Convert.ToInt32(Console.ReadLine());
            if (choice2 == 1)
            {
                matrix2.matrixVector(matrix1.sizeMatrix);
            }
            if (choice2 == 2)
            {
                matrix2.matrixInputFromFile();
            }

            matrix2.printMatrix();

            Console.WriteLine("Выберите предложенный вариант - введите соответствующее варианту число ");
            Console.WriteLine("1. Ввести количество знаков после запятой вручную");
            Console.WriteLine("2. Использовать округление до 3-х знаков после запятой.");
            Console.WriteLine("3. Вывести значения Q и R матрицы полностью. ");
            choice3 = Convert.ToInt32(Console.ReadLine());
            if (choice3 == 1)
            {
                Console.WriteLine("Введите количество знаков после запятой для элементов Q и R матриц");
                roundNumber = Convert.ToInt32(Console.ReadLine());
            }
            if (choice3 == 2)
            {
                roundNumber = 3;
            }
            if (choice3 == 3)
            {
                roundNumber = 14;
            }
            SLAE slae = new SLAE(matrix1, matrix2, roundNumber);
            slae.QRdecomposition();
            slae.SLAESolution();
            slae.determinantCalculation();
           
        }
    }
}
