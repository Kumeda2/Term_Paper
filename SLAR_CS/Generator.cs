using System;
using System.Windows;
using System.Windows.Controls;

namespace SLAR_CS
{
    internal class Generator
    {
        private const int Max = 10000;
        private const int Min = -10000;
        public Generator()
        {

        }

        //генерація СЛАР
        public void Generating(Grid MatrixField)
        {
            Random rand = new Random();
            foreach (UIElement element in MatrixField.Children)
            {
                if (element is TextBox textBox)
                {
                    double randomNumber = rand.NextDouble() * (Max - Min) + Min;
                    textBox.Text = $"{randomNumber.ToString("0.0")}";
                }
            }
        }

    }
}
