using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLearningApp.Helpers;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;
using ServiceLearningApp.Storage;

namespace ServiceLearningApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class UploadController : Controller
    {
        private readonly IUploadRepository uploadRepository;
        private readonly IMapper mapper;
        private readonly IStorageService storageService;

        public UploadController(IUploadRepository uploadRepository, IMapper mapper, IStorageService storageService, string uploadPath)
        {
            this.uploadRepository = uploadRepository;
            this.mapper = mapper;
            this.storageService = storageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUpload([FromQuery] QueryParams? queryParams)
        {
            var uploads = await this.uploadRepository.GetAllAsync(queryParams);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = uploads
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUpload(int id)
        {
            var upload = await this.uploadRepository.GetAsync(id);
            if (upload == null)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = upload
            });
        }

        [HttpPost]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> CreateUpload([FromBody] Upload upload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await this.uploadRepository.PostAsync(upload);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = upload
            });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> UpdateUpload(int id, [FromBody] Upload updatedUpload)
        {
            var existingUpload = await this.uploadRepository.GetAsync(id);

            if (existingUpload == null)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            existingUpload.Name = updatedUpload.Name;

            await this.uploadRepository.PutAsync(existingUpload);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = existingUpload
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> DeleteUpload(int id)
        {
            var upload = await uploadRepository.GetAsync(id);
            if (upload == null)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            await this.uploadRepository.DeleteAsync(upload.Id);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = upload
            });
        }

        [HttpGet("download/{folder}/{file}")]
        [ResponseCache(Duration = 31536000)]
        public async Task<FileStreamResult> DownloadAsync(string folder, string file)
        {
            var url = folder + "/" + file;
            return await this.uploadRepository.DownloadAsync(url, Request.Headers.Range);
        }

        [HttpPost("upload/image")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> UploadImageAsync([FromForm] UploadType type, [FromForm] IFormFile file, [FromForm] bool compress = false)
        {
            var upload = await this.uploadRepository.UploadImageAsync(type, file, compress);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = upload
            });
        }

        [HttpPost("upload/file")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> UploadFileAsync([FromForm] UploadDto model)
        {
            var upload = await this.uploadRepository.UploadFileAsync(model);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = upload
            });
        }
    }
}
