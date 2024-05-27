using Microsoft.AspNetCore.Mvc;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;

namespace ServiceLearningApp.Interfaces
{
    public interface IQuestionRepository : IGenericRepository<Question>
    {

    }
}
