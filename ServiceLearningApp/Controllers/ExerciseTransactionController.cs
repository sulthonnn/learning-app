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
    [Route("api/exercise-transaction")]
    [Authorize(Policy = "Bearer")]
    public class ExerciseTransactionController : Controller
    {
        private readonly IExerciseTransactionRepository exerciseTransactionRepository;
        private readonly IMapper mapper;

        public ExerciseTransactionController(IExerciseTransactionRepository exerciseTransactionRepository, IMapper mapper)
        {
            this.exerciseTransactionRepository = exerciseTransactionRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> GetAllExerciseTransactionDto([FromQuery] QueryParams? queryParams)
        {
            var exerciseTransactions = await this.exerciseTransactionRepository.GetAllAsyncDto(queryParams);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = exerciseTransactions
            });
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetExerciseTransaction(int id)
        {
            var exerciseTransaction = await this.exerciseTransactionRepository.GetAsync(id);
            if (exerciseTransaction == null)
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
                Data = exerciseTransaction
            });
        }

        [HttpPost]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> CreateExerciseTransaction([FromBody] ExerciseTransaction exerciseTransaction)
        {
            try
            {
                await this.exerciseTransactionRepository.PostAsync(exerciseTransaction);

                var exercisceTransactionId = exerciseTransaction.Id;

                var historyAnswers = exerciseTransaction.HistoryAnswer;
                exerciseTransaction.HistoryAnswer = [];

                foreach (var historyAnswer in historyAnswers)
                {
                    historyAnswer.FkExerciseTransactionId = exercisceTransactionId;
                }

                await this.exerciseTransactionRepository.PostHistoryAnswerAsync(historyAnswers);

                var historyAnswerDtos = historyAnswers.Select(ha => new HistoryAnswerDto
                {
                    FkQuestionId = ha.FkQuestionId,
                    FkOptionId = ha.FkOptionId,
                    FkExerciseTransactionId = ha.FkExerciseTransactionId
                }).ToList();

                var exerciseTransactionDto = this.mapper.Map<ExerciseTransaction, ExerciseTransactionDto>(exerciseTransaction);
                exerciseTransactionDto.HistoryAnswer = historyAnswerDtos;

                exerciseTransaction.HistoryAnswer = historyAnswers;

                return new OkObjectResult(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Success",
                    Data = exerciseTransactionDto
                });
            }
            catch (Exception ex)
            {

                return new BadRequestObjectResult(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{id}/history-answer")]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetHistoryAnswerByExerciseId(int id)
        {
            var historyAnswers = await this.exerciseTransactionRepository.GetHistoryAnswerByExerciseId(id);
            if (historyAnswers == null || historyAnswers.Count == 0)
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
                Data = historyAnswers
            });
        }

        [HttpGet("ranking")]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetRankAsync([FromQuery] QueryParams? queryParams)
        {
            var rankings = await this.exerciseTransactionRepository.GetRankAsync(queryParams);
            if (rankings == null || rankings.Count == 0)
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
                Data = rankings
            });
        }

        [HttpGet("all-ranking")]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetAllRankingAsync([FromQuery] QueryParams? queryParams)
        {
            var rankings = await this.exerciseTransactionRepository.GetAllRankingAsync(queryParams);
            if (rankings == null || rankings.Count == 0)
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
                Data = rankings
            });
        }

        //[HttpPut("{id}")]
        //[Authorize(Policy = "Student")]
        //public async Task<IActionResult> UpdateExerciseTransaction(int id, [FromBody] ExerciseTransaction updatedExerciseTransaction)
        //{
        //    var existingExerciseTransaction = await this.exerciseTransactionRepository.GetAsync(id);

        //    if (existingExerciseTransaction == null)
        //    {
        //        return new BadRequestObjectResult(new
        //        {
        //            StatusCode = StatusCodes.Status404NotFound,
        //            Message = "Data tidak ditemukan"
        //        });
        //    }

        //    existingExerciseTransaction.Title = updatedExerciseTransaction.Title;
        //    //this.mapper.Map(updatedExerciseTransaction, existingExerciseTransaction);

        //    await this.exerciseTransactionRepository.PutAsync(existingExerciseTransaction);

        //    return new OkObjectResult(new
        //    {
        //        StatusCode = StatusCodes.Status200OK,
        //        Message = "Success",
        //        Data = existingExerciseTransaction
        //    });
        //}

        [HttpDelete("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> DeleteExerciseTransaction(int id)
        {
            var ExerciseTransaction = await this.exerciseTransactionRepository.GetAsync(id);
            if (ExerciseTransaction == null)
            {
                return new BadRequestObjectResult(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            await this.exerciseTransactionRepository.DeleteAsync(ExerciseTransaction.Id);
            
            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = this.mapper.Map<ExerciseTransaction, ExerciseTransactionDto>(ExerciseTransaction)
            });
        }
    }
}
