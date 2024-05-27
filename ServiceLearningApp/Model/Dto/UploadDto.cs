namespace ServiceLearningApp.Model.Dto
{
    public class UploadDto
    {
        public UploadType Type { get; set; }

        public IFormFile File { get; set; }
    }
}
