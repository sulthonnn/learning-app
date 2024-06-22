using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLearningApp.Helpers;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;

namespace ServiceLearningApp.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/sub-chapter")]
    [Authorize(Policy = "Bearer")]
    public class SubChapterController : Controller
    {
        private readonly ISubChapterRepository subChapterRepository;
        private readonly IMapper mapper;
        private readonly IUploadRepository uploadRepository;

        public SubChapterController(ISubChapterRepository SubChapterRepository, IMapper mapper, IUploadRepository uploadRepository)
        {
            this.subChapterRepository = SubChapterRepository;
            this.mapper = mapper;
            this.uploadRepository = uploadRepository;
        }

        [HttpGet]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetAllSubChapter(QueryParams? queryParams)
        {
            var SubChapters = await this.subChapterRepository.GetAllAsync(queryParams);

            return new OkObjectResult(new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Data subbab berhasil didapatkan",
                Data = SubChapters
            });
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetSubChapter(int id)
        {
            var SubChapter = await this.subChapterRepository.GetAsync(id);
            if (SubChapter == null)
            {
                return new BadRequestObjectResult(new
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = "Not Found",
                    Message = "Data tidak ditemukan"
                });
            }

            return new OkObjectResult(new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Data subbab berhasil didapatkan",
                Data = SubChapter
            });
        }

        [HttpPost]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> CreateSubChapter([FromBody] SubChapter SubChapter)
        {

            await this.subChapterRepository.PostAsync(SubChapter);

            return new CreatedResult("", new
            {
                Code = StatusCodes.Status201Created,
                Status = "Ok",
                Message = "Data subbab berhasil dibuat",
                Data = this.mapper.Map<SubChapter, SubChapterDto>(SubChapter)
            });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> UpdateSubChapter(int id, [FromBody] SubChapter updatedSubChapter)
        {
            var existingSubChapter = await this.subChapterRepository.GetAsync(id);

            if (existingSubChapter == null)
            {
                return new BadRequestObjectResult(new
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = "Not Found",
                    Message = "Data tidak ditemukan"
                });
            }

            existingSubChapter.Title = updatedSubChapter.Title;
            existingSubChapter.Reference = updatedSubChapter.Reference;
            existingSubChapter.FkChapterId = updatedSubChapter.FkChapterId;
            existingSubChapter.FkUploadId = updatedSubChapter.FkUploadId;

            await this.subChapterRepository.PutAsync(existingSubChapter);

            return new OkObjectResult(new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Data subbab berhasil diubah",
                Data = this.mapper.Map<SubChapter, SubChapterDto>(existingSubChapter)
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> DeleteSubChapter(int id)
        {
            var SubChapter = await this.subChapterRepository.GetAsync(id);
            if (SubChapter == null)
            {
                return new BadRequestObjectResult(new
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = "Not Found",
                    Message = "Data tidak ditemukan"
                });
            }

            await this.subChapterRepository.DeleteAsync(SubChapter.Id);

            return new OkObjectResult(new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Data subbab berhasil dihapus",
                Data = this.mapper.Map<SubChapter, SubChapterDto>(SubChapter)
            });
        }

        [HttpPost("upload")]
        [Authorize(Policy = "Teacher")]
        public async Task<ActionResult<Upload>> Upload([FromForm] UploadDto model)
        {
            try
            {
                var upload = await uploadRepository.UploadFileAsync(model);
                return new OkObjectResult(new
                {
                    Code = StatusCodes.Status200OK,
                    Status = "Ok",
                    Message = "File subbab berhasil di-upload",
                    Data = upload
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }        
        }

        [HttpGet("download/{folder}/{file}")]
        [Authorize(Policy = "Student")]
        [ResponseCache(Duration = 3600)]
        public async Task<FileStreamResult> Download(string folder, string file)
        {
            var url = Path.Combine(folder, file).Replace("\\", "/");

            return await uploadRepository.DownloadAsync(url, Request.Headers.Range);
        }
    }
}
