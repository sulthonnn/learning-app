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
    [Route("api/[controller]")]
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
        [AllowAnonymous]
        public async Task<IActionResult> GetAllChapter([FromQuery] QueryParams? queryParams)
        {
            var chapters = await this.chapterRepository.GetAllAsync(queryParams);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = chapters
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetChapter(int id)
        {
            var chapter = await this.chapterRepository.GetAsync(id);
            if (chapter == null)
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
                Data = chapter
            });
        }

        [HttpPost]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> CreateChapter([FromBody] Chapter chapter)
        {
            await this.chapterRepository.PostAsync(chapter);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = this.mapper.Map<Chapter, ChapterDto>(chapter)
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
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            existingChapter.Title = updatedChapter.Title;
            //this.mapper.Map(updatedChapter, existingChapter);

            await this.chapterRepository.PutAsync(existingChapter);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
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
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            await this.chapterRepository.DeleteAsync(chapter.Id);
            
            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = this.mapper.Map<Chapter, ChapterDto>(chapter)
            });
        }
    }
}
