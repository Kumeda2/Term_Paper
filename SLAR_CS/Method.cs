using System.Windows.Controls;
using System.Windows.Media;

namespace SLAR_CS
{
    internal class Method
    {
        private const byte Red = 227;
        private const byte Green = 66;
        private const byte Blue = 66;
        private const byte Alpha = 128;
        private const string MethodNotSelected = "Оберіть метод";
        public string selectedMethod;
        public double[] output;
        public int methodComplexity;
        public double[,] intermediateMatrix;
        public MethodResultState methodResultState;

        public Method()
        {
            methodResultState = new MethodResultState();
        }

        //збереження обраного методу 
        public void SelectionAndValidation(ComboBox method)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)method.SelectedItem;
            selectedMethod = selectedItem.Content.ToString();
            method.Background = Brushes.Transparent;
            method.ToolTip = null;
        }
        //функція для змінми вигляду комбобоксу
        public void MethodBackground(ComboBox method)
        {
            Color color = Color.FromArgb(Alpha, Red, Green, Blue);
            SolidColorBrush brush = new SolidColorBrush(color);
            method.Background = brush;
            method.ToolTip = MethodNotSelected;
        }
        //реалізація методу Гаусса
        public void GaussMethod(double[,] matrix, int size)
        {
            methodComplexity = 0;
            methodResultState.SetState(MethodResultState.State.Success);
            output = new double[size];

            // заміна опорних діагональних елементів, які дорівнюють 0
            for (int i = 0; i < size; i++)
            {
                methodComplexity++;
                methodComplexity += RemoveZero(size, i, matrix);

                //Перевірка на скінченність та існування розв'язків
                isExist(size, matrix);
                if ((MethodResultState.State)methodResultState.GetState() == MethodResultState.State.Undefined)
                {
                    return;
                }
                isInf(matrix[i, i]);
                if ((MethodResultState.State)methodResultState.GetState() == MethodResultState.State.Inf)
                {
                    return;
                }


                // Прямий хід методу Гаусса(зведення до трикутного вигляду)
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

            // Зворотній хід методу Гаусса(обчислення розв'язку)
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
        //реалізація методу Жордана-Гаусса
        public void JordanGaussMethod(double[,] matrix, int size)
        {
            methodComplexity = 0;
            methodResultState.SetState(MethodResultState.State.Success);
            output = new double[size];

            for (int i = 0; i < size; i++)
            {
                methodComplexity++;
                methodComplexity += RemoveZero(size, i, matrix);

                //Перевірка на скінченність розв'язків
                isExist(size, matrix);
                if ((MethodResultState.State)methodResultState.GetState() == MethodResultState.State.Undefined)
                {
                    return;
                }
                isInf(matrix[i, i]);
                if ((MethodResultState.State)methodResultState.GetState() == MethodResultState.State.Inf)
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
            //запис розв'язку
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
            // пошук і заміна нульового діагонального елемента
            for (int i = 0; i < size; i++)
            {
                methodComplexity++;
                if (matrix[i, i] == 0)
                {
                    for (int j = i + 1; j < size; j++)
                    {
                        methodComplexity++;
                        if (matrix[j, i] != 0)
                        {
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
                            break;
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
            for (int i = size - 1; i > 0; i--)
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
        //функція для заміни нульових елементів діагоналі
        private int RemoveZero(int size, int i, double[,] matrix)
        {
            int counter = 0;
            if (matrix[i, i] == 0)
            {
                for (int j = i + 1; j < size; j++)
                {
                    counter++;
                    if (matrix[j, i] != 0)
                    {
                        double temp;
                        for (int n = 0; n < size; n++)
                        {
                            counter++;
                            temp = matrix[i, n];
                            matrix[i, n] = matrix[j, n];
                            matrix[j, n] = temp;
                        }
                        temp = matrix[i, size];
                        matrix[i, size] = matrix[j, size];
                        matrix[j, size] = temp;
                        break;
                    }
                }
            }
            return counter;
        }
        //функція, що перевіряє існування розв'язків
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
                    methodResultState.SetState(MethodResultState.State.Undefined);
                    return;
                }
            }
        }
        //функція, що перевіряє нескінченність розв'язків
        private void isInf(double elem)
        {
            if (elem == 0)//перевірка, чи опорний елемент дорівнює нулю
            {
                methodResultState.SetState(MethodResultState.State.Inf);
                return;
            }
        }
        //функція, що зберігає проміжні результати
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
