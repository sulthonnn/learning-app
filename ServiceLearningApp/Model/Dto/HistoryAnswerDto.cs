using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model.Dto
{
    public class HistoryAnswerDto
    {
        public int Id { get; set; }
        public int FkQuestionId { get; set; }
        public required string Question { get; set; }
        public int FkOptionId { get; set; }
        public required string Option { get; set; }
        public bool IsAnswer { get; set; }

    }
}
