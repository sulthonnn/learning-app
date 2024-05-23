using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model
{
    public class Option : BaseEntity
    {
        public required string OptionText { get; set; }
        public bool IsAnswer { get; set; }

        [ForeignKey(nameof(FkQuestionId))]
        public Question? Question { get; set; }
        public int FkQuestionId { get; set; }

    }
}
