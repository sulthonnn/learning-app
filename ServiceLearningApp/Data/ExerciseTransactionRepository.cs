using Microsoft.EntityFrameworkCore;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ServiceLearningApp.Helpers;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace ServiceLearningApp.Data
{
    public class ExerciseTransactionRepository : IExerciseTransactionRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ExerciseTransactionRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IReadOnlyList<ExerciseTransaction>> GetAllAsync(QueryParams? queryParams)
        {
            IQueryable<ExerciseTransaction> query = this.dbContext.ExerciseTransactions;

            // Filtering&Sorting
            query = ApplyFilterAndSort(query, queryParams);
            // Pagination
            query = ApplyPagination(query, queryParams);            

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ExerciseTransaction> GetAsync(int id)
        {
            return await this.dbContext.ExerciseTransactions
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task PostAsync(ExerciseTransaction entity)
        {
            entity.Score = CalculateScore(entity.CorrectAnswer, entity.IncorrectAnswer);
            await this.dbContext.ExerciseTransactions.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await this.dbContext.ExerciseTransactions.CountAsync();
        }

        public async Task PutAsync(ExerciseTransaction entity)
        {
            this.dbContext.Entry(entity).State = EntityState.Modified;
            await this.dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ExerciseTransaction = await this.dbContext.ExerciseTransactions.FindAsync(id);
            if (ExerciseTransaction != null)
            {
                this.dbContext.ExerciseTransactions.Remove(ExerciseTransaction);
                await this.dbContext.SaveChangesAsync();
            }
        }

        public async Task PostHistoryAnswerAsync(List<HistoryAnswer> historyAnswers)
        {
            if (historyAnswers == null)
                throw new ArgumentNullException(nameof(historyAnswers));

            this.dbContext.HistoryAnswers.AddRange(historyAnswers);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<HistoryAnswer>> GetHistoryAnswerByExerciseId(int id)
        {
            return await this.dbContext.HistoryAnswers
                .Include(e => e.Question)
                .Include(e => e.Option)
                .Include(e => e.ExerciseTransaction)
                .Where(e => e.FkExerciseTransactionId == id)
                .AsNoTracking()
                .ToListAsync();
        }

        private int CalculateScore(int correctAnswer, int incorrectAnswer)
        {
            var totalQuestion = correctAnswer + incorrectAnswer;
            if (totalQuestion == 0) return 0; 

            double score = (double)correctAnswer / totalQuestion * 100;

            return (int)Math.Round(score);
        }


        private IQueryable<ExerciseTransaction> ApplyFilterAndSort(IQueryable<ExerciseTransaction> query, QueryParams? queryParams)
        {

            // Filtering
            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query = query.Where(e => EF.Functions.ILike(e.User.UserName, "%" + queryParams.Search + "%"));
            }

            // Sorting
            if (!string.IsNullOrEmpty(queryParams.Sort))
            {
                query = queryParams.Sort switch
                {
                    "startDate" => query.OrderBy(e => e.StartDate),
                    "-startDate" => query.OrderByDescending(e => e.StartDate),
                    "score" => query.OrderBy(e => e.Score),
                    "-score" => query.OrderByDescending(e => e.Score),
                    _ => query.OrderBy(e => e.Id),
                };
            }

            return query;
        }

        private IQueryable<ExerciseTransaction> ApplyPagination(IQueryable<ExerciseTransaction> query, QueryParams? queryParams)
        {
            if (queryParams == null)
                return query;

            // Pagination
            if (queryParams.Page > 0 && queryParams.PerPage > 0)
            {
                query = query.Skip((queryParams.Page - 1) * queryParams.PerPage).Take(queryParams.PerPage);
            }  

            return query;
        }
    }
}
