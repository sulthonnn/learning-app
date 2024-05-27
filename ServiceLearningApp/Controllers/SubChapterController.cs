using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLearningApp.Data;
using ServiceLearningApp.Helpers;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;
using ServiceLearningApp.Security;

namespace ServiceLearningApp.Controllers
{
    [Produces("application/json")]
    [Route("api/sub-chapter")]
    //[Authorize(Policy = "Bearer")]
    public class SubChapterController : Controller
    {
        private readonly ISubChapterRepository SubChapterRepository;
        private readonly IMapper mapper;
        private readonly IUploadRepository uploadRepository;

        public SubChapterController(ISubChapterRepository SubChapterRepository, IMapper mapper, IUploadRepository uploadRepository)
        {
            this.SubChapterRepository = SubChapterRepository;
            this.mapper = mapper;
            this.uploadRepository = uploadRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllSubChapter(QueryParams? queryParams)
        {
            var SubChapters = await this.SubChapterRepository.GetAllAsync(queryParams);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = SubChapters
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubChapter(int id)
        {
            var SubChapter = await this.SubChapterRepository.GetAsync(id);
            if (SubChapter == null)
            {
                return new BadRequestObjectResult(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = SubChapter
            });
        }

        [HttpPost]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> CreateSubChapter([FromBody] SubChapter SubChapter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await this.SubChapterRepository.PostAsync(SubChapter);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = SubChapter
            });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> UpdateSubChapter(int id, [FromBody] SubChapter updatedSubChapter)
        {
            var existingSubChapter = await this.SubChapterRepository.GetAsync(id);

            if (existingSubChapter == null)
            {
                return new BadRequestObjectResult(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            existingSubChapter.Title = updatedSubChapter.Title;
            existingSubChapter.Reference = updatedSubChapter.Reference;
            existingSubChapter.FkChapterId = updatedSubChapter.FkChapterId;
            existingSubChapter.FkUploadId = updatedSubChapter.FkUploadId;

            await this.SubChapterRepository.PutAsync(existingSubChapter);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = existingSubChapter
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> DeleteSubChapter(int id)
        {
            var SubChapter = await this.SubChapterRepository.GetAsync(id);
            if (SubChapter == null)
            {
                return new BadRequestObjectResult(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            await this.SubChapterRepository.DeleteAsync(SubChapter.Id);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = SubChapter
            });
        }

        [HttpPost("upload")]
        public async Task<ActionResult<Upload>> Upload([FromForm] UploadDto model)
        {
            try
            {
                var upload = await uploadRepository.UploadFileAsync(model);
                return upload;
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
        [ResponseCache(Duration = 3600)]
        public async Task<FileStreamResult> Download(string folder, string file)
        {
            var url = Path.Combine(folder, file).Replace("\\", "/");

            return await uploadRepository.DownloadAsync(url, Request.Headers.Range);
        }
    }
}
