using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model
{
    public class SubChapter : BaseEntity
    {
        public required string Title { get; set; }
        public string? Reference { get; set; }


        [ForeignKey(nameof(FkChapterId))]
        public Chapter? Chapter { get; set; }
        public int FkChapterId { get; set; }

        [ForeignKey(nameof(FkImageId))]
        public Upload? Image { get; set; }
        public int? FkImageId { get; set; }

    }
}
