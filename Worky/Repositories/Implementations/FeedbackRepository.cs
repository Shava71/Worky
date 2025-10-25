using System.Data;
using Dapper;
using MySqlConnector;
using Worky.Context;
using Worky.Contracts;
using Worky.DTO;
using Worky.Migrations;
using Worky.Repositories.Interfaces;

namespace Worky.Repositories.Implementations;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly WorkyDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public FeedbackRepository(WorkyDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<IEnumerable<FeedbackDtos>> GetFeedbacksAsync(string userId, ulong? vacancyId = null, ulong? resumeId = null)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sql = "SELECT * FROM Feedbacks WHERE 1=1";
                if (vacancyId.HasValue) sql += " AND vacancy_id = @vacancyId";
                if (resumeId.HasValue) sql += " AND resume_id = @resumeId";
                // Adapt for userId if needed, e.g., join with vacancies or resumes to filter by user

                var feedbacks = await db.QueryAsync<Feedback>(sql, new { vacancyId, resumeId });
                return feedbacks.Select(f => new FeedbackDtos { id = f.id, resume_id = f.resume_id, vacancy_id = f.vacancy_id, status = f.status });
            }
        }

        public async Task<ulong> CreateFeedbackAsync(MakeFeedbackRequest request, string creator1, string creator2)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (IDbConnection db = new MySqlConnection(connectionString))
            {
                string sql = "INSERT INTO Feedbacks (resume_id, vacancy_id, status, createdBy1, createdBy2) VALUES (@resume_id, @vacancy_id, @status, @createdBy1, @createdBy2); SELECT LAST_INSERT_ID();";
                return await db.ExecuteScalarAsync<ulong>(sql, new
                {
                    request.resume_id,
                    request.vacancy_id,
                    status = FeedbackStatus.InProgress.ToString(),
                    createdBy1 = creator1,
                    createdBy2 = creator2
                });
            }
        }

        public async Task DeleteFeedbackAsync(ulong id)
        {
            var feedback = await _dbContext.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _dbContext.Feedbacks.Remove(feedback);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateFeedbackStatusAsync(ulong id, FeedbackStatus status)
        {
            var feedback = await _dbContext.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                feedback.status = status;
                _dbContext.Feedbacks.Update(feedback);
                await _dbContext.SaveChangesAsync();
            }
        }
}