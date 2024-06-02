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
    [Route("api/exercise-transaction")]
    [Authorize(Policy = "Bearer")]
    public class ExerciseTransactionController : Controller
    {
        private readonly IExerciseTransactionRepository ExerciseTransactionRepository;
        private readonly IMapper mapper;

        public ExerciseTransactionController(IExerciseTransactionRepository ExerciseTransactionRepository, IMapper mapper)
        {
            this.ExerciseTransactionRepository = ExerciseTransactionRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllExerciseTransaction([FromQuery] QueryParams? queryParams)
        {
            var ExerciseTransactions = await this.ExerciseTransactionRepository.GetAllAsync(queryParams);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = ExerciseTransactions
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetExerciseTransaction(int id)
        {
            var ExerciseTransaction = await this.ExerciseTransactionRepository.GetAsync(id);
            if (ExerciseTransaction == null)
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
                Data = ExerciseTransaction
            });
        }

        [HttpPost]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> CreateExerciseTransaction([FromBody] ExerciseTransaction ExerciseTransaction)
        {
            await this.ExerciseTransactionRepository.PostAsync(ExerciseTransaction);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = this.mapper.Map<ExerciseTransaction, ExerciseTransactionDto>(ExerciseTransaction)
            });
        }

        //[HttpPut("{id}")]
        //[Authorize(Policy = "Student")]
        //public async Task<IActionResult> UpdateExerciseTransaction(int id, [FromBody] ExerciseTransaction updatedExerciseTransaction)
        //{
        //    var existingExerciseTransaction = await this.ExerciseTransactionRepository.GetAsync(id);

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

        //    await this.ExerciseTransactionRepository.PutAsync(existingExerciseTransaction);

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
            var ExerciseTransaction = await this.ExerciseTransactionRepository.GetAsync(id);
            if (ExerciseTransaction == null)
            {
                return new BadRequestObjectResult(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            await this.ExerciseTransactionRepository.DeleteAsync(ExerciseTransaction.Id);
            
            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = this.mapper.Map<ExerciseTransaction, ExerciseTransactionDto>(ExerciseTransaction)
            });
        }
    }
}
