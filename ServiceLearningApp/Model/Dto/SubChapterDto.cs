using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model.Dto
{
    public class SubChapterDto
    {
        public required string Title { get; set; }
        public string? Reference { get; set; }
        public required string ChapterTitle {  get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
    }
}
