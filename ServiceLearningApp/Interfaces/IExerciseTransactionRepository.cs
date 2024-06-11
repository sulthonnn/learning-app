using ServiceLearningApp.Model;

namespace ServiceLearningApp.Interfaces
{
    public interface IExerciseTransactionRepository : IGenericRepository<ExerciseTransaction>
    {
        Task PostHistoryAnswerAsync(List<HistoryAnswer> historyAnswer);
        Task<IReadOnlyList<HistoryAnswer>> GetHistoryAnswerByExerciseId(int id);

    }
}
