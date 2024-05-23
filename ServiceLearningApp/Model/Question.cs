using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model
{
    public class Question : BaseEntity
    {
        public required string QuestionText {  get; set; }  
        public required string FeedBack {  get; set; }

        [ForeignKey(nameof(FkImageId))]
        public Upload? Image { get; set; }
        public int FkImageId { get; set; }

        [ForeignKey(nameof(FkExerciseId))]
        public Exercise? Exercise { get; set; }
        public int FkExerciseId { get; set; }
    }
}
