﻿using OxyPlot;
using OxyPlot.Series;
using System.IO;
using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace SLAR_CS
{
    public partial class Graph : Window
    {
        private const string SavingError = "Помилка збереження файла:";
        private const string Error = "Помилка";
        private double[] results = new double[2];
        public Graph(double[] results)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.results = results;
        }

        public void SetupPlot(double[,] cubicSystem)
        {
            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine(results[i]);
            }

            Button saveToFile = new Button();
            Grid.SetColumn(saveToFile, 1);
            Grid.SetRow(saveToFile, 1);

            saveToFile.Content = "Зберегти до файла";
            saveToFile.Height = 30;
            saveToFile.Width = 140;
            saveToFile.FontSize = 14;
            saveToFile.Padding = new Thickness(0);
            saveToFile.Background = System.Windows.Media.Brushes.White;
            saveToFile.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(103, 58, 183));


            saveToFile.Click += File_Click;

            GraphVisualization.Children.Add(saveToFile);

            TextBlock result_1 = new TextBlock();
            result_1.Text = results[0].ToString();
            result_1.FontSize = 18;
            result_1.Foreground = System.Windows.Media.Brushes.White;


            X1.Children.Add(result_1);

            TextBlock result_2 = new TextBlock();
            result_2.Text = results[1].ToString();
            result_2.FontSize = 18;
            result_2.Foreground = System.Windows.Media.Brushes.White;

            X2.Children.Add(result_2);

            double firstDotX;
            double firstDotY;
            double secondDotX;
            double secondDotY;

            if (cubicSystem[0, 1] != 0)
            {
                firstDotX = results[0] + 2;
                firstDotY = (cubicSystem[0, 2] - cubicSystem[0, 0] * firstDotX) / cubicSystem[0, 1];

                secondDotX = results[0] - 2;
                secondDotY = (cubicSystem[0, 2] - cubicSystem[0, 0] * secondDotX) / cubicSystem[0, 1];
            }
            else
            {
                firstDotY = results[1] + 2;
                firstDotX = (cubicSystem[0, 2] - cubicSystem[0, 1] * firstDotY) / cubicSystem[0, 0];

                secondDotY = results[1] - 2;
                secondDotX = (cubicSystem[0, 2] - cubicSystem[0, 1] * secondDotY) / cubicSystem[0, 0];
            }

            var plotModel = new PlotModel { Title = "Графічний метод" };

            var lineSeries_1 = new LineSeries
            {
                Title = "Line Series",
                MarkerType = MarkerType.None
            };

            lineSeries_1.Points.Add(new DataPoint(results[0], results[1]));

            lineSeries_1.Points.Add(new DataPoint(firstDotX, firstDotY));

            lineSeries_1.Points.Add(new DataPoint(secondDotX, secondDotY));

            var lineSeries_2 = new LineSeries
            {
                Title = "Line Series",
                MarkerType = MarkerType.None
            };

            if (cubicSystem[0, 1] != 0)
            {
                firstDotX = results[0] + 2;
                firstDotY = (cubicSystem[1, 2] - cubicSystem[1, 0] * firstDotX) / cubicSystem[1, 1];

                secondDotX = results[0] - 2;
                secondDotY = (cubicSystem[1, 2] - cubicSystem[1, 0] * secondDotX) / cubicSystem[1, 1];
            }
            else
            {
                firstDotY = results[1] + 2;
                firstDotX = (cubicSystem[1, 2] - cubicSystem[1, 1] * firstDotY) / cubicSystem[1, 0];

                secondDotY = results[1] - 2;
                secondDotX = (cubicSystem[1, 2] - cubicSystem[1, 1] * secondDotY) / cubicSystem[1, 0];
            }

            lineSeries_2.Points.Add(new DataPoint(firstDotX, firstDotY));

            lineSeries_2.Points.Add(new DataPoint(secondDotX, secondDotY));

            plotModel.Series.Add(lineSeries_1);
            plotModel.Series.Add(lineSeries_2);

            plotView.Model = plotModel;
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            // Створюємо вікно вибору файлу
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            // Показуємо вікно вибору файлу і чекаємо на результат
            bool? result = saveFileDialog.ShowDialog();

            // Якщо користувач вибрав файл і натиснув "Зберегти"
            if (result == true)
            {
                // Отримуємо шлях до вибраного файла
                string filePath = saveFileDialog.FileName;
                try
                {
                    // Відкриваємо файл для запису
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        // Записуємо заголовок
                        writer.WriteLine(Title);

                        // Записуємо розв'язки
                        for (int i = 0; i < results.Length; i++)
                        {
                            writer.WriteLine($"X{i + 1}: {results[i].ToString()}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{SavingError} {ex.Message}", Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}