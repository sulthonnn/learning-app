using ServiceLearningApp.Helpers;
using ServiceLearningApp.Model;
using System.Linq.Expressions;

namespace ServiceLearningApp.Interfaces
{
    public interface IGenericRepository<TModel> where TModel : class
    {
        Task<TModel> GetAsync(int id);
        Task<IReadOnlyList<TModel>> GetAllAsync(QueryParams? queryParams);
        Task PostAsync(TModel entity);
        Task PutAsync(TModel entity);
        Task DeleteAsync(int id);
        Task<int> CountAsync();
    }
}
