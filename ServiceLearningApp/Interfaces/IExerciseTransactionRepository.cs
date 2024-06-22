using ServiceLearningApp.Helpers;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;

namespace ServiceLearningApp.Interfaces
{
    public interface IExerciseTransactionRepository : IGenericRepository<ExerciseTransaction>
    {
        Task<IReadOnlyList<ExerciseTransactionDto>> GetAllAsyncDto(QueryParams? queryParams);

        //Task<ExerciseTransactionDto> GetAsyncDto(int id);
        Task PostHistoryAnswerAsync(List<HistoryAnswer> historyAnswer);
        Task<IReadOnlyList<HistoryAnswer>> GetHistoryAnswerByExerciseId(int id);
        Task<IReadOnlyList<RankingDto>> GetRankAsync(QueryParams queryParams);
        Task<IReadOnlyList<AllRankingDto>> GetAllRankingAsync(QueryParams queryParams);



    }
}
