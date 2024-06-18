using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceLearningApp.Model;
using System.Text.Json;
using System.IO;

namespace ServiceLearningApp.Data
{
    public class DbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext dbContext)
        {

            var basePath = AppContext.BaseDirectory; // Get the current application directory

            if (!dbContext.Chapters.Any())
            {
                var chaptersPath = Path.Combine(basePath, "Data", "Seed", "Chapter.json");
                var chaptersData = File.ReadAllText(chaptersPath);
                var chapters = JsonSerializer.Deserialize<List<Chapter>>(chaptersData);
                dbContext.Chapters.AddRange(chapters);
            }

            if (!dbContext.SubChapters.Any())
            {
                var subChaptersPath = Path.Combine(basePath, "Data", "Seed", "SubChapter.json");
                var subChaptersData = File.ReadAllText(subChaptersPath);
                var subChapters = JsonSerializer.Deserialize<List<SubChapter>>(subChaptersData);
                dbContext.SubChapters.AddRange(subChapters);
            }

            if (!dbContext.Questions.Any())
            {
                var questionsPath = Path.Combine(basePath, "Data", "Seed", "Question.json");
                var questionsData = File.ReadAllText(questionsPath);
                var questions = JsonSerializer.Deserialize<List<Question>>(questionsData);
                dbContext.Questions.AddRange(questions);
            }

            if (!dbContext.Options.Any())
            {
                var optionsPath = Path.Combine(basePath, "Data", "Seed", "Option.json");
                var optionsData = File.ReadAllText(optionsPath);
                var options = JsonSerializer.Deserialize<List<Option>>(optionsData);
                dbContext.Options.AddRange(options);
            }

            if (dbContext.ChangeTracker.HasChanges()) await dbContext.SaveChangesAsync();
        }

        private static async Task ResetTableAsync<T>(ApplicationDbContext dbContext, string tableName) where T : class
        {
            // Delete existing data
            dbContext.Set<T>().RemoveRange(dbContext.Set<T>());
            await dbContext.SaveChangesAsync();

            // Reset identity column seed value
            var resetIdentitySql = $"DBCC CHECKIDENT ('[{tableName}]', RESEED, 1);";

            await dbContext.Database.ExecuteSqlRawAsync(resetIdentitySql);
        }
    }
}
