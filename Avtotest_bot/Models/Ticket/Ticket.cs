namespace Avtotest_bot.Models
{
    class TicketM
    {
        public int Index { get; set; }
        public int CorrectCount { get; set; }
        public int TotalAnswersCount { get; set; }
        public int StartIndex { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public bool IsCompleted
        {
            get
            {
                return StartIndex + QuestionsCount <= CurrentQuestionIndex;
            }
        }
        public int QuestionsCount { get; set; }

        public TicketM()
        {

        }

        public TicketM(int index, int questionsCount)
        {
            Index = index;
            QuestionsCount = questionsCount;
            StartIndex = index * questionsCount;
            CurrentQuestionIndex = StartIndex;
        }


        public void SetDefault()
        {
            CurrentQuestionIndex = StartIndex;
            CorrectCount = 0;
        }
    }
}
