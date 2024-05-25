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

            //Перевірка на наявність розв'язків
            isExist(size, matrix);
            if (resultState == IsError.Undefined)
            {
                return;
            }

            // заміна опорних діагональних елементів, які дорівнюють 0
            for (int i = 0; i < size - 1; i++)
            {
                if (matrix[i, i] == 0)
                {
                    // Пошук ненульового елемента у стовпці для обміну рядків
                    for (int j = i + 1; j < size - 1; j++)
                    {
                        if (matrix[j, i] != 0)
                        {
                            // Обмін рядків, якщо знайдено ненульовий елемент
                            for (int n = 0; n < size; n++)
                            {
                                double temp = matrix[i, n];
                                matrix[i, n] = matrix[j, n];
                                matrix[j, n] = temp;
                                temp = matrix[i, size];
                                matrix[i, size] = matrix[j, size];
                                matrix[j, size] = temp;
                                methodComplexity++;
                            }
                            methodComplexity++;
                            break; // Вихід з циклу після обміну рядків
                        }
                        methodComplexity++;
                    }
                    methodComplexity++;
                }

                //Перевірка на скінченність розв'язків
                isInf(matrix[i, i]);
                if (resultState == IsError.Inf)
                {
                    return;
                }

                // Прямий хід методу Гаусса: нормалізація матриці
                for (int j = i + 1; j < size; j++)
                {
                    double coefficient = matrix[j, i] / matrix[i, i]; 
                    for (int n = i; n < size; n++)
                    {
                        matrix[j, n] -= coefficient * matrix[i, n]; 
                        if (Math.Abs(matrix[j, n]) < 1e-6)
                            matrix[j, n] = 0;
                        methodComplexity++;
                    }
                    matrix[j, size] -= coefficient * matrix[i, size]; 
                    methodComplexity += 2;
                }
                methodComplexity += 2;
            }
            methodComplexity++;

            //запис проміжних результатів
            intermediatePhase(size, matrix);

            // Зворотній хід методу Гаусса: обчислення розв'язку
            for (int i = size - 1; i >= 0; i--)
            {
                output[i] = matrix[i, size];
                for (int j = i + 1; j < size; j++)
                {
                    output[i] -= matrix[i, j] * output[j];
                    methodComplexity++;
                }
                output[i] /= matrix[i, i];
                methodComplexity += 2;
            }
            methodComplexity++;
        }
        public void JordanGaussMethod(double[,] matrix, int size)
        {
            methodComplexity = 0;
            resultState = IsError.Success;
            output = new double[size];

            //Перевірка на наявність розв'язків
            isExist(size, matrix);
            if (resultState == IsError.Undefined)
            {
                return;
            }

            for (int i = 0; i < size - 1; i++)
            {
                if (matrix[i, i] == 0)
                {
                    // Пошук ненульового елемента у стовпці для обміну рядків
                    for (int j = i + 1; j < size; j++)
                    {
                        if (matrix[j, i] != 0)
                        {
                            // Обмін рядків, якщо знайдено ненульовий елемент
                            for (int n = 0; n < size; n++)
                            {
                                double temp = matrix[i, n];
                                matrix[i, n] = matrix[j, n];
                                matrix[j, n] = temp;
                                temp = matrix[i, size];
                                matrix[i, size] = matrix[j, size];
                                matrix[j, size] = temp;
                                methodComplexity++;
                            }
                            methodComplexity++;
                            break; // Вихід з циклу після обміну рядків   
                        }
                        methodComplexity++;
                    }
                    methodComplexity++;
                }

                //Перевірка на скінченність розв'язків
                isInf(matrix[i, i]);
                if (resultState == IsError.Inf)
                {
                    return;
                }

                //Зведення матриці до трикутної форми
                double pivot = matrix[i, i];

                for (int j = i; j < size; j++)
                {
                    matrix[i, j] /= pivot;
                    methodComplexity++;
                }
                methodComplexity++;

                matrix[i, size] /= pivot;

                for (int j = i + 1; j < size; j++)
                {
                    double coefficient = matrix[j, i];
                    for (int k = i; k < size; k++)
                    {
                        matrix[j, k] -= coefficient * matrix[i, k];
                        methodComplexity++;
                    }
                    matrix[j, size] -= coefficient * matrix[i, size];
                    methodComplexity += 2;
                }
                methodComplexity += 2;
            }
            methodComplexity++;
            //запис проміжних результатів
            intermediatePhase(size, matrix);

            // Зводимо матрицю до одиничної форми
            for (int i = size - 1; i > 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    double coefficient = matrix[j, i];
                    for (int k = i; k < size; k++)
                    {
                        matrix[j, k] -= coefficient * matrix[i, k];
                        methodComplexity++;
                    }
                    matrix[j, size] -= coefficient * matrix[i, size];
                    methodComplexity += 2;
                }
                methodComplexity += 2;
            }
            methodComplexity++;

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
                if (matrix[i, i] == 0)
                {
                    // Пошук ненульового елемента у стовпці для обміну рядків
                    for (int j = i + 1; j < size - 1; j++)
                    {
                        if (matrix[j, i] != 0)
                        {
                            // Обмін рядків, якщо знайдено ненульовий елемент
                            for (int n = 0; n < size; n++)
                            {
                                double temp = matrix[i, n];
                                matrix[i, n] = matrix[j, n];
                                matrix[j, n] = temp;
                                temp = inverseMatrix[i, n];
                                inverseMatrix[i, n] = inverseMatrix[j, n];
                                inverseMatrix[j, n] = temp;
                                methodComplexity++;
                            }
                            methodComplexity++;
                            break; // Вихід з циклу після обміну рядків   
                        }
                        methodComplexity++;
                    }
                    methodComplexity++;
                }

                //Зведення матриці до трикутної форми
                double pivot = matrix[i, i];

                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] /= pivot;
                    inverseMatrix[i, j] /= pivot;
                    methodComplexity++;
                }
                methodComplexity++;

                for (int j = i + 1; j < size; j++)
                {
                    double coefficient = matrix[j, i];
                    for (int k = 0; k < size; k++)
                    {
                        matrix[j, k] -= coefficient * matrix[i, k];
                        inverseMatrix[j, k] -= coefficient * inverseMatrix[i, k];
                        methodComplexity++;
                    }
                    methodComplexity++;
                }
                methodComplexity += 2;
            }
            methodComplexity++;

            // Зводимо матрицю до одиничної форми
            for (int i = size - 1; i >= 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    double coefficient = matrix[j, i];
                    for (int k = 0; k < size; k++)
                    {
                        matrix[j, k] -= coefficient * matrix[i, k];
                        inverseMatrix[j, k] -= coefficient * inverseMatrix[i, k];
                        methodComplexity++;
                    }
                    methodComplexity += 2;
                }
                methodComplexity += 2;
            }
            methodComplexity++;

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
            output = new double[size];
            for (int i = 0; i < size; i++)
            {
                output[i] = 0.0;
                for (int j = 0; j < size; j++)
                {
                    output[i] += inverseMatrix[i, j] * matrix[j, size];
                    methodComplexity++;
                }
                methodComplexity += 2;
            }
            methodComplexity++;
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

        private void isInf(double elem)
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
