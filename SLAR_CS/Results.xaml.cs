﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SLAR_CS
{
    public partial class Results : Window
    {
        private const string MatrixMethod = "Матричний метод";
        private const string SavingError = "Помилка збереження файла:";
        private const string Error = "Помилка";
        private double[] results;
        private double[,] system;
        private string method;
        private Graph graph;

        public Results(double[] results, double[,] halfResult, string method, double[,] system)
        {
            this.results = results;
            this.method = method;
            this.system = system;
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public void AddingAnswer(double[] result)
        {
            for (int i = 0; i < result.Length; i++)
            {
                TextBox x = new TextBox();
                Grid.SetColumn(x, 2);
                Grid.SetRow(x, i);

                x.Text = result[i].ToString();
                x.VerticalAlignment = VerticalAlignment.Center;
                x.HorizontalAlignment = HorizontalAlignment.Left;
                x.Width = 30;
                x.Height = 30;

                X.Children.Add(x);
            }
        }

        public void AddingX(int size)
        {
            for (int i = 0; i < size; i++)
            {
                TextBlock x = new TextBlock();
                Grid.SetColumn(x, 0);
                Grid.SetRow(x, i);

                x.Text = $"X{i + 1}";
                x.VerticalAlignment = VerticalAlignment.Bottom;
                x.HorizontalAlignment = HorizontalAlignment.Left;
                x.Width = 30;
                x.Height = 30;

                X.Children.Add(x);
            }
        }

        public void AddingStacpanel(int complexity, double[,] halfResults, int size)
        {
            TextBlock text = new TextBlock();
            Grid.SetColumn(text, 0);
            Grid.SetRow(text, 0);
            Grid.SetColumnSpan(text, 3);

            text.VerticalAlignment = VerticalAlignment.Center;
            text.HorizontalAlignment = HorizontalAlignment.Left;
            text.Text = "Проміжні результати";
            text.Margin = new Thickness(12, 0, 0, 0);

            HalfResult.Children.Add(text);


            Button file = new Button();
            Grid.SetRow(file, 7);
            Grid.SetColumn(file, 0);
            Grid.SetColumnSpan(file, 3);

            file.Content = "Зберегти до файла";
            file.Height = 30;
            file.Width = 140;
            file.FontSize = 14;
            file.Padding = new Thickness(0);
            file.VerticalAlignment = VerticalAlignment.Bottom;
            file.HorizontalAlignment = HorizontalAlignment.Left;
            file.Click += File_Click;
            X.Children.Add(file);

            TextBlock algorithm = new TextBlock();
            Grid.SetRow(algorithm, 7);
            Grid.SetColumn(algorithm, 3);

            algorithm.Text = "Кількість операцій алгоритму: " + complexity.ToString();
            algorithm.FontSize = 12;
            algorithm.FontWeight = FontWeights.Bold;
            algorithm.VerticalAlignment = VerticalAlignment.Bottom;
            algorithm.HorizontalAlignment = HorizontalAlignment.Left;
            X.Children.Add(algorithm);

            if (size == 2)
            {
                Button graph = new Button();
                Grid.SetColumn(graph, 4);
                Grid.SetRow(graph, 7);

                graph.Content = "Графічно";
                graph.Height = 30;
                graph.Width = 80;
                graph.FontSize = 14;
                graph.Padding = new Thickness(0);
                graph.VerticalAlignment = VerticalAlignment.Bottom;
                graph.HorizontalAlignment = HorizontalAlignment.Right;
                graph.Click += Graph_Click;

                X.Children.Add(graph);
            }

            if (method == MatrixMethod)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        AddingHalfResults(i, j, halfResults);
                    }
                }
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size + 1; j++)
                    {
                        AddingHalfResults(i, j, halfResults);
                    }
                }
            }
        }

        private void AddingHalfResults(int i, int j, double[,] _halfResults)
        {
            TextBox elem = new TextBox();
            Grid.SetRow(elem, i + 1);
            Grid.SetColumn(elem, j);

            elem.Text = _halfResults[i, j].ToString();
            elem.Width = 30;
            elem.VerticalAlignment = VerticalAlignment.Center;
            elem.HorizontalAlignment = HorizontalAlignment.Center;

            HalfResult.Children.Add(elem);
        }
        public void AddingEq(int size)
        {
            for (int i = 0; i < size; i++)
            {
                TextBlock x = new TextBlock();
                Grid.SetColumn(x, 1);
                Grid.SetRow(x, i);

                x.Text = "=";
                x.VerticalAlignment = VerticalAlignment.Bottom;
                x.HorizontalAlignment = HorizontalAlignment.Center;
                x.Width = 30;
                x.Height = 30;

                X.Children.Add(x);
            }
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

        private void Graph_Click(object sender, RoutedEventArgs e)
        {
            if (graph != null)
            {
                graph.Close();
            }
            graph = new Graph(results);
            graph.SetupPlot(system);
            graph.Show();
        }
    }
}