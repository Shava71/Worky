using SeachService.DAL.DTO;
using SearchService.DAL.DTO;
using SearchService.DAL.Entities;

namespace SearchService.BLL.Mapping;

public static class MappingExtensions
{
    public static ResumeDocument ToDocument(this ResumeDtos dto)
        => new()
        {
            id = dto.id,
            workerId = dto.worker_id!,
            skill = dto.skill ?? "",
            city = dto.city!,
            experience = dto.experience ?? 0,
            educationId = dto.education_id,
            educationName = dto.education_name,
            incomeDate = dto.income_date,
            wantedSalary = dto.wantedSalary,
            post = dto.post!,
            worker = new WorkerInfo
            {
                id = dto.worker!.id,
                firstName = dto.worker.first_name,
                secondName = dto.worker.second_name,
                surname = dto.worker.surname,
                birthday = dto.worker.birthday,
                age = dto.worker.age
            },
            activities = dto.activities?.Select(a => new Activity
            {
                id = a.id,
                direction = a.direction,
                type = a.type
            }).ToList() ?? new()
        };

    public static VacancyDocument ToDocument(this VacancyDtos dto)
        => new()
        {
            id = dto.id,
            companyId = dto.company_id ?? Guid.Empty,
            post = dto.post,
            description = dto.description ?? "",
            minSalary = dto.min_salary,
            maxSalary = dto.max_salary,
            experience = dto.experience,
            educationId = dto.education_id,
            educationName = dto.education_name,
            incomeDate = dto.income_date,
            workFormat = dto.work_format_name,
            workHour = dto.work_hour_name,
            location = dto.Location, 
            company = new CompanyInfo
            {
                id = dto.company?.id ?? Guid.Empty,
                name = dto.company?.name ?? "",
                phoneNumber = dto.company?.phoneNumber ?? "",
                email = dto.company?.email ?? "",
                website = dto.company?.website ?? "",
            },
            activities = dto.activities?.Select(a => new Activity
            {
                id = a.id,
                direction = a.direction,
                type = a.type
            }).ToList() ?? new()
        };
}