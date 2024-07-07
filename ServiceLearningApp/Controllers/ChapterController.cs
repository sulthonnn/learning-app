using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLearningApp.Helpers;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model;

namespace ServiceLearningApp.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class ChapterController : Controller
    {
        private readonly IChapterRepository chapterRepository;
        private readonly IMapper mapper;

        public ChapterController(IChapterRepository chapterRepository, IMapper mapper)
        {
            this.chapterRepository = chapterRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetAllChapter([FromQuery] QueryParams? queryParams)
        {
                var chapters = await this.chapterRepository.GetAllAsync(queryParams);

                return new OkObjectResult(new
                {
                    Code = StatusCodes.Status200OK,
                    Status = "Ok",
                    Message = "Data bab berhasil didapatkan",
                    Data = chapters
                });
            }

        [HttpGet("{id}")]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetChapter(int id)
        {
            var chapter = await this.chapterRepository.GetAsync(id);
            if (chapter == null)
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
                Message = "Data bab berhasil didapatkan",
                Data = chapter
            });
        }

        [HttpPost]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> CreateChapter([FromBody] Chapter chapter)
        {
            await this.chapterRepository.PostAsync(chapter);

            return new CreatedResult("", new
            {
                Code = StatusCodes.Status201Created,
                Status = "Created",
                Message = "Data bab berhasil dibuat",
                Data = chapter
            });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> UpdateChapter(int id, [FromBody] Chapter updatedChapter)
        {
            var existingChapter = await this.chapterRepository.GetAsync(id);

            if (existingChapter == null)
            {
                return new BadRequestObjectResult(new
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = "Not Found",
                    Message = "Data tidak ditemukan"
                });
            }

            existingChapter.Title = updatedChapter.Title;

            await this.chapterRepository.PutAsync(existingChapter);

            return new OkObjectResult(new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Data Bab berhasil diubah",
                Data = existingChapter
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            var chapter = await this.chapterRepository.GetAsync(id);
            if (chapter == null)
            {
                return new BadRequestObjectResult(new
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = "Not Found",
                    Message = "Data tidak ditemukan"
                });
            }

            await this.chapterRepository.DeleteAsync(chapter.Id);
            
            return new OkObjectResult(new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Data bab berhasil dihapus",
                Data = chapter
            });
        }
    }
}
