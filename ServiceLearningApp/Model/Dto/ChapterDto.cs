using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model.Dto
{
    public class ChapterDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }

    }
}
