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
        [AllowAnonymous]
        public async Task<IActionResult> GetAllOption([FromQuery] QueryParams? queryParams)
        {
            var Options = await this.optionRepository.GetAllAsync(queryParams);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = Options
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOption(int id)
        {
            var Option = await this.optionRepository.GetAsync(id);
            if (Option == null)
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
                Data = Option
            });
        }

        [HttpPost]
        [Authorize(Policy = "Teacher")]
        public async Task<IActionResult> CreateOption([FromBody] Option Option)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await this.optionRepository.PostAsync(Option);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = this.mapper.Map<Option, OptionDto>(Option)
            });
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
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            existingOption.OptionText = updatedOption.OptionText;
            existingOption.IsAnswer = updatedOption.IsAnswer;
            existingOption.FkQuestionId = updatedOption.FkQuestionId;
            //this.mapper.Map(updatedOption, existingOption);

            await this.optionRepository.PutAsync(existingOption);

            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = this.mapper.Map<Option, OptionDto>(existingOption)
            });
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
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Data tidak ditemukan"
                });
            }

            await this.optionRepository.DeleteAsync(Option.Id);
            
            return new OkObjectResult(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = this.mapper.Map<Option, OptionDto>(Option)
            });
        }
    }
}
