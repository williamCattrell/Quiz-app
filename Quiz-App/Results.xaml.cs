using Microsoft.VisualBasic;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using Microsoft.Win32;
using System.IO;
using System.Text;

namespace PIIIProject
{
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : Window
    {
        private string _saveFile;

        public Results()
        {
            InitializeComponent();

            lvResults.ItemsSource = Question.QuestionList;
            lbFinalScore.Content = CalculateScore().ToString() + "/" + Question.QuestionList.Count;
        }

        //tabulates the score from the quiz by comapring the chosen answer with the correct one
        private int CalculateScore()
        {
            int score = 0;
            foreach (Question question in Question.QuestionList)
            {
                if (question.SelectedAnswer == question.CorrectAnswer)
                    score++;
            }
            return score;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            //kills the app
            App.Current.MainWindow.Close();
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // No save file location is set
            if (string.IsNullOrEmpty(_saveFile))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV Files|*.csv | txt Files|*.txt";
                if (saveFileDialog.ShowDialog() == true)
                    _saveFile = saveFileDialog.FileName;
                else
                    return;
            }
            SaveFile();
            // Disable save button
            btnSave.IsHitTestVisible = false;
        }

        private void SaveFile()
        {
            // Add header and append ToString content of each question object in static list
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Question,YourAnswer,CorrectAnswer,Correct?");
                foreach (Question question in Question.QuestionList)
                {
                    sb.AppendLine(question.ToString());
                }

                File.WriteAllText(_saveFile, sb.ToString());
            }
            catch (Exception)
            {
                MessageBox.Show("Error while writing answer data to file", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("File Saved", "Saving File", MessageBoxButton.OK);
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            //kills the current window and calls the reset method created in the MainWindow instance
            this.Close();
            App.Current.MainWindow.Show();
            ((MainWindow)Application.Current.MainWindow).Reset();
        }
    }
}