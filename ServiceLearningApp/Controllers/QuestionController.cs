using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLearningApp.Data;
using ServiceLearningApp.Helpers;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model;
using ServiceLearningApp.Security;

namespace ServiceLearningApp.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class QuestionController : Controller
    {
        private readonly IQuestionRepository QuestionRepository;
        private readonly IMapper mapper;

        public QuestionController(IQuestionRepository QuestionRepository, IMapper mapper)
        {
            this.QuestionRepository = QuestionRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllQuestion([FromQuery] QueryParams? queryParams)
        {
            var Questions = await this.QuestionRepository.GetAllAsync(queryParams);

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
            var Question = await this.QuestionRepository.GetAsync(id);
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

            await this.QuestionRepository.PostAsync(Question);

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
            var existingQuestion = await this.QuestionRepository.GetAsync(id);

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

            await this.QuestionRepository.PutAsync(existingQuestion);

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
            var Question = await this.QuestionRepository.GetAsync(id);
            if (Question == null)
            {
                return new BadRequestObjectResult(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            await this.QuestionRepository.DeleteAsync(Question.Id);
            
            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = Question
            });
        }
    }
}
