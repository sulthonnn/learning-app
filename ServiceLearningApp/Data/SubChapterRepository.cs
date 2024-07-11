using Microsoft.EntityFrameworkCore;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ServiceLearningApp.Helpers;

namespace ServiceLearningApp.Data
{
    public class SubChapterRepository : ISubChapterRepository
    {
        private readonly ApplicationDbContext dbContext;

        public SubChapterRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IReadOnlyList<SubChapter>> GetAllAsync(QueryParams? queryParams)
        {
            IQueryable<SubChapter> query = this.dbContext.SubChapters;

            query = ApplyFilterAndSort(query, queryParams);
            query = ApplyPagination(query, queryParams);

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SubChapter> GetAsync(int id)
        {
            return await this.dbContext.SubChapters
                .Include(e => e.Upload)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task PostAsync(SubChapter entity)
        {

            await this.dbContext.SubChapters.AddAsync(entity);

            if (entity.FkUploadId == 0 || entity.FkUploadId == null)
                throw new ArgumentNullException();

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await this.dbContext.SubChapters.CountAsync();
        }

        public async Task PutAsync(SubChapter entity)
        {
            this.dbContext.Entry(entity).State = EntityState.Modified;
            await this.dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var SubChapter = await this.dbContext.SubChapters.FindAsync(id);
            if (SubChapter != null)
            {
                this.dbContext.SubChapters.Remove(SubChapter);
                await this.dbContext.SaveChangesAsync();
            }
        }

        private IQueryable<SubChapter> ApplyFilterAndSort(IQueryable<SubChapter> query, QueryParams? queryParams)
        {
            if (queryParams == null)
                return query;

            // Filtering
            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query = query.Where(e => EF.Functions.ILike(e.Title, "%" + queryParams.Search + "%"));
            }

            if (queryParams.ChapterId.HasValue)
            {
                query = query.Where(e => e.FkChapterId == queryParams.ChapterId);
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

        private IQueryable<SubChapter> ApplyPagination(IQueryable<SubChapter> query, QueryParams? queryParams)
        {
            query = query
               .Include(e => e.Chapter)
               .Include(e => e.Upload);

            // Pagination
            if (queryParams.Page > 0 && queryParams.PerPage > 0)
            {
                query = query.Skip((queryParams.Page - 1) * queryParams.PerPage).Take(queryParams.PerPage);
            }

            return query;
        }
    }
}
