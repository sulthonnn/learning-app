using Microsoft.EntityFrameworkCore;
using ServiceLearningApp.Interfaces;
using ServiceLearningApp.Model;
using ServiceLearningApp.Helpers;

namespace ServiceLearningApp.Data
{
    public class OptionRepository : IOptionRepository
    {
        private readonly ApplicationDbContext dbContext;

        public OptionRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IReadOnlyList<Option>> GetAllAsync(QueryParams? queryParams)
        {
            IQueryable<Option> query = this.dbContext.Options;

            // Filtering&Sorting
            query = ApplyFilterAndSort(query, queryParams);
            // Pagination
            query = ApplyPagination(query, queryParams);            

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Option> GetAsync(int id)
        {
            return await this.dbContext.Options
                .Include(e => e.Question)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task PostAsync(Option entity)
        {
            if (entity.IsAnswer)
            {
                var isAnswerExists = await this.dbContext.Options
                    .Where(e => e.FkQuestionId == entity.FkQuestionId && e.IsAnswer)
                    .AnyAsync();

                if (isAnswerExists)
                {
                    throw new BadHttpRequestException("Opsi jawaban benar sudah ada");
                }
            }

            await this.dbContext.Options.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await this.dbContext.Options.CountAsync();
        }

        public async Task PutAsync(Option entity)
        {
            if (entity.IsAnswer)
            {
                var isAnswerExists = await this.dbContext.Options
                    .Where(e => e.FkQuestionId == entity.FkQuestionId && e.IsAnswer)
                    .AnyAsync();

                if (isAnswerExists)
                {
                    throw new BadHttpRequestException("Opsi jawaban benar sudah ada");
                }
            }

            this.dbContext.Entry(entity).State = EntityState.Modified;
            await this.dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var Option = await this.dbContext.Options.FindAsync(id);
            if (Option != null)
            {
                this.dbContext.Options.Remove(Option);
                await this.dbContext.SaveChangesAsync();
            }
        }

        private IQueryable<Option> ApplyFilterAndSort(IQueryable<Option> query, QueryParams? queryParams)
        {
            query = query.Include(e => e.Question);

            // Filtering
            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query = query.Where(e => EF.Functions.ILike(e.OptionText, "%" + queryParams.Search + "%"));
            }

            if (queryParams.QuestionId.HasValue)
            {
                query = query.Where(e => e.FkQuestionId == queryParams.QuestionId);
            }

            // Sorting
            if (!string.IsNullOrEmpty(queryParams.Sort))
            {
                query = queryParams.Sort switch
                {
                    "option" => query.OrderBy(e => e.OptionText),
                    "-option" => query.OrderByDescending(e => e.OptionText),
                    _ => query.OrderBy(e => e.Id),
                };
            }

            return query;
        }

        private IQueryable<Option> ApplyPagination(IQueryable<Option> query, QueryParams? queryParams)
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
