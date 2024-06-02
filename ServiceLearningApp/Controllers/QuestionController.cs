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
    public class QuestionController : Controller
    {
        private readonly IQuestionRepository questionRepository;
        private readonly IUploadRepository uploadRepository;
        private readonly IMapper mapper;

        public QuestionController(IQuestionRepository QuestionRepository, IMapper mapper, IUploadRepository uploadRepository)
        {
            this.questionRepository = QuestionRepository;
            this.uploadRepository = uploadRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllQuestion([FromQuery] QueryParams? queryParams)
        {
            var Questions = await this.questionRepository.GetAllAsync(queryParams);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = Questions
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestion(int id)
        {
            var Question = await this.questionRepository.GetAsync(id);
            if (Question == null)
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
                Data = Question
            });
        }

        [HttpPost]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> CreateQuestion([FromBody] Question Question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await this.questionRepository.PostAsync(Question);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = Question
            });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] Question updatedQuestion)
        {
            var existingQuestion = await this.questionRepository.GetAsync(id);

            if (existingQuestion == null)
            {
                return new BadRequestObjectResult(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            existingQuestion.QuestionText = updatedQuestion.QuestionText;
            existingQuestion.FeedBack = updatedQuestion.FeedBack;
            existingQuestion.FkImageId = updatedQuestion.FkImageId;
            existingQuestion.FkSubChapterId = updatedQuestion.FkSubChapterId;
            //this.mapper.Map(updatedQuestion, existingQuestion);

            await this.questionRepository.PutAsync(existingQuestion);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = existingQuestion
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var Question = await this.questionRepository.GetAsync(id);
            if (Question == null)
            {
                return new BadRequestObjectResult(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            await this.questionRepository.DeleteAsync(Question.Id);
            
            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = Question
            });
        }

        [HttpGet("random/sub-chapter/{subChapterId}/count/{count}")]
        [AllowAnonymous]
        public async Task<ActionResult<Question>> GetRandomQuestionsBySubChapter(int subChapterId, int count)
        {
            var randomQuestion = await this.questionRepository.GetRandomQuestionsBySubChapterIdAsync(subChapterId, count);
            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = randomQuestion
            });
        }

        [HttpGet("random/chapter/{chapterId}/count/{count}")]
        [AllowAnonymous]
        public async Task<ActionResult<Question>> GetRandomQuestionByChapter(int chapterId, int count)
        {
            var randomQuestion = await this.questionRepository.GetRandomQuestionsByChapterIdAsync(chapterId, count);
            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = randomQuestion
            });
        }

        [HttpPost("image")]
        //[HttpPut("image")]
        //[Authorize(Policy = "Administrator")]
        public async Task<ActionResult<Upload>> UploadImageAsync([FromForm] UploadDto model)
        {
            try
            {
                var upload = await this.uploadRepository.UploadImageAsync(UploadType.QuestionImage, model.File);
                return new OkObjectResult(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success",
                    Data = upload
                }); ;
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message,
                });
            }
        }


        [HttpGet("image/{folder}/{file}")]
        [AllowAnonymous]
        [ResponseCache(Duration = 3600)]
        public async Task<FileStreamResult> Download(string folder, string file)
        {
            var url = folder + "/" + file;
            return await this.uploadRepository.DownloadAsync(url, Request.Headers.Range);
        }
    }
}
