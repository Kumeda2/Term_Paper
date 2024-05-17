﻿using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SLAR_CS
{
    internal class Matrix
    {
        private const byte Red = 227;
        private const byte Green = 66;
        private const byte Blue = 66;
        private const byte Alpha = 128;
        private const double Max = 10000.0;
        private const double Min = -Max;
        private const string ProgramName = "Калькулятор СЛАР";
        private const string IncvalidInput = "Введено некоректні символи";
        private const string OutOfLimits = "Введене число, що не входить в межі";
        private const string UserInputLimit = "Введіть число в межах (-10000; 10000)";
        private const string UserInput = "Введіть число";
        private const string CloseToZero = "Введений коефіцієнт занадто близький до нуля, введіть або 0 або зменшіть кількість знаків після коми";

        private int size;

        public double[,] matrix;
        public Result result;

        public Matrix(int _size)
        {
            size = _size;
            matrix = new double[size, size + 1];
        }

        public void FillingMatrix(int size, Grid MatrixGrid)
        {
            
            foreach (UIElement element in MatrixGrid.Children)
            {
                if (!(element is TextBox textBox)) continue;

                SetTextBoxProperties(textBox);

                int i, j;

                //перевіряємо, чи це коефіцієнт, чи вільний член
                string[] nameParts = textBox.Name.Split('_');
                (i, j) = nameParts.Length > 2 ?
                    (int.Parse(nameParts[1]), int.Parse(nameParts[2])) :
                    (int.Parse(nameParts[1]), size);

                textBox.Text = textBox.Text.Replace('.', ',');
                if (double.TryParse(textBox.Text, out double value))
                {
                    if (value > Max || value < Min)
                    {
                        SetTextBoxProperties(textBox, Color.FromArgb(Alpha, Red, Green, Blue), UserInputLimit);
                        MessageBox.Show(OutOfLimits, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                        result = Result.InvalidInput;
                        break;
                    }
                    //валідація експоненти
                    if (textBox.Text.Contains('e') || textBox.Text.Contains('E'))
                    {
                        string[] expParts = textBox.Text.Split('e', 'E');
                        if (double.TryParse(expParts[1], out double power))
                        {
                            if (Math.Abs(power) > 3)
                            {
                                SetTextBoxProperties(textBox, Color.FromArgb(Alpha, Red, Green, Blue), UserInputLimit);
                                MessageBox.Show(CloseToZero, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                                result = Result.InvalidInput;
                                break;
                            }
                        }
                    }

                    int counter = 0;
                    if (textBox.Text.Contains(','))
                    {
                        string[] afterDot = textBox.Text.Split(',');
                        foreach(char c in afterDot[1])
                        {
                            if (c == '0')
                            {
                                counter++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (counter > 3)
                    {
                        SetTextBoxProperties(textBox, Color.FromArgb(Alpha, Red, Green, Blue), UserInputLimit);
                        MessageBox.Show(CloseToZero, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                        result = Result.InvalidInput;
                        break;
                    }

                    matrix[i, j] = value;
                }
                else if (string.IsNullOrEmpty(textBox.Text))
                {
                    matrix[i, j] = 0;
                }
                else
                {
                    SetTextBoxProperties(textBox, Color.FromArgb(Alpha, Red, Green, Blue), UserInput);
                    MessageBox.Show(IncvalidInput, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                    result = Result.InvalidInput;
                    break;
                }
                result = Result.Success;
            }
            if (result == Result.Success)
                FillingWithZeroes(MatrixGrid);
        }

        public static void SetTextBoxProperties(TextBox textBox, Color? backgroundColor = null, string toolTip = null)
        {
            textBox.Background = new SolidColorBrush(backgroundColor ?? Colors.Transparent);
            textBox.ToolTip = toolTip;
        }

        private void FillingWithZeroes(Grid MatrixGrid)
        {
            foreach (UIElement element in MatrixGrid.Children)
            {
                if (element is TextBox textBox)
                {
                    if (textBox.Text == "")
                    {
                        textBox.Text = "0";
                    }
                }
            }
        }

        public double Determinant(int size) 
        {
            double[,] copyOfMatrix = new double[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    copyOfMatrix[i, j] = matrix[i, j];
                }
            }

            double determinant = 1.0;

            for (int i = 0; i < size; i++)
            {
                int maxRow = i;
                double maxVal = Math.Abs(copyOfMatrix[i, i]);

                for (int j = i + 1; j < size; j++)
                {
                    if (Math.Abs(copyOfMatrix[j, i]) > maxVal)
                    {
                        maxVal = Math.Abs(copyOfMatrix[j, i]);
                        maxRow = j;
                    }
                }

                for (int n = 0; n < size; n++)
                {
                    double temp = copyOfMatrix[i, n];
                    copyOfMatrix[i, n] = copyOfMatrix[maxRow, n];
                    copyOfMatrix[maxRow, n] = temp;
                }

                if (copyOfMatrix[i, i] == 0)
                    return 0;

                for (int j = i + 1; j < size; j++)
                {
                    double multiplier = copyOfMatrix[j, i] / copyOfMatrix[i, i];
                    for (int k = i; k < size; k++)
                    {
                        copyOfMatrix[j, k] -= multiplier * copyOfMatrix[i, k];
                    }
                }
            }

            for (int i = 0; i < size; i++)
            {
                determinant *= copyOfMatrix[i, i];
            }

            return determinant;
        }
    }

    internal enum Result
    {
        Success,
        InvalidInput,
        InvalidParameters
    }
}