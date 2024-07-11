using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model.Dto
{
    public class RandomQuestionDto
    {
        public int Id { get; set; }
        public required string QuestionText { get; set; }
        public string? FeedBack { get; set; }
        public int? FkImageId { get; set; }
        public string Url {  get; set; }
        public string ThumbnailUrl { get; set; }

        public int FkSubChapterId { get; set; }

        public List<OptionDto> Options { get; set; }
    }
}
