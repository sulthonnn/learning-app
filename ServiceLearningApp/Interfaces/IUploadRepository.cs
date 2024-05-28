using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;

namespace ServiceLearningApp.Interfaces
{
    public interface IUploadRepository : IGenericRepository<Upload>
    {
        Task<FileStreamResult> DownloadAsync(string url, StringValues range);
        Task<Upload> UploadImageAsync(UploadType type, IFormFile file, bool compress = false);
        Task<Upload> UploadFileAsync(UploadDto model);
        Task<Upload> UploadFileAsync(Stream file, string fileName, long length, UploadType type);
    }
}
