using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace SLAR_CS
{
    internal class Method : EnumSolutions
    {
        private const byte Red = 227;
        private const byte Green = 66;
        private const byte Blue = 66;
        private const byte Alpha = 128;
        private const string MethodNotSelected = "Метод не обрано";
        public string selectedMethod;
        public double[] output;
        public int methodComplexity;
        public double[,] intermediateMatrix;
        public IsError resultState = IsError.Success;

        public Method()
        {

        }

        public void SelectionAndValidation(ComboBox method)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)method.SelectedItem;
            if (selectedItem == null)
            {
                MethodBackground(method);
            }
            else
            {
                selectedMethod = selectedItem.Content.ToString();
                method.Background = Brushes.Transparent;
                method.ToolTip = null;
            }
        }

        public void MethodBackground(ComboBox method)
        {
            Color color = Color.FromArgb(Alpha, Red, Green, Blue);
            SolidColorBrush brush = new SolidColorBrush(color);
            method.Background = brush;
            method.ToolTip = MethodNotSelected;
        }
        public void GaussMethod(double[,] matrix, int size)
        {
            methodComplexity = 0;
            resultState = IsError.Success;
            output = new double[size];

            // заміна опорних діагональних елементів, які дорівнюють 0
            for (int i = 0; i < size; i++)
            {
                methodComplexity++;
                methodComplexity += RemoveZero(size, i, matrix);

                //Перевірка на скінченність розв'язків
                isInf(matrix[i, i], matrix[i, size]);
                isExist(size, matrix);
                if (resultState == IsError.Inf)
                {
                    return;
                }
                else if (resultState == IsError.Undefined)
                {
                    return;
                }

                // Прямий хід методу Гаусса: нормалізація матриці
                for (int j = i + 1; j < size; j++)
                {
                    methodComplexity++;
                    double coefficient = matrix[j, i] / matrix[i, i]; 
                    for (int n = i; n < size; n++)
                    {
                        methodComplexity++;
                        matrix[j, n] -= coefficient * matrix[i, n]; 
                    }
                    matrix[j, size] -= coefficient * matrix[i, size]; 
                }
            }

            //запис проміжних результатів
            intermediatePhase(size, matrix);

            // Зворотній хід методу Гаусса: обчислення розв'язку
            for (int i = size - 1; i >= 0; i--)
            {
                methodComplexity++;
                output[i] = matrix[i, size];
                for (int j = i + 1; j < size; j++)
                {
                    methodComplexity++;
                    output[i] -= matrix[i, j] * output[j];  
                }
                output[i] /= matrix[i, i];
            }
        }
        public void JordanGaussMethod(double[,] matrix, int size)
        {
            methodComplexity = 0;
            resultState = IsError.Success;
            output = new double[size];

            for (int i = 0; i < size; i++)
            {
                methodComplexity++;
                methodComplexity += RemoveZero(size, i, matrix);

                //Перевірка на скінченність розв'язків
                isInf(matrix[i, i], matrix[i, size]);
                isExist(size, matrix);
                if (resultState == IsError.Inf)
                {
                    return;
                }
                else if (resultState == IsError.Undefined)
                {
                    return;
                }

                //Зведення матриці до трикутної форми
                double pivot = matrix[i, i];

                for (int j = i; j < size; j++)
                {
                    methodComplexity++;
                    matrix[i, j] /= pivot;
                }

                matrix[i, size] /= pivot;

                for (int j = i + 1; j < size; j++)
                {
                    methodComplexity++;
                    double coefficient = matrix[j, i];
                    for (int k = i; k < size; k++)
                    {
                        methodComplexity++;
                        matrix[j, k] -= coefficient * matrix[i, k];
                    }
                    matrix[j, size] -= coefficient * matrix[i, size];
                }
            }

            //запис проміжних результатів
            intermediatePhase(size, matrix);

            // Зводимо матрицю до одиничної форми
            for (int i = size - 1; i > 0; i--)
            {
                methodComplexity++;
                for (int j = i - 1; j >= 0; j--)
                {
                    methodComplexity++;
                    double coefficient = matrix[j, i];
                    for (int k = i; k < size; k++)
                    {
                        methodComplexity++;
                        matrix[j, k] -= coefficient * matrix[i, k];
                    }
                    matrix[j, size] -= coefficient * matrix[i, size];
                }
            }

            output = new double[size];
            for (int i = 0; i < size; i++)
            {
                output[i] = matrix[i, size];
            }
        }
        public void MatrixMethod(double[,] matrix, int size)
        {
            methodComplexity = 0;
            double[,] inverseMatrix = new double[size, size];
            output = new double[size];
            //створення одиничної матриці
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i != j)
                    {
                        inverseMatrix[i, j] = 0;
                    }
                    else
                    {
                        inverseMatrix[i, j] = 1.0;
                    }
                }
            }

            for (int i = 0; i < size; i++)
            {
                methodComplexity++;
                if (matrix[i, i] == 0)
                {
                    // Пошук ненульового елемента у стовпці для обміну рядків
                    for (int j = i + 1; j < size - 1; j++)
                    {
                        methodComplexity++;
                        if (matrix[j, i] != 0)
                        {
                            // Обмін рядків, якщо знайдено ненульовий елемент
                            for (int n = 0; n < size; n++)
                            {
                                methodComplexity++;
                                double temp = matrix[i, n];
                                matrix[i, n] = matrix[j, n];
                                matrix[j, n] = temp;
                                temp = inverseMatrix[i, n];
                                inverseMatrix[i, n] = inverseMatrix[j, n];
                                inverseMatrix[j, n] = temp;
                            }
                            break; // Вихід з циклу після обміну рядків   
                        }
                    }
                }

                //Зведення матриці до трикутної форми
                double pivot = matrix[i, i];

                for (int j = 0; j < size; j++)
                {
                    methodComplexity++;
                    matrix[i, j] /= pivot;
                    inverseMatrix[i, j] /= pivot;
                }

                for (int j = i + 1; j < size; j++)
                {
                    methodComplexity++;
                    double coefficient = matrix[j, i];
                    for (int k = 0; k < size; k++)
                    {
                        methodComplexity++;
                        matrix[j, k] -= coefficient * matrix[i, k];
                        inverseMatrix[j, k] -= coefficient * inverseMatrix[i, k];
                    }
                }
            }

            // Зводимо матрицю до одиничної форми
            for (int i = size - 1; i >= 0; i--)
            {
                methodComplexity++;
                for (int j = i - 1; j >= 0; j--)
                {
                    methodComplexity++;
                    double coefficient = matrix[j, i];
                    for (int k = 0; k < size; k++)
                    {
                        methodComplexity++;
                        matrix[j, k] -= coefficient * matrix[i, k];
                        inverseMatrix[j, k] -= coefficient * inverseMatrix[i, k];
                    }
                }
            }

            //запис проміжних результатів
            intermediateMatrix = new double[size, size];
            for (int k = 0; k < size; k++)
            {
                for (int j = 0; j < size; j++)
                {
                    intermediateMatrix[k, j] = inverseMatrix[k, j];
                }
            }

            //Обчислюємо розв'язок системи
            for (int i = 0; i < size; i++)
            {
                methodComplexity++;
                output[i] = 0.0;
                for (int j = 0; j < size; j++)
                {
                    methodComplexity++;
                    output[i] += inverseMatrix[i, j] * matrix[j, size];
                }
            }
        }

        private int RemoveZero(int size, int i, double[,] matrix)
        {
            int counter = 0;
            if (matrix[i, i] == 0)
            {
                // Пошук ненульового елемента у стовпці для обміну рядків
                for (int j = i + 1; j < size; j++)
                {
                    counter++;
                    if (matrix[j, i] != 0)
                    {
                        // Обмін рядків, якщо знайдено ненульовий елемент
                        for (int n = 0; n < size; n++)
                        {
                            counter++;
                            double temp = matrix[i, n];
                            matrix[i, n] = matrix[j, n];
                            matrix[j, n] = temp;
                            temp = matrix[i, size];
                            matrix[i, size] = matrix[j, size];
                            matrix[j, size] = temp;
                        }
                        break; // Вихід з циклу після обміну рядків   
                    }
                }
            }
            return counter;
        }

        private void isExist(int size, double[,] matrix)
        {
            for (int i = 0; i < size; i++)
            {
                int zeromethodComplexity = 0;
                for (int j = 0; j < size; j++)
                {
                    if (matrix[i, j] == 0)
                    {
                        zeromethodComplexity++;
                    }
                }
                if (zeromethodComplexity == size && matrix[i, size] != 0)
                {
                    resultState = IsError.Undefined;
                    return;
                }
            }
        }

        private void isInf(double elem, double b)
        {
            if (elem == 0)//перевірка, чи опорний елемент дорівнює нулю
            {
                resultState = IsError.Inf;
                return;
            }
        }

        private void intermediatePhase(int size, double[,] matrix)
        {
            intermediateMatrix = new double[size, size + 1];
            for (int k = 0; k < size; k++)
            {
                for (int j = 0; j < size + 1; j++)
                {
                    intermediateMatrix[k, j] = matrix[k, j];
                }
            }
        }
    }
}
