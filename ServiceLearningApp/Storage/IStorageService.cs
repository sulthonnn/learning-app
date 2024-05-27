
namespace ServiceLearningApp.Storage
{
    public interface IStorageService
    {
        Task<Stream> ReadAsync(string path);

        Task<Stream> ReadAsync(string path, string range);

        Task<Stream> ReadAsync(string path, int bufferSize);

        Task<Stream> WriteAsync(string path, byte[] content);

        Task<Stream> WriteAsync(string path, Stream content);

        Task<Stream> WriteAsync(string path, Stream content, int bufferSize);

        Task DeleteAsync(string path);

        Task CreateDirectoryAsync(string path);

        Task<DateTime> GetLastModified(string path);
    }
}