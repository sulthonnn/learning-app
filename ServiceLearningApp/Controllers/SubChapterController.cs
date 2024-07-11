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
        public async Task<IActionResult> CreateSubChapter([FromBody] SubChapter subChapter)
        {
            try
            {
                await this.subChapterRepository.PostAsync(subChapter);

                return new CreatedResult("", new
                {
                    Code = StatusCodes.Status201Created,
                    Status = "Ok",
                    Message = "Data subbab berhasil dibuat",
                    Data = this.mapper.Map<SubChapter, SubChapterDto>(subChapter)
                });

            }
            catch (Exception)
            {
                return BadRequest(new
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = "Bad Request",
                    Message = "File upload tidak boleh kosong"
                });
            }

        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> UpdateSubChapter(int id, [FromBody] SubChapter subChapter)
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

            existingSubChapter.Title = subChapter.Title;
            existingSubChapter.Reference = subChapter.Reference;
            existingSubChapter.FkChapterId = subChapter.FkChapterId;
            if (subChapter.FkUploadId != 0)
            {
                existingSubChapter.FkUploadId = subChapter.FkUploadId;
            }

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
                    Code = StatusCodes.Status400BadRequest,
                    Status = "Bad Request",
                    Message = ex.Message
                });
            }        
        }

        [HttpGet("download/{folder}/{file}")]
        [Authorize(Policy = "Student")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> Download(string folder, string file)
        {
            try
            {
                var url = Path.Combine(folder, file).Replace("\\", "/");

                var fileStreamResult = await uploadRepository.DownloadAsync(url, Request.Headers.Range);

                return fileStreamResult;
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = "Bad Request",
                    Message = ex.Message
                });
            }
        }

    }
}
