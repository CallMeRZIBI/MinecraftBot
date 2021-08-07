using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_bot_maybye
{
    class Matrix
    {
        private Random rnd = new Random();
        private double[,] matrix;
        private int rows_num;
        private int cols_num;

        public Matrix(int rows, int cols)
        {
            this.rows_num = rows;
            this.cols_num = cols;
            matrix = new double[rows_num, cols_num];
            for (var i = 0; i < rows_num; i++)
            {
                for (var j = 0; j < cols_num; j++)
                {
                    matrix[i, j] = 0;
                }
            }
        }

        public double[,] Random()
        {
            for (var i = 0; i < rows_num; i++)
            {
                for (var j = 0; j < cols_num; j++)
                {
                    if (rnd.Next(0, 2) == 1)
                    {
                        matrix[i, j] = rnd.NextDouble();
                    }
                    else
                    {
                        matrix[i, j] = rnd.NextDouble() * -1;
                    }
                }
            }
            return matrix;
        }

        public Matrix Multiply(double n)
        {
            var output = new Matrix(this.rows_num, this.cols_num);
            for (var i = 0; i < this.rows_num; i++)
            {
                for (var j = 0; j < this.cols_num; j++)
                {
                    output.matrix[i, j] = matrix[i, j] *= n;
                }
            }
            return output;
        }

        public double[,] Add(Matrix n)
        {
            for (var i = 0; i < rows_num; i++)
            {
                for (var j = 0; j < cols_num; j++)
                {
                    matrix[i, j] += n.matrix[i, j];
                }
            }
            return matrix;
        }

        public double[,] Add(int n)
        //scalar product

        {
            for (var i = 0; i < rows_num; i++)
            {
                for (var j = 0; j < cols_num; j++)
                {
                    matrix[i, j] += n;
                }
            }
            return matrix;
        }

        public static Matrix Subtract(Matrix a, Matrix b)
        {
            //return new matrix a-b
            var result = new Matrix(a.rows_num, a.cols_num);
            for (var i = 0; i < result.rows_num; i++)
            {
                for (var j = 0; j < result.cols_num; j++)
                {
                    result.matrix[i, j] = a.matrix[i, j] - b.matrix[i, j];
                }
            }
            return result;
        }

        public static Matrix Multiply(Matrix a, Matrix n)
        //matrix product
        {
            var result = new Matrix(a.rows_num, n.cols_num);
            var b = n;
            for (var i = 0; i < result.rows_num; i++)
            {
                for (var j = 0; j < result.cols_num; j++)
                {
                    //dot product in values of col
                    double sum = 0;
                    for (var k = 0; k < a.cols_num; k++)
                    {
                        sum += a.matrix[i, k] * b.matrix[k, j];
                    }
                    result.matrix[i, j] = sum;
                }
            }
            return result;
        }

        public static Matrix FromArray(double[] arr)
        {
            var m = new Matrix(arr.Length, 1);
            for (var i = 0; i < arr.Length; i++)
            {
                m.matrix[i, 0] = arr[i];
            }
            return m;
        }

        public double[,] ToArray()
        {
            double[,] arr = new double[this.rows_num, this.cols_num];
            for (var i = 0; i < this.rows_num; i++)
            {
                for (var j = 0; j < this.cols_num; j++)
                {
                    arr[i, j] = this.matrix[i, j];
                }
            }
            return arr;
        }

        public static Matrix Transpose(Matrix matrix)
        {
            var result = new Matrix(matrix.cols_num, matrix.rows_num);

            for (var i = 0; i < matrix.rows_num; i++)
            {
                for (var j = 0; j < matrix.cols_num; j++)
                {
                    result.matrix[j, i] = matrix.matrix[i, j];
                }
            }
            return result;
        }

        public void Map(Func<double, double> func)
        {
            for (var i = 0; i < this.rows_num; i++)
            {
                for (var j = 0; j < this.cols_num; j++)
                {
                    double val = this.matrix[i, j];
                    this.matrix[i, j] = func(val);
                }
            }
        }

        public static Matrix Map(Matrix matrix, Func<double, double> func)
        {
            var result = new Matrix(matrix.rows_num, matrix.cols_num);
            //apply a function for every element of function
            for (var i = 0; i < matrix.rows_num; i++)
            {
                for (var j = 0; j < matrix.cols_num; j++)
                {
                    double val = matrix.matrix[i, j];
                    result.matrix[i, j] = func(val);
                }
            }
            return result;
        }
}
}
