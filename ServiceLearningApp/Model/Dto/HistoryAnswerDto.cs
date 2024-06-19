using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model.Dto
{
    public class HistoryAnswerDto
    {
        public int Id { get; set; }
        public int FkQuestionId { get; set; }
        public int FkOptionId { get; set; }
    }
}
