using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model
{
    public class Question : BaseEntity
    {
        public required string QuestionText {  get; set; }  
        public string? FeedBack {  get; set; }

        [ForeignKey(nameof(FkImageId))]
        public Upload? Image { get; set; }
        public int? FkImageId { get; set; }

        [ForeignKey(nameof(FkSubChapterId))]
        public SubChapter? SubChapter { get; set; }
        public int FkSubChapterId { get; set; }
    }
}
