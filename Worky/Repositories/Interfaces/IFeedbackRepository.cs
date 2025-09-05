using Worky.Contracts;
using Worky.DTO;
using Worky.Migrations;

namespace Worky.Repositories.Interfaces;

public interface IFeedbackRepository
{
    Task<IEnumerable<FeedbackDtos>> GetFeedbacksAsync(string userId, ulong? vacancyId = null, ulong? resumeId = null);
    Task<ulong> CreateFeedbackAsync(MakeFeedbackRequest request, string creator1, string creator2);
    Task DeleteFeedbackAsync(ulong id);
    Task UpdateFeedbackStatusAsync(ulong id, FeedbackStatus status);
}