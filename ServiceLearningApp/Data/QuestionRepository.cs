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
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext dbContext;

        public QuestionRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IReadOnlyList<Question>> GetAllAsync(QueryParams? queryParams)
        {
            IQueryable<Question> query = this.dbContext.Questions;

            // Filtering&Sorting
            query = ApplyFilterAndSort(query, queryParams);
            // Pagination
            query = ApplyPagination(query, queryParams);            

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Question> GetAsync(int id)
        {
            return await this.dbContext.Questions
                .Include(e => e.SubChapter)
                .Include(e => e.Image)
                .AsNoTracking()
                .FirstAsync(c => c.Id == id);
        }

        public async Task PostAsync(Question entity)
        {
            await this.dbContext.Questions.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await this.dbContext.Questions.CountAsync();
        }

        public async Task PutAsync(Question entity)
        {
            this.dbContext.Entry(entity).State = EntityState.Modified;
            await this.dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var Question = await this.dbContext.Questions.FindAsync(id);
            if (Question != null)
            {
                this.dbContext.Questions.Remove(Question);
                await this.dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Question>> GetRandomQuestionsBySubChapterIdAsync(int subChapterId, int count)
        {
            return await dbContext.Questions
                .Include(q => q.SubChapter)
                .Include(q => q.Image)
                .Where(q => q.FkSubChapterId == subChapterId)
                .OrderBy(r => EF.Functions.Random())
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Question>> GetRandomQuestionsByChapterIdAsync(int chapterId, int count)
        {
            return await dbContext.Questions
                .Include(q => q.SubChapter)
                .Include(q => q.Image)
                .Where(q => q.SubChapter.FkChapterId == chapterId)
                .OrderBy(r => EF.Functions.Random())
                .Take(count)
                .ToListAsync();
        }

        private IQueryable<Question> ApplyFilterAndSort(IQueryable<Question> query, QueryParams? queryParams)
        {
            query = query
                .Include(e => e.SubChapter)
                .Include(e => e.Image);

            // Filtering
            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query = query.Where(e => EF.Functions.ILike(e.QuestionText, "%" + queryParams.Search + "%"));
            }

            if (queryParams.FkSubChapterId.HasValue)
            {
                query = query.Where(e => e.FkSubChapterId == queryParams.FkQuestionId);
            }

            // Sorting
            if (!string.IsNullOrEmpty(queryParams.Sort))
            {
                query = queryParams.Sort switch
                {
                    "question" => query.OrderBy(e => e.QuestionText),
                    "-question" => query.OrderByDescending(e => e.QuestionText),
                    _ => query.OrderBy(e => e.Id),
                };
            }

            return query;
        }

        private IQueryable<Question> ApplyPagination(IQueryable<Question> query, QueryParams? queryParams)
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
