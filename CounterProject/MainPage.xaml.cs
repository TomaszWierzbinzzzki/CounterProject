using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Maui.Controls;

namespace CounterProject
{
    public partial class MainPage : ContentPage
    {
        private int counterId = 0;
        private List<int> counterValue = new List<int>();

        public MainPage()
        {
            InitializeComponent();
            loadCounterValues();
        }

        private void buttonClearData(object sender, EventArgs e)
        {
            var filePath = getFilePath();

            if (File.Exists(filePath))
            {
                File.WriteAllText(filePath, string.Empty);
            }

            counterValue.Clear();
            countersStack.Children.Clear();
            errorMessageLabel.IsVisible = false;
            inputEntry.Text = "0";
            counterId = 0;
        }

        private void buttonAddCounter(object sender, EventArgs e)
        {
            errorMessageLabel.IsVisible = false;

            if (int.TryParse(inputEntry.Text, out int inputValue))
            {
                counterValue.Add(inputValue);

                var counterLayout = CreateCounterLayout(counterId++, inputValue);
                countersStack.Children.Add(counterLayout);

                inputEntry.Text = "0";
                saveCounterValues();
            }
            else
            {
                errorMessageLabel.Text = "Proszę wprowadzić prawidłową liczbę.";
                errorMessageLabel.IsVisible = true;
            }
        }

        private void onInputFocus(object sender, FocusEventArgs e)
        {
            inputEntry.Text = string.Empty;
        }

        private string getFilePath()
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(folderPath, "counters.json");
        }

        private void saveCounterValues()
        {
            var json = JsonSerializer.Serialize(counterValue);
            var filePath = getFilePath();

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(json);
                }
            }
        }

        private void loadCounterValues()
        {
            var filePath = getFilePath();

            if (File.Exists(filePath))
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var json = reader.ReadToEnd();
                        counterValue = JsonSerializer.Deserialize<List<int>>(json);

                        countersStack.Children.Clear();
                        counterId = 0;

                        foreach (var value in counterValue)
                        {
                            var counterLayout = CreateCounterLayout(counterId++, value);
                            countersStack.Children.Add(counterLayout);
                        }
                    }
                }
            }
        }

        private StackLayout CreateCounterLayout(int index, int initialValue)
        {
            var counterLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 4
            };

            var labelCounterName = new Label
            {
                Text = $"Licznik {index + 1}",
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };

            var labelValue = new Label
            {
                Text = initialValue.ToString(),
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center
            };

            var buttonSubtract = new Button
            {
                Text = "-",
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            buttonSubtract.Clicked += (s, e) => onClickSubtract(index, labelValue);

            var buttonAdd = new Button
            {
                Text = "+",
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };
            buttonAdd.Clicked += (s, e) => onClickAdd(index, labelValue);

            counterLayout.Children.Add(labelCounterName);
            counterLayout.Children.Add(labelValue);
            counterLayout.Children.Add(buttonSubtract);
            counterLayout.Children.Add(buttonAdd);
                  
            
            return counterLayout;
        }


        private void onClickAdd(int index, Label labelValue)
        {
            counterValue[index]++;
            labelValue.Text = $"{counterValue[index]}";
            saveCounterValues();
        }

        private void onClickSubtract(int index, Label labelValue)
        {
            counterValue[index]--;
            labelValue.Text = $"{counterValue[index]}";
            saveCounterValues();
        }
    }
}