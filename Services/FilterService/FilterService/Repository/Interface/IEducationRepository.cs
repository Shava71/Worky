using FilterService.Data;
using FilterService.Models;

namespace FilterService.Repository;

public interface IEducationRepository
{
   public Task<List<Education?>> GetAllEducations();
}