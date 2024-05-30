using System.Windows;
using System.Windows.Controls;

namespace SLAR_CS
{
    public partial class MainWindow : Window
    {
        private SizeSelector size;
        private Method method;
        private Matrix matrix;
        private Generator generation;
        private Results resultsWindow;
        private const string SizeNotSelected = "Оберіть розмір системи";
        private const string ProgramName = "Калькулятор СЛАР";
        private const string MethodNotSeleted = "Не обрано метод";
        private const string Gauss = "Метод Гауса";
        private const string JordanGauss = "Метод Жордана-Гауса";
        private const string MatrixMethod = "Матричний метод";
        private const string DeterminantZero = "Визначник доруівнює нулю, введіть інші коефіцієнти";
        private const string ParametersNotSet = "Встановіть параметри СЛАР";
        private const string Undefined = "Система не має розв'язків";
        private const string Inf = "Система має безліч розв'язків";
        private double[,] copyOfMatrix;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        //обробка події вибору методу
        private void Method_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox methodComboBox = (ComboBox)sender;
            method = new Method();
            method.SelectionAndValidation(methodComboBox);
        }

        //обробка події вибору розміру
        private void Size_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox sizeComboBox = (ComboBox)sender;
            size = new SizeSelector(sizeComboBox, Matrix);
            size.Selector(sizeComboBox);
        }

        //обробка події натискання на кнопку очищення системи
        private void Erase_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement element in Matrix.Children)
            {
                if (element is TextBox textBox)
                {
                    textBox.Text = "";
                    textBox.Background = null;
                    textBox.ToolTip = null;
                }
            }
            Generation.Visibility = Visibility.Collapsed;
        }

        //обробка події натискання на кнопку обчислення СЛАР
        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            //перевірка чи всі параметри обрано
            if (size == null && method == null)
            {
                MessageBox.Show(ParametersNotSet, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);//виведення помилки
                return;
            }
            //перевірка чи обрано розмір
            else if (size == null || size.Size == 0)
            {
                size = new SizeSelector(Size, Matrix);
                size.Size = 0;
                size.SetComboBoxBackground(SizeNotSelected);//зміна кольору комбобокса для наглядності
                MessageBox.Show(SizeNotSelected, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);//виведення помилки
                return;
            }
            //перевірка чи обрано метод
            else if (method == null || method.selectedMethod == "")
            {
                method = new Method();
                method.selectedMethod = "";
                method.MethodBackground(Method);//зміна кольору комбобокса для наглядності
                MessageBox.Show(MethodNotSeleted, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);//виведення помилки
                return;
            }
            //створення матриці та її копії
            else
            {
                matrix = new Matrix(size.Size);
                matrix.FillingMatrix(size.Size, Matrix);//заповнення розширеної матриці коефіцієнтами та вільними членами
                copyOfMatrix = new double[size.Size, size.Size + 1];//копія розширеної матриці 
                for (int i = 0; i < size.Size; i++)
                {
                    for (int j = 0; j < size.Size + 1; j++)
                    {
                        copyOfMatrix[i, j] = matrix.matrix[i, j];
                    }
                }
            }
            //перевірка успішності заповнення матриці
            if ((MatrixState.State)matrix.matrixState.GetState() == MatrixState.State.Success)
            {
                //виконання методу Гаусса
                if (method.selectedMethod == Gauss)
                {
                    method.GaussMethod(copyOfMatrix, size.Size);
                }
                else if (method.selectedMethod == JordanGauss)
                {
                    //виконання методу Жодана-Гаусса
                    method.JordanGaussMethod(copyOfMatrix, size.Size);
                }
                //виконання матричного методу 
                else if (method.selectedMethod == MatrixMethod)
                {
                    //перевірка на нульовний визначник
                    if (matrix.Determinant(size.Size) == 0)
                    {
                        MessageBox.Show(DeterminantZero, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    method.MatrixMethod(copyOfMatrix, size.Size);
                }

                var state = (MethodResultState.State)method.methodResultState.GetState();
                
                if (ValidResult(state))
                {
                    //закриття вікна з розв'язком, якщо воно вже існує
                    if (resultsWindow != null)
                    {
                        if (resultsWindow.graph != null)
                        {
                            resultsWindow.graph.Close();
                        }
                        resultsWindow.Close();
                    }

                    //виведення результатів
                    resultsWindow = new Results(method.output, method.selectedMethod, matrix.matrix);
                    resultsWindow.Show();
                    resultsWindow.AddingAnswer(method.output);
                    resultsWindow.AddingX(method.output.Length);
                    resultsWindow.AddingEq(method.output.Length);
                    resultsWindow.AddingStacpanel(method.methodComplexity, method.intermediateMatrix, method.output.Length);
                    resultsWindow.Closed += (s, args) =>
                    {
                        resultsWindow = null;
                    };
                }
            }
        }
        //перевірка упішності роботи методу
        private bool ValidResult(MethodResultState.State state)
        {
            switch (state)
            {
                case MethodResultState.State.Undefined:
                    MessageBox.Show(Undefined, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                case MethodResultState.State.Inf:
                    MessageBox.Show(Inf, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                // Add more cases if needed
                default:
                    // Handle other states if necessary
                    return true;
            }
        }

        //обробка події натискання на кнопку генерації
        private void Generation_Click(object sender, RoutedEventArgs e)
        {
            //перевірка вибору параметрів СЛАР
            if (size == null && method == null)
            {
                MessageBox.Show(ParametersNotSet, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);//виведення помилки
                return;
            }
            //перевірка вибору розміру
            else if (size == null)
            {
                size = new SizeSelector(Size, Matrix);
                size.SetComboBoxBackground(SizeNotSelected);//зміна кольору комбобокса для наглядності
                MessageBox.Show(SizeNotSelected, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);//виведення помилки
                return;
            }
            if (size.isCorrect == false)
            {
                size.SetComboBoxBackground(SizeNotSelected);//зміна кольору комбобокса для наглядності
                MessageBox.Show(SizeNotSelected, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);//виведення помилки
                return;
            }
            generation = new Generator();
            generation.Generating(Matrix);//генерація СЛАР
        }
    }
}