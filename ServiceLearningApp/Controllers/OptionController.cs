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
    [Route("api/v1/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class OptionController : Controller
    {
        private readonly IOptionRepository optionRepository;
        private readonly IMapper mapper;

        public OptionController(IOptionRepository OptionRepository, IMapper mapper)
        {
            this.optionRepository = OptionRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> GetAllOption([FromQuery] QueryParams? queryParams)
        {
            var Options = await this.optionRepository.GetAllAsync(queryParams);

            return new OkObjectResult(new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Data opsi berhasil didapatkan",
                Data = Options
            });
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> GetOption(int id)
        {
            var Option = await this.optionRepository.GetAsync(id);
            if (Option == null)
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
                Message = "Data opsi berhasil didapatkan",
                Data = Option
            });
        }

        [HttpPost]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> CreateOption([FromBody] Option Option)
        {
            try
            {
                await this.optionRepository.PostAsync(Option);

                return new OkObjectResult(new
                {
                    Code = StatusCodes.Status201Created,
                    Status = "Created",
                    Message = "Data opsi berhasil dibuat",
                    Data = this.mapper.Map<Option, OptionDto>(Option)
                });
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = "Bad Request",
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> UpdateOption(int id, [FromBody] Option updatedOption)
        {
            var existingOption = await this.optionRepository.GetAsync(id);

            if (existingOption == null)
            {
                return new BadRequestObjectResult(new
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = "Not Found",
                    Message = "Data tidak ditemukan"
                });
            }

            try
            {
                existingOption.OptionText = updatedOption.OptionText;
                existingOption.IsAnswer = updatedOption.IsAnswer;
                existingOption.FkQuestionId = updatedOption.FkQuestionId;

                await this.optionRepository.PutAsync(existingOption);

                return new OkObjectResult(new
                {
                    Code = StatusCodes.Status200OK,
                    Status = "Ok",
                    Message = "Data opsi berhasil diubah",
                    Data = this.mapper.Map<Option, OptionDto>(existingOption)
                });
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = "Bad Request",
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> DeleteOption(int id)
        {
            var Option = await this.optionRepository.GetAsync(id);
            if (Option == null)
            {
                return new BadRequestObjectResult(new
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = "Not Found",
                    Message = "Data tidak ditemukan"
                });
            }

            await this.optionRepository.DeleteAsync(Option.Id);
            
            return new OkObjectResult(new
            {
                Code = StatusCodes.Status200OK,
                Status = "Ok",
                Message = "Data opsi berhasil dihapus",
                Data = this.mapper.Map<Option, OptionDto>(Option)
            });
        }
    }
}
