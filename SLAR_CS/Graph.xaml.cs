﻿using OxyPlot;
using OxyPlot.Series;
using System.IO;
using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Globalization;

namespace SLAR_CS
{
    public partial class Graph : Window
    {
        private const string SavingError = "Помилка збереження файла:";
        private const string Error = "Помилка";
        private const string Solution = "Розв'язок";
        private const string Save = "Зберегти до файла";
        private const string Graphic = "Графічне відображення";
        private const string First = "Перше рівняння";
        private const string Second = "Друге рівняння";
        private double[] results = new double[2];
        public Graph(double[] results)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.results = results;
        }

        //побудова графіка
        public void SetupPlot(double[,] cubicSystem)
        {
            //додавання елементів в вікно
            Button saveToFile = new Button();
            Grid.SetColumn(saveToFile, 1);
            Grid.SetRow(saveToFile, 1);

            saveToFile.Content = Save;
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

            //обчислення точок для побудови графіка першого рівняння
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

            var plotModel = new PlotModel { Title = Graphic };
            //встановлення властивостей лінії
            var lineSeries_1 = new LineSeries
            {
                Title = First,
                MarkerType = MarkerType.None
            };

            //додавання точок до об'єкта lineSeries_1
            lineSeries_1.Points.Add(new DataPoint(results[0], results[1]));

            lineSeries_1.Points.Add(new DataPoint(firstDotX, firstDotY));

            lineSeries_1.Points.Add(new DataPoint(secondDotX, secondDotY));

            //встановлення властивостей лінії
            var lineSeries_2 = new LineSeries
            {
                Title = Second,
                MarkerType = MarkerType.None
            };

            //обчислення точок для побудови графіка другого рівняння
            if (cubicSystem[1, 1] != 0)
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

            //додавання точок до об'єкта lineSeries_2
            lineSeries_2.Points.Add(new DataPoint(firstDotX, firstDotY));

            lineSeries_2.Points.Add(new DataPoint(secondDotX, secondDotY));

            plotModel.Series.Add(lineSeries_1);
            plotModel.Series.Add(lineSeries_2);

            plotView.Model = plotModel;
        }

        //функція для збереження у файл
        private void File_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";
            //призначення назви файлу
            string defaultFileName = $"Solution_{DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture)}.txt";
            saveFileDialog.FileName = defaultFileName;

            //вибір шляху збереження
            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    //запис в файл розв'язку
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine(Solution);

                        for (int i = 0; i < results.Length; i++)
                        {
                            writer.WriteLine($"X{i + 1}: {results[i].ToString()}");
                        }
                    }
                } 
                //повідомлення в разі помилки
                catch (Exception ex)
                {
                    MessageBox.Show($"{SavingError} {ex.Message}", Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
