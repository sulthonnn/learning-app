using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model.Dto
{
    public class OptionDto
    {
        public int Id { get; set; }
        public required string OptionText { get; set; }
        public bool IsAnswer { get; set; }
        public int FkQuestionId { get; set; }
    }
}
