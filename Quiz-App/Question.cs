using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace PIIIProject
{
    public class Question
    {
        private static List<Question> _questionList;
        private string _selectedAnswer;
        private string _correctAnswer;
        private string _questionText;
        private string[] _answers;

        public Question(string questionText, string[] answers, string correctAnswer)
        {
            _selectedAnswer = "";
            QuestionText = questionText;
            Answers = answers;
            CorrectAnswer = correctAnswer;
        }

        public string SelectedAnswer
        {
            get { return _selectedAnswer; }
            set
            {
                if (!Answers.Contains(value))
                    throw new ArgumentException("Selected answer must be one of the answers");
                _selectedAnswer = value;
            }
        }
        public string CorrectAnswer
        {
            get { return _correctAnswer; }
            private set
            {
                if (!Answers.Contains(value))
                    throw new ArgumentException("Correct answer must be one of the answers");
                _correctAnswer = value;
            }
        }
        public string QuestionText
        {
            get { return _questionText; }
            set { _questionText = value; }
        }
        public string[] Answers
        {
            get { return _answers; }
            set { _answers = value; }
        }
        public bool IsCorrectAnswer
        {
            get { return SelectedAnswer == _correctAnswer; }
        }

        // Returns Question, Selected Answer, Correct Answer and IsCorrectAnswer bool value
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(QuestionText);
            sb.Append(",");
            sb.Append(SelectedAnswer);
            sb.Append(",");
            sb.Append(CorrectAnswer);
            sb.Append(",");
            sb.Append(IsCorrectAnswer);
            return sb.ToString();
        }
        public static List<Question> QuestionList
        {
            get { return _questionList; }
            set { _questionList = value; }
        }
    }
}