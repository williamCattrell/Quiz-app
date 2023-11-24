using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PIIIProject
{
    //William Cattrell 1855666
    //Griffin Bonomo-Clough 2161736

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //backing fields
        private int _questionCounter = 1, _score = 0;

        private int _SCOREMAX;
        private string _scoreOver;
        private List<Button> answerButtons = new List<Button>();
        private List<Question> questions;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AnswerCheck(object sender, RoutedEventArgs e)
        {
            var selectedButton = sender as Button;
            // Index of selected answer
            int selectedAnswerIndex = int.Parse(selectedButton.Tag.ToString()) - 1;
            // Value of selected answer
            string selectedAnswer = questions[_questionCounter - 1].Answers[selectedAnswerIndex];

            questions[_questionCounter - 1].SelectedAnswer = selectedAnswer;

            // Loop through all buttons and change their appearance according to whether the selected answer is right or wrong
            foreach (Button button in answerButtons)
            {
                // Extract the value from the button
                TextBlock textBlock = (TextBlock)button.Content;
                string buttonContent = textBlock.Text.ToString();

                // Correct answer was the selected answer
                if (questions[_questionCounter - 1].IsCorrectAnswer)
                {
                    // Selected button is current button in loop
                    if (selectedButton == button)
                    {
                        button.Background = Brushes.LightGreen;
                        lbHeader.Foreground = Brushes.Lime;
                        lbHeader.Content = "CORRECT";
                        _score++;
                        lbScore.Content = _score + _scoreOver;
                        button.IsHitTestVisible = false;
                        continue;
                    }
                }
                //assigning the right answer the color green and marking incorrect if user selected wrong answer
                else if (questions[_questionCounter - 1].CorrectAnswer == buttonContent)
                {
                    button.Background = Brushes.LightGreen;
                    lbHeader.Foreground = Brushes.Red;
                    lbHeader.Content = "INCORRECT";
                    button.IsHitTestVisible = false;
                    continue;
                }
                // Correct answer was not the selected answer
                else
                {
                    lbHeader.Content = "INCORRECT";
                    lbHeader.Foreground = Brushes.Red;
                }
                button.Background = Brushes.Red;
                button.IsHitTestVisible = false;
            }
            // Player is on the final question
            if (_questionCounter == _SCOREMAX)
            {
                btnNextQuestion.Visibility = Visibility.Collapsed;
                btnNextQuestion.IsHitTestVisible = false;
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            //ensures the laoding did not fail
            if (LoadQuestionFile())
            {
                //hides all non question items and brings the pertinent items back into view
                btnLoad.Visibility = Visibility.Collapsed;
                btnExit.Visibility = Visibility.Collapsed;
                lbLoadText.Visibility = Visibility.Collapsed;
                btnAnswer1.Visibility = Visibility.Visible;
                btnAnswer2.Visibility = Visibility.Visible;
                btnAnswer3.Visibility = Visibility.Visible;
                btnAnswer4.Visibility = Visibility.Visible;
                btnFinish.Visibility = Visibility.Visible;
                btnNextQuestion.Visibility = Visibility.Visible;
                lbHeader.Content = "Question " + _questionCounter;
                lbScoreText.Visibility = Visibility.Visible;
                lbScore.Content = _score + _scoreOver;

                //place the answers to the right places in the buttons
                InitializeQuestion();
            }
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            //brings up the result window and hides the MainWindow
            Results result = new Results();
            result.Show();
            this.Hide();
        }

        //loads the next question
        private void btnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            _questionCounter++;
            lbHeader.Content = "Question " + _questionCounter;
            lbHeader.Foreground = Brushes.Black;
            lbScore.Content = _score + _scoreOver;

            //reset the color of each question button
            foreach (Button button in answerButtons)
            {
                button.Background = Brushes.Turquoise;
                button.IsHitTestVisible = true;
            }
            if (_questionCounter <= _SCOREMAX)
            {
                //loads the question data
                InitializeQuestion();
            }
            else
            {
                //if the last question was answered, it will show the results screen
                Results result = new Results();
                result.Show();
                this.Hide();
            }
        }

        private bool LoadQuestionFile()
        {
            //load sequence same as in CSConferenceApp done in class
            OpenFileDialog questionFile = new OpenFileDialog();
            questionFile.Filter = "Text Files|*.txt";
            questionFile.Title = "Select a Question File to Load";
            questionFile.ShowDialog();

            try
            {
                // File name is not provided
                if (questionFile.FileName == "")
                    throw new ArgumentException("File name cannot be empty");

                string[] lines = File.ReadAllLines(questionFile.FileName);

                // File is empty
                if (lines.Length == 0)
                    throw new ArgumentException("File cannot be empty");

                _SCOREMAX = lines.Length;
                _scoreOver = "/" + lines.Length.ToString();

                // Create list of Question objects and populate list
                questions = new List<Question>(lines.Length);
                PopulateQuestionList(questions, lines);

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        private void PopulateQuestionList(List<Question> questions, string[] fileContents)
        {
            const int QUESTIONINDEX = 0, CORRECTANSWERINDEX = 5;
            const int ANS1INDEX = 1, ANS2INDEX = 2, ANS3INDEX = 3, ANS4INDEX = 4;

            //this will load the parsed data from the laod into questions the program can use
            try
            {
                for (int i = 0; i < fileContents.Length; i++)
                {
                    //splitting the elements
                    string[] lineContents = fileContents[i].Split(',');
                    string[] answers = new string[] { lineContents[ANS1INDEX], lineContents[ANS2INDEX], lineContents[ANS3INDEX], lineContents[ANS4INDEX] };

                    //setting the questions into a new object of Question type
                    questions.Add(new Question(lineContents[QUESTIONINDEX], answers, lineContents[CORRECTANSWERINDEX]));
                }
                //adding it to the list
                Question.QuestionList = questions;
            }
            catch (Exception)
            {
                throw new ArgumentException("File contents are not valid or corrupted, ensure the file follows the structure of 'question,answer1,answer2,answer3,answer4,correctAnswer'");
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            //kills the app
            this.Close();
        }

        private void InitializeQuestion()
        {
            //clears the answer buttons and repopulates them from the list of questions based on current question as index
            answerButtons.Clear();
            txbQuestion.Text = questions[_questionCounter - 1].QuestionText;
            txbAnswer1Content.Text = questions[_questionCounter - 1].Answers[0];
            txbAnswer2Content.Text = questions[_questionCounter - 1].Answers[1];
            txbAnswer3Content.Text = questions[_questionCounter - 1].Answers[2];
            txbAnswer4Content.Text = questions[_questionCounter - 1].Answers[3];

            //sets the data to the buttons
            answerButtons.Add(btnAnswer1);
            answerButtons.Add(btnAnswer2);
            answerButtons.Add(btnAnswer3);
            answerButtons.Add(btnAnswer4);
        }

        public void Reset()
        {
            /*the following  lines of code were taken from this source:
            https://stackoverflow.com/questions/61843128/wpf-application-restart
            we used this to force a reset of the app to wipe the quiz data and bring
            the game game to a "fresh" state.*/

            var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(currentExecutablePath);
            Application.Current.Shutdown();
        }
    }
}