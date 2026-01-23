using FilterService.Data;
using FilterService.Models;
using Microsoft.EntityFrameworkCore;

namespace FilterService.Repository;

public class EducationRepository : IEducationRepository
{
    private readonly ApplicationDbContext _context;

    public EducationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Education?>> GetAllEducations()
    {
        return await _context.Educations.ToListAsync();
    }
}