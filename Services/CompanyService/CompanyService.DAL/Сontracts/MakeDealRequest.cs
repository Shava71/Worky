namespace CompanyService.DAL.Contracts;

public record MakeDealRequest(
    int tarrif_id,
    int countMonth
    );