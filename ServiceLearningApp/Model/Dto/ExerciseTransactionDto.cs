using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model.Dto
{
    public class ExerciseTransactionDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CorrectAnswer { get; set; }
        public int IncorrectAnswer { get; set; }
        public int Score { get; set; }
        public int FkSubChapterId { get; set; }
        public required string SubChapter { get; set; }
        public required string UserFullName { get; set; }

        public List<HistoryAnswerDto> HistoryAnswer { get; set; }
    }
}
