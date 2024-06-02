using Microsoft.AspNetCore.Mvc;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;

namespace ServiceLearningApp.Interfaces
{
    public interface IQuestionRepository : IGenericRepository<Question>
    {
        Task<List<Question>> GetRandomQuestionsBySubChapterIdAsync(int subChapterId, int count);
        Task<List<Question>> GetRandomQuestionsByChapterIdAsync(int chapterId, int count);

    }
}
