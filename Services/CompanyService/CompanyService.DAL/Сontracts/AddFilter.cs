namespace CompanyService.DAL.Contracts;

public record AddFilter(
    Guid id,
    List<int> typeOfActivity_id
);