using Microsoft.EntityFrameworkCore;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ServiceLearningApp.Helpers;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Microsoft.AspNetCore.Mvc;
using ServiceLearningApp.Model.Dto;

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
        public async Task<IReadOnlyList<ExerciseTransactionDto>> GetAllAsyncDto(QueryParams? queryParams)
        {
            // Ambil query dari DbContext
            IQueryable<ExerciseTransaction> query = this.dbContext.ExerciseTransactions
                .Include(e => e.SubChapter)
                .Include(e => e.User)
                .Include(e => e.HistoryAnswer);

            // Filtering & Sorting
            query = ApplyFilterAndSort(query, queryParams);
            query = ApplyPagination(query, queryParams);

            // Projection ke ExerciseTransactionDto
            var exerciseTransactions = await query
                .Select(e => new ExerciseTransactionDto
                {
                    Id = e.Id,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    CorrectAnswer = e.CorrectAnswer,
                    IncorrectAnswer = e.IncorrectAnswer,
                    Score = e.Score,
                    SubChapter = e.SubChapter.Title,
                    UserFullName = e.User.FullName,
                    HistoryAnswer = e.HistoryAnswer.Select(h => new HistoryAnswerDto
                    {
                        FkQuestionId = h.FkQuestionId,
                        FkOptionId = h.FkOptionId,
                        FkExerciseTransactionId = h.FkExerciseTransactionId
                    }).ToList()
                })
                .AsNoTracking()
                .ToListAsync();

            return exerciseTransactions;
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

        public async Task<IReadOnlyList<RankingDto>> GetRankAsync(QueryParams queryParams)
        {
            IQueryable<ExerciseTransaction> query = this.dbContext.ExerciseTransactions
                .Include(e => e.SubChapter)
                .Include(e => e.User);

            query = ApplyFilterAndSort(query, queryParams);
            query = ApplyPagination(query, queryParams);

            // Grouping berdasarkan FkUserId dan SubChapterId, lalu mengambil nilai tertinggi untuk setiap subchapter
            var maxScoreTransactionsBySubChapter = await query
                .GroupBy(e => new { e.FkUserId, e.FkSubChapterId, e.SubChapter.FkChapterId })
                .Select(g => g.OrderByDescending(e => e.Score)
                             .ThenBy(e => (e.EndDate - e.StartDate).TotalSeconds)
                             .FirstOrDefault())
                .ToListAsync();

            // Grouping hasil berdasarkan FkUserId dan ChapterId, lalu menjumlahkan skor
            var userScoresByChapter = maxScoreTransactionsBySubChapter
                .GroupBy(e => new { e.FkUserId, e.SubChapter.FkChapterId })
                .Select(g => new
                {
                    User = g.First().User,
                    ChapterId = g.Key.FkChapterId,
                    TotalScore = g.Sum(e => e.Score),
                    TotalSeconds = g.Sum(e => (decimal)(e.EndDate - e.StartDate).TotalSeconds),
                    Transactions = g.ToList()
                })
                .ToList();

            // Projection ke RankingDto
            var rankings = userScoresByChapter
                .Select(u => new RankingDto
                {
                    ChapterId = u.ChapterId,
                    UserFullName = u.User.FullName,
                    TotalScore = u.TotalScore,
                    AverageTime = u.Transactions.Count > 0 ? (u.TotalSeconds / u.Transactions.Count) : 0m,
                    ExerciseTransactions = u.Transactions.Select(t => new ExerciseTransactionDto
                    {
                        Id = t.Id,
                        StartDate = t.StartDate,
                        EndDate = t.EndDate,
                        CorrectAnswer = t.CorrectAnswer,
                        IncorrectAnswer = t.IncorrectAnswer,
                        Score = t.Score,
                        SubChapter = t.SubChapter.Title,
                        UserFullName = t.User.FullName
                    }).ToList()
                })
                .ToList();

            // Sorting berdasarkan queryParams.Sort
            if (!string.IsNullOrEmpty(queryParams.Sort))
            {
                rankings = queryParams.Sort switch
                {
                    "score" => rankings.OrderBy(e => e.TotalScore).ToList(),
                    "-score" => rankings.OrderByDescending(e => e.TotalScore).ToList(),
                    "time" => rankings.OrderBy(e => e.AverageTime).ToList(),
                    "-time" => rankings.OrderByDescending(e => e.AverageTime).ToList(),
                    _ => rankings
                };
            }

            return rankings;
        }

        public async Task<IReadOnlyList<AllRankingDto>> GetAllRankingAsync(QueryParams queryParams)
        {
            IQueryable<ExerciseTransaction> query = this.dbContext.ExerciseTransactions
                .Include(e => e.SubChapter)
                .Include(e => e.User);

            query = ApplyFilterAndSort(query, queryParams);
            query = ApplyPagination(query, queryParams);

            // Grouping berdasarkan FkUserId dan SubChapterId, lalu mengambil nilai tertinggi untuk setiap subchapter
            var maxScoreTransactionsBySubChapter = await query
                .GroupBy(e => new { e.FkUserId, e.FkSubChapterId, e.SubChapter.FkChapterId })
                .Select(g => g.OrderByDescending(e => e.Score)
                             .ThenBy(e => (e.EndDate - e.StartDate).TotalSeconds)
                             .FirstOrDefault())
                .ToListAsync();

            // Grouping hasil berdasarkan FkUserId dan ChapterId, lalu menjumlahkan skor tiap subchapter
            var userScoresByChapter = maxScoreTransactionsBySubChapter
                .GroupBy(e => new { e.FkUserId, e.SubChapter.FkChapterId })
                .Select(g => new
                {
                    User = g.First().User,
                    ChapterId = g.Key.FkChapterId,
                    TotalScore = g.Sum(e => e.Score),
                    TotalSeconds = g.Sum(e => (decimal)(e.EndDate - e.StartDate).TotalSeconds),
                    Transactions = g.ToList()
                })
                .ToList();

            // Grouping hasil berdasarkan FkUserId untuk mendapatkan total semua skor chapter
            var userTotalScores = userScoresByChapter
                .GroupBy(u => u.User.Id)
                .Select(g => new
                {
                    User = g.First().User,
                    TotalScore = g.Sum(x => x.TotalScore),
                    TotalSeconds = g.Sum(x => x.TotalSeconds),
                    Chapters = g.ToList()
                })
                .ToList();

            // Projection ke RankingDto
            var rankings = userTotalScores
                .Select(u => new AllRankingDto
                {
                    UserFullName = u.User.FullName,
                    TotalScore = u.TotalScore,
                    AverageTime = u.Chapters.Count > 0 ? (u.TotalSeconds / u.Chapters.Count) : 0m,
                    ExerciseTransactions = u.Chapters.SelectMany(c => c.Transactions.Select(t => new ExerciseTransactionDto
                    {
                        Id = t.Id,
                        StartDate = t.StartDate,
                        EndDate = t.EndDate,
                        CorrectAnswer = t.CorrectAnswer,
                        IncorrectAnswer = t.IncorrectAnswer,
                        Score = t.Score,
                        SubChapter = t.SubChapter.Title,
                        UserFullName = t.User.FullName
                    })).ToList()
                })
                .ToList();

            // Sorting berdasarkan queryParams.Sort
            if (!string.IsNullOrEmpty(queryParams.Sort))
            {
                rankings = queryParams.Sort switch
                {
                    "score" => rankings.OrderBy(e => e.TotalScore).ToList(),
                    "-score" => rankings.OrderByDescending(e => e.TotalScore).ToList(),
                    "time" => rankings.OrderBy(e => e.AverageTime).ToList(),
                    "-time" => rankings.OrderByDescending(e => e.AverageTime).ToList(),
                    _ => rankings
                };
            }

            return rankings;
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
                query = query.Where(e => EF.Functions.ILike(e.User.FullName, "%" + queryParams.Search + "%"));
            }

            if (queryParams.ChapterId.HasValue)
            {
                query = query.Where(e => e.SubChapter.FkChapterId == queryParams.ChapterId);
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
                    "time" => query.OrderBy(e => (e.EndDate - e.StartDate).TotalSeconds),
                    "-time" => query.OrderByDescending(e => e.Score),
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
