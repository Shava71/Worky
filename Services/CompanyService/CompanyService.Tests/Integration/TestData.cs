using CompanyService.DAL.HttpClients.Clients;

namespace CompanyService.Tests.Integration;

public static class TestData
{
    public static List<TypeOfActivityResponse> Activities => new()
    {
        new TypeOfActivityResponse
        {
            id = 1,
            direction = "IT",
            type = "Backend"
        },
        new TypeOfActivityResponse
        {
            id = 2,
            direction = "IT",
            type = "Frontend"
        },
        new TypeOfActivityResponse
        {
            id = 3,
            direction = "HR",
            type = "Recruitment"
        }
    };
}