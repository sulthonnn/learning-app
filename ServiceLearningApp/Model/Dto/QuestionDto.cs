using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model.Dto
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public required string QuestionText { get; set; }
        public string? FeedBack { get; set; }
        public int? FkImageId { get; set; }
        public int FkSubChapterId { get; set; }
    }
}
