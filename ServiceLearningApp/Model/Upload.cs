namespace ServiceLearningApp.Model
{
    public class Upload : BaseEntity
    {
        public required string Url { get; set; }
        public string? ThumbnailUrl { get; set; }
        public required string Name { get; set; }
        public long Size { get; set; }
        public UploadType Type { get; set; }
    }
}
