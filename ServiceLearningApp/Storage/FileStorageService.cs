

namespace ServiceLearningApp.Storage
{
    public class FileStorageService : IStorageService
    {
        public Task<Stream> ReadAsync(string path)
        {
            return Task.FromResult((Stream)new FileStream(path, FileMode.Open, FileAccess.Read));
        }

        public Task<Stream> ReadAsync(string path, string range)
        {
            return ReadAsync(path);
        }

        public Task<Stream> ReadAsync(string path, int bufferSize)
        {
            return Task.FromResult((Stream)new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize));
        }

        public async Task<Stream> WriteAsync(string path, byte[] content)
        {
            var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            await fileStream.WriteAsync(content.AsMemory(0, content.Length));
            return fileStream;
        }

        public async Task<Stream> WriteAsync(string path, Stream content)
        {
            var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            await content.CopyToAsync(fileStream);
            return fileStream;
        }

        public async Task<Stream> WriteAsync(string path, Stream content, int bufferSize)
        {
            var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, bufferSize);
            await content.CopyToAsync(fileStream);
            return fileStream;
        }

        public Task CreateDirectoryAsync(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string path)
        {
            File.Delete(path);
            return Task.CompletedTask;
        }

        public Task<DateTime> GetLastModified(string path)
        {
            return Task.FromResult(File.GetLastWriteTime(path));
        }
    }
}