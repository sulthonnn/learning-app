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
    [Route("api/v1/exercise-transaction")]
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
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetAllExerciseTransaction([FromQuery] QueryParams? queryParams)
        {
            var exerciseTransactions = await this.exerciseTransactionRepository.GetAllAsync(queryParams);
            
            return new OkObjectResult(new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Data latihan soal yang sudah dikerjakan berhasil didapatkan",
                Data = this.mapper.Map<IReadOnlyList<ExerciseTransactionListDto>>(exerciseTransactions)
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
                    Code = StatusCodes.Status404NotFound,
                    Status = "Not Found",
                    Message = "Data tidak ditemukan"
                });
            }

            return new OkObjectResult(new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Data latihan soal yang sudah dikerjakan berhasil didapatkan",
                Data = this.mapper.Map<ExerciseTransaction, ExerciseTransactionDto>(exerciseTransaction)
            });
        }

        [HttpPost]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> CreateExerciseTransaction([FromBody] ExerciseTransaction exerciseTransaction)
        {
            if (exerciseTransaction.HistoryAnswer == null || !exerciseTransaction.HistoryAnswer.Any())
            {
                return new BadRequestObjectResult(new { Code = StatusCodes.Status400BadRequest, Status = "Bad Request", Message = "History Answer tidak boleh kosong" });
            }

            try
            {
                await this.exerciseTransactionRepository.PostAsync(exerciseTransaction);

                var ex = await this.exerciseTransactionRepository.GetAsync(exerciseTransaction.Id);

                var exerciseTransactionDto = this.mapper.Map<ExerciseTransaction, ExerciseTransactionDto>(exerciseTransaction);
                exerciseTransactionDto.SubChapter = ex.SubChapter.Title; // Example assuming SubChapterDto has a Title property
                exerciseTransactionDto.UserFullName = ex.User.FullName;

                return new CreatedResult("", new
                {
                    Code = StatusCodes.Status201Created,
                    Status = "Created",
                    Message = "Berhasil mengerjakan latihan soal",
                    Data = exerciseTransactionDto
                });
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


        [HttpGet("{id}/history-answer")]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> GetHistoryAnswerByExerciseId(int id)
        {
            var historyAnswers = await this.exerciseTransactionRepository.GetHistoryAnswerByExerciseId(id);
            if (historyAnswers == null || historyAnswers.Count == 0)
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
                Message = "History jawaban pertanyaan berhasil didapatkan",
                Data = this.mapper.Map<IReadOnlyList<HistoryAnswer>, IReadOnlyList<HistoryAnswerDto>>(historyAnswers)
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
                    Code = StatusCodes.Status404NotFound,
                    Status = "Not Found",
                    Message = "Data tidak ditemukan"
                });
            }

            return new OkObjectResult(new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Data ranking berhasil didapatkan",
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
                    Code = StatusCodes.Status404NotFound,
                    Status = "Not Found",
                    Message = "Data tidak ditemukan"
                });
            }

            return new OkObjectResult(new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Data ranking berhasil didapatkan",
                Data = rankings
            });
        }

    }
}
