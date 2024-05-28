using Microsoft.EntityFrameworkCore;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model;
using ServiceLearningApp.Helpers;
using ServiceLearningApp.Storage;
using ServiceLearningApp.Model.Dto;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace ServiceLearningApp.Data
{
    public class UploadRepository : ControllerBase, IUploadRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IStorageService storageService;
        public string uploadPath = "Assets";

        public UploadRepository(ApplicationDbContext dbContext, IStorageService storageService)
        {
            this.dbContext = dbContext;
            this.storageService = storageService;
        }

        public async Task<IReadOnlyList<Upload>> GetAllAsync(QueryParams? queryParams)
        {
            IQueryable<Upload> query = this.dbContext.Uploads;

            // Filtering & Sorting
            query = ApplyFilterAndSort(query, queryParams);
            // Pagination
            query = ApplyPagination(query, queryParams);

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Upload> GetAsync(int id)
        {
            return await this.dbContext.Uploads
                .AsNoTracking()
                .FirstAsync(c => c.Id == id);
        }

        public async Task PostAsync(Upload entity)
        {
            await this.dbContext.Uploads.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await this.dbContext.Uploads.CountAsync();
        }

        public async Task PutAsync(Upload entity)
        {
            this.dbContext.Entry(entity).State = EntityState.Modified;
            await this.dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var upload = await this.dbContext.Uploads.FindAsync(id);
            if (upload != null)
            {
                this.dbContext.Uploads.Remove(upload);
                await this.dbContext.SaveChangesAsync();
            }
        }

        public async Task<FileStreamResult> DownloadAsync(string url, StringValues range)
        {
            try
            {
                var filePath = Path.Combine(uploadPath, url);
                var lastModifiedDate = await this.storageService.GetLastModified(filePath);

                var fileStream = range.IsNullOrEmpty()
                    ? await this.storageService.ReadAsync(filePath)
                    : await this.storageService.ReadAsync(filePath, range);

                new FileExtensionContentTypeProvider().TryGetContentType(url, out var contentType);
                return File(
                    fileStream,
                    contentType ?? "application/octet-stream",
                    lastModifiedDate,
                    new EntityTagHeaderValue("\"" + lastModifiedDate.Ticks.ToString() + "\""),
                    true
                );
            }
            catch (Exception ex)
            {
                if (ex is DirectoryNotFoundException || ex is FileNotFoundException)
                    throw new InvalidOperationException("File not found", ex);
                throw;
            }
        }

        public async Task<Upload> UploadImageAsync(UploadType type, IFormFile file, bool compress = false)
        {
            // Validasi jika file null
            if (file == null)
            {
                throw new ArgumentNullException("File tidak boleh kosong.");
            }

            var allowedContentTypes = new List<string> { "image/jpeg", "image/png", "image/gif", "image/svg+xml", "image/webp", "image/jpg" };
            if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
            {
                throw new ArgumentException("Hanya file gambar (JPEG, PNG, GIF, SVG, WEBP, JPG) yang diizinkan.");
            }

            var directoryPath = Path.Combine(uploadPath, Guid.NewGuid().ToString());
            await storageService.CreateDirectoryAsync(directoryPath);

            var filePath = Path.Combine(directoryPath, file.FileName);

            if (compress)
            {
                filePath = Path.ChangeExtension(filePath, "webp");
                using (var imageStream = new MemoryStream())
                {
                    ImageHelper.Resize(file.OpenReadStream(), imageStream, 700);
                    imageStream.Seek(0, SeekOrigin.Begin);
                    await this.storageService.WriteAsync(filePath, imageStream);
                }
            }
            else
            {
                using var stream = await this.storageService.WriteAsync(filePath, file.OpenReadStream());
            }

            var thumbnailPath = ImageHelper.GetThumbnailPath(filePath);
            using (var thumbStream = new MemoryStream())
            {
                ImageHelper.CreateThumbnail(file.OpenReadStream(), thumbStream);
                thumbStream.Seek(0, SeekOrigin.Begin);
                await this.storageService.WriteAsync(thumbnailPath, thumbStream);
            }

            var upload = new Upload
            {
                Id = 0,
                Name = file.FileName,
                Size = file.Length,
                Type = type,
                Url = Path.GetRelativePath(uploadPath, filePath).Replace(Path.DirectorySeparatorChar, '/'),
                ThumbnailUrl = Path.GetRelativePath(uploadPath, thumbnailPath).Replace(Path.DirectorySeparatorChar, '/'),
            };

            this.dbContext.Add(upload);
            await this.dbContext.SaveChangesAsync();

            return upload;
        }

        public async Task<Upload> UploadFileAsync(UploadDto model)
        {
            // Validasi jika file null
            if (model.File == null)
            {
                throw new ArgumentNullException("File tidak boleh kosong.");
            }

            // Validasi jika file bukan PDF
            if (model.File.ContentType.ToLower() != "application/pdf")
            {
                throw new ArgumentException("Hanya file pdf yang diizinkan.");
            }

            var directoryPath = Path.Combine(uploadPath, Guid.NewGuid().ToString());
            await this.storageService.CreateDirectoryAsync(directoryPath);

            var filePath = Path.Combine(directoryPath, model.File.FileName);
            using var stream = await this.storageService.WriteAsync(filePath, model.File.OpenReadStream());

            //document
            model.Type = UploadType.Document;

            var upload = new Upload
            {
                Id = 0,
                Name = model.File.FileName,
                Size = model.File.Length,
                Type = model.Type,
                Url = Path.GetRelativePath(uploadPath, filePath).Replace(Path.DirectorySeparatorChar, '/'),
            };

            this.dbContext.Add(upload);
            await this.dbContext.SaveChangesAsync();

            return upload;
        }

        public async Task<Upload> UploadFileAsync(Stream file, string fileName, long length, UploadType type)
        {
            var directoryPath = Path.Combine(uploadPath, Guid.NewGuid().ToString());
            await this.storageService.CreateDirectoryAsync(directoryPath);

            var filePath = Path.Combine(directoryPath, fileName);
            using var stream = await this.storageService.WriteAsync(filePath, file);

            var upload = new Upload
            {
                Id = 0,
                Name = fileName,
                Size = length,
                Type = type,
                Url = Path.GetRelativePath(uploadPath, filePath).Replace(Path.DirectorySeparatorChar, '/'),
            };

            dbContext.Add(upload);
            await dbContext.SaveChangesAsync();

            return upload;
        }

        private IQueryable<Upload> ApplyFilterAndSort(IQueryable<Upload> query, QueryParams? queryParams)
        {
            if (queryParams == null)
                return query;

            // Filtering
            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query = query.Where(e => EF.Functions.ILike(e.Name, "%" + queryParams.Search + "%"));
            }

            // Sorting
            if (!string.IsNullOrEmpty(queryParams.Sort))
            {
                query = queryParams.Sort switch
                {
                    "name" => query.OrderBy(e => e.Name),
                    "-name" => query.OrderByDescending(e => e.Name),
                    _ => query.OrderBy(e => e.Id),
                };
            }

            return query;
        }

        private IQueryable<Upload> ApplyPagination(IQueryable<Upload> query, QueryParams? queryParams)
        {
            if (queryParams == null)
                return query;

            // Pagination
            if (queryParams.Page > 0 && queryParams.PerPage > 0)
            {
                query = query.Skip((queryParams.Page - 1) * queryParams.PerPage).Take(queryParams.PerPage);
            }

            return query;
        }
    }
}
