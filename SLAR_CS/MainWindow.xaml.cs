using System;
using System.Windows;
using System.Windows.Controls;

namespace SLAR_CS
{

    public partial class MainWindow : Window
    {
        private SizeSelector size;
        private Method method;
        private Matrix matrix;
        private OptionGenerator generation;
        private Results resultsWindow;
        private const string SizeNotSelected = "Оберіть розмір системи";
        private const string IncorrectSize = "Для графічного розв'язку розмір має дорівнювати 2";
        private const string IncorrectSizeMassege = "Некоректний розмір системи";
        private const string ProgramName = "Калькулятор СЛАР";
        private const string MethodNotSeleted = "Не обрано метод";
        private const string Gauss = "Метод Гауса";
        private const string JordanGauss = "Метод Жордана-Гауса";
        private const string MatrixMethod = "Матричний метод";
        private const string DeterminantZero = "Визначник доруівнює нулю, введіть інші коефіцієнти";
        private const string ParametersNotSet = "Встановіть параметри СЛАР";
        private const string Undefined = "Система не має розв'язоків";
        private const string Inf = "Система має безліч розв'язків";
        private double[,] copyOfMatrix;


        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Method_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox methodComboBox = (ComboBox)sender;
            method = new Method();
            method.SelectionAndValidation(methodComboBox, Size);
        }

        public void Size_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox sizeComboBox = (ComboBox)sender;
            string arg;
            size = new SizeSelector(sizeComboBox, Matrix, Method, arg = method != null? method.SelectedMethod : null);
            size.Selector(sizeComboBox);
        }

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

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            if (size == null && method == null)
            {
                MessageBox.Show(ParametersNotSet, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (size == null || size.Size == 0)
            {
                size = new SizeSelector(Size, Matrix, Method, method.SelectedMethod);
                size.Size = 0;
                size.SetComboBoxBackground(SizeNotSelected);
                MessageBox.Show(SizeNotSelected, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (method == null || method.SelectedMethod == "")
            {
                method = new Method();
                method.SelectedMethod = "";
                method.MethodBackground(Method);
                MessageBox.Show(MethodNotSeleted, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                matrix = new Matrix(size.Size);
                matrix.FillingMatrix(size.Size, Matrix);
                copyOfMatrix = new double[size.Size, size.Size + 1];
                for(int i = 0; i < size.Size; i++)
                {
                    for(int j = 0; j < size.Size + 1; j++)
                    {
                        copyOfMatrix[i, j] = matrix.matrix[i, j];
                    }
                }    
            }

            if (matrix.result == Result.Success)
            {
                if (method.SelectedMethod == Gauss)
                {
                    method.GaussMethod(copyOfMatrix, size.Size);
                    if (method.resultState == IsError.Undefined)
                    {
                        MessageBox.Show(Undefined, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else if(method.resultState == IsError.Inf)
                    {
                        MessageBox.Show(Inf, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else if (method.SelectedMethod == JordanGauss)
                {
                    method.JordanGaussMethod(copyOfMatrix, size.Size);
                    if (method.resultState == IsError.Undefined)
                    {
                        MessageBox.Show(Undefined, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else if (method.resultState == IsError.Inf)
                    {
                        MessageBox.Show(Inf, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else if (method.SelectedMethod == MatrixMethod)
                {
                    if (matrix.Determinant(size.Size) == 0 && matrix.result != Result.InvalidInput)
                    {
                        MessageBox.Show(DeterminantZero, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    method.MatrixMethod(copyOfMatrix, size.Size);
                }
                
                if (resultsWindow != null)
                {
                    resultsWindow.Close();
                }

                resultsWindow = new Results(method.output, method.intermediateMatrix, method.SelectedMethod, matrix.matrix);
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

        private void Generation_Click(object sender, RoutedEventArgs e)
        {
            if (size == null && method == null)
            {
                MessageBox.Show(ParametersNotSet, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (size == null)
            {
                size = new SizeSelector(Size, Matrix, Method, method.SelectedMethod);
                size.SetComboBoxBackground(SizeNotSelected);
                MessageBox.Show(SizeNotSelected, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(size.isCorrect ==  false)
            {
                size.SetComboBoxBackground(SizeNotSelected);
                MessageBox.Show(IncorrectSizeMassege, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            generation = new OptionGenerator();
            generation.Generating(Matrix);
        }
    }
}
