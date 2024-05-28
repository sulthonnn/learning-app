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
    public class ChapterRepository : IChapterRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ChapterRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IReadOnlyList<Chapter>> GetAllAsync(QueryParams? queryParams)
        {
            IQueryable<Chapter> query = this.dbContext.Chapters;

            // Filtering&Sorting
            query = ApplyFilterAndSort(query, queryParams);
            // Pagination
            query = ApplyPagination(query, queryParams);            

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Chapter> GetAsync(int id)
        {
            return await this.dbContext.Chapters
                .AsNoTracking()
                .FirstAsync(c => c.Id == id);
        }

        public async Task PostAsync(Chapter entity)
        {
            await this.dbContext.Chapters.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await this.dbContext.Chapters.CountAsync();
        }

        public async Task PutAsync(Chapter entity)
        {
            this.dbContext.Entry(entity).State = EntityState.Modified;
            await this.dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var chapter = await this.dbContext.Chapters.FindAsync(id);
            if (chapter != null)
            {
                this.dbContext.Chapters.Remove(chapter);
                await this.dbContext.SaveChangesAsync();
            }
        }

        private IQueryable<Chapter> ApplyFilterAndSort(IQueryable<Chapter> query, QueryParams? queryParams)
        {
            if (queryParams == null)
                return query;

            // Filtering
            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query = query.Where(e => EF.Functions.ILike(e.Title, "%" + queryParams.Search + "%"));
            }

            // Sorting
            if (!string.IsNullOrEmpty(queryParams.Sort))
            {
                query = queryParams.Sort switch
                {
                    "title" => query.OrderBy(e => e.Title),
                    "-title" => query.OrderByDescending(e => e.Title),
                    _ => query.OrderBy(e => e.Id),
                };
            }

            return query;
        }

        private IQueryable<Chapter> ApplyPagination(IQueryable<Chapter> query, QueryParams? queryParams)
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
