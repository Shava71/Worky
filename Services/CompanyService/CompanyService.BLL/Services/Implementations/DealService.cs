using CompanyService.BLL.Services.Interfaces;
using CompanyService.DAL.Contracts;
using CompanyService.DAL.Entities;
using CompanyService.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace CompanyService.BLL.Services.Implementations;

public class DealService : IDealService
{
    private readonly IDealRepository _dealRepository;
    private readonly ITariffRepository _tariffRepository;
    private readonly ILogger<DealService> _logger;

    public DealService(IDealRepository dealRepository, ITariffRepository tariffRepository, ILogger<DealService> logger)
    {
        _dealRepository = dealRepository;
        _tariffRepository = tariffRepository;
        _logger = logger;
    }
    
    public async Task<Guid> CreateDeal(MakeDealRequest request, Guid company_id, CancellationToken cancellationToken = default)
    {
        Tarrif tariff = await _tariffRepository.GetTariff(request.tarrif_id, cancellationToken);
        if (tariff == null)
        {
            _logger.LogError("tariff with id = {0} now found", request.tarrif_id);
            throw new ApplicationException("Tarrif not found");
        }
        
        DateTime dateTime = DateTime.UtcNow.Date;
        DateOnly currentDate = DateOnly.FromDateTime(dateTime);

        Deal? currentDeal = await _dealRepository.CurrentActiveDealAsync(currentDate, company_id, cancellationToken);
        if (currentDeal is not null)
        {
            _logger.LogError("Deal {0} is active now with tariff {1}", currentDeal.id, currentDeal.tariff_id);
            throw new ApplicationException("Invalid deal");
        }
        
        Deal newDeal = new Deal()
        {
            id = Guid.NewGuid(),
            tariff_id = tariff.id,
            company_id = company_id,
            date_start = currentDate,
            date_end = currentDate.AddMonths(request.countMonth),
            sum = tariff.price * request.countMonth
        };
        
        return await _dealRepository.CreateDeal(newDeal, cancellationToken);
    }
    

    public async Task<Tarrif?> GetTariff(int? dealId, CancellationToken cancellationToken = default)
    {
        return await _tariffRepository.GetTariff(dealId, cancellationToken);
    }

    public async Task<List<Tarrif>> GetTariff(CancellationToken cancellationToken = default)
    {
        return await _tariffRepository.GetTariff(cancellationToken);
    }
}