using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model
{
    public class Exercise : BaseEntity
    {
        public required string Title { get; set; }

        [ForeignKey(nameof(FkSubChapterId))]
        public SubChapter? SubChapter { get; set; }
        public int FkSubChapterId { get; set; }
    }
}
