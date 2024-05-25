using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SLAR_CS
{
    internal class SizeSelector
    {
        private const int Height = 25;
        private const int Width = 25;
        private const byte Red = 227;
        private const byte Green = 66;
        private const byte Blue = 66;
        private const byte Alpha = 128;
        private ComboBox Sizes;
        private Grid matrixGrid;
        private int size;
        public bool isCorrect;

        public int Size
        {
            get => size;
            set { size = value; }
        }

        public SizeSelector(ComboBox cb, Grid MatrixGrid)
        {
            Sizes = cb;
            matrixGrid = MatrixGrid;
        }

        public void Selector(ComboBox size)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)size.SelectedItem;
            Size = Convert.ToInt32(selectedItem.Content.ToString());
            Visualisation(Size, matrixGrid);
        }

        private void Visualisation(int size, Grid Matrix)
        {
            isCorrect = true;

            Matrix.Children.Clear();
            Sizes.Background = Brushes.Transparent;
            Sizes.ToolTip = null;
            int last = 0;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0, index = 0; j < size * 2; j += 2, index++)
                {
                    AddTextBox(i, j, index, Matrix);
                    AddTextBlock(i, j, index, Matrix);
                    last = j + 3;
                }

                AddTextBlockEq(i, last, Matrix);
                AddTextBoxB(i, last, Matrix);
            }
        }
        public void SetComboBoxBackground(string tip)
        {
            Color color = Color.FromArgb(Alpha, Red, Green, Blue);
            SolidColorBrush brush = new SolidColorBrush(color);
            Sizes.Background = brush;
            Sizes.ToolTip = tip;
            isCorrect = false;

        }

        private void AddTextBox(int i, int j, int index, Grid Matrix)
        {
            TextBox textBox = new TextBox();
            Grid.SetRow(textBox, i);
            Grid.SetColumn(textBox, j);

            textBox.Name = $"Index_{i}_{index}";
            SetTextBoxProperties(textBox);

            Matrix.Children.Add(textBox);
        }

        private void SetTextBoxProperties(TextBox textBox)
        {
            textBox.MinHeight = Height;
            textBox.MaxHeight = Height;
            textBox.MinWidth = Width;
            textBox.MaxWidth = Width;
            textBox.VerticalAlignment = VerticalAlignment.Center;
            textBox.HorizontalAlignment = HorizontalAlignment.Center;
        }
        private void AddTextBlock(int i, int j, int index, Grid Matrix)
        {
            TextBlock textBlock = new TextBlock();
            Grid.SetRow(textBlock, i);
            Grid.SetColumn(textBlock, j + 1);
            textBlock.Name = $"X1_{i}_{index}";
            if (index != size - 1)
            {
                textBlock.Text = $"X{index + 1}+";
            }
            else
            {
                textBlock.Text = $"X{index + 1}";
            }
            SetTextBlockProperties(textBlock);
            Matrix.Children.Add(textBlock);
        }

        private void SetTextBlockProperties(TextBlock textBlock)
        {
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
        }

        private void AddTextBlockEq(int i, int last, Grid Matrix)
        {
            TextBlock textBlock_eq = new TextBlock();
            Grid.SetRow(textBlock_eq, i);
            Grid.SetColumn(textBlock_eq, last - 1);
            textBlock_eq.Name = $"Equal_{i}";
            textBlock_eq.Text = "=";
            SetTextBlockProperties(textBlock_eq);
            Matrix.Children.Add(textBlock_eq);
        }

        private void AddTextBoxB(int i, int last, Grid Matrix)
        {
            TextBox textBox_B = new TextBox();
            Grid.SetRow(textBox_B, i);
            Grid.SetColumn(textBox_B, last);
            textBox_B.Name = $"B_{i}";
            SetTextBoxProperties(textBox_B);
            Matrix.Children.Add(textBox_B);
        }
    }
}
