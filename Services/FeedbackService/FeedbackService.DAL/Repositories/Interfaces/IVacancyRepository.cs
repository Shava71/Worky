using FeedbackService.DAL.Entities;

namespace FeedbackService.DAL.Repositories.Interfaces;

public interface IVacancyRepository
{
    Task<Guid> AddVacancyAsync(Vacancy vacancy);
    Task DeleteVacancyAsync(Guid vacancyId);
    Task<Vacancy> GetVacacnyAsync(Guid vacancyId);
}