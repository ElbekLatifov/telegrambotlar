namespace SavolJavobBot.Models
{
    class SAvollar
    {
        public string? Savol;
        public List<string>? Variantlar;
        public string? CorrectAnswer;

        public bool IsCorrect(string javob)
        {
            return CorrectAnswer == javob;
        }

        public bool IsBor(string matn)
        {
            return Variantlar!.Contains(matn);
        }

        public void SaveQuestions(string savol, List<string> variants, string Correctanswer)
        {
            Savol = savol; 
            Variantlar = variants;
            CorrectAnswer = Correctanswer;
        }
    }
}
