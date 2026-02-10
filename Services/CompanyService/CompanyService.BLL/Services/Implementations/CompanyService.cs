using System.Globalization;
using CompanyService.BLL.Services.Http.Interfaces;
using CompanyService.BLL.Services.Interfaces;
using CompanyService.DAL.Clients;
using CompanyService.DAL.Contracts;
using CompanyService.DAL.DTO;
using CompanyService.DAL.Entities;
using CompanyService.DAL.Events;
using CompanyService.DAL.HttpClients.Clients;
using CompanyService.DAL.Models;
using CompanyService.DAL.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZXing;
using ZXing.QrCode;
using ZXing.Rendering;


namespace CompanyService.BLL.Services.Implementations;

public class CompanyService : ICompnayService
{
        private readonly IVacancyRepository _vacancyRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IDealRepository _dealRepository;
        private readonly ILogger<CompanyService> _logger;
        private readonly IAuthClient _authClient;
        private readonly IFilterCacheService _filterCacheService;
        
        private readonly ITopicProducer<VacancyCreatedEvent> _vacancyCreatedTopicProducer;
        private readonly ITopicProducer<VacancyUpdatedEvent> _vacancyUpdatedTopicProducer;
        private readonly ITopicProducer<VacancyDeletedEvent> _vacancyDeletedTopicProducer;
        
        private readonly ITopicProducer<VacancyFilterAddEvent> _vacancyFilterAddTopicProducer;
        private readonly ITopicProducer<VacancyFilterDeleteEvent> _vacancyFilterDeleteTopicProducer;

        public CompanyService(
            IVacancyRepository vacancyRepository,
            ICompanyRepository companyRepository, 
            ILogger<CompanyService> logger,
            IAuthClient authClient,
            IFilterCacheService filterCacheService,
            IDealRepository dealRepository,
            
            ITopicProducer<VacancyCreatedEvent> vacancyCreatedTopicProducer,
            ITopicProducer<VacancyUpdatedEvent> vacancyUpdatedTopicProducer,
            ITopicProducer<VacancyDeletedEvent> vacancyDeletedTopicProducer,
            
            ITopicProducer<VacancyFilterAddEvent> vacancyFilterAddTopicProducer,
            ITopicProducer<VacancyFilterDeleteEvent> vacancyFilterDeleteTopicProducer)
        {
            _vacancyRepository = vacancyRepository;
            _companyRepository = companyRepository;
            _logger = logger;
            _authClient = authClient;
            _filterCacheService = filterCacheService;
            _dealRepository = dealRepository;
            
            _vacancyCreatedTopicProducer = vacancyCreatedTopicProducer;
            _vacancyUpdatedTopicProducer = vacancyUpdatedTopicProducer;
            _vacancyDeletedTopicProducer = vacancyDeletedTopicProducer;
            
            _vacancyFilterAddTopicProducer = vacancyFilterAddTopicProducer;
            _vacancyFilterDeleteTopicProducer = vacancyFilterDeleteTopicProducer;
        }

        public async Task<VacancyDtos> GetVacancyInfoAsync(Guid vacancyId)
        {
            return await BuildFullVacancyAsync(vacancyId);
        }

        public async Task<IEnumerable<VacancyDtos>> GetMyVacanciesAsync(Guid companyId, Guid? vacancyId)
        {
            IEnumerable<VacancyDtos> vacancies = await _vacancyRepository.GetMyVacanciesAsync(companyId.ToString(), vacancyId);
            List<VacancyDtos> vacancyList = vacancies.ToList();

            if (!vacancyList.Any())
                return vacancyList;
            
            List<int> allActivityIds = vacancyList
                .SelectMany(v => v.activities.Select(a => a.id))
                .Distinct()
                .ToList();

            if (allActivityIds.Any())
            {
                List<TypeOfActivityResponse> activities = await _filterCacheService.GetFiltersByIdsAsync(allActivityIds);

                Dictionary<int , TypeOfActivityResponse> activityDict = activities.ToDictionary(a => a.id, a => a);

                // foreach (VacancyDtos vacancy in vacancyList)
                // {
                //     vacancy.activities = vacancy.activities
                //         .Where(a => activityDict.ContainsKey(a.id))
                //         .Select(a => activityDict[a.id])
                //         .ToList();
                // }
                foreach (VacancyDtos vacancy in vacancyList)
                {
                    vacancy.activities = vacancy.activities
                        .Where(a => activityDict.ContainsKey(a.id))
                        .Select(a =>
                        {
                            var cached = activityDict[a.id];

                            return new TypeOfActivityResponse
                            {
                                id = cached.id,
                                direction = cached.direction,
                                type = cached.type,
                                
                                filter_id = a.filter_id
                            };
                        })
                        .ToList();
                }
            }

            Company company = await _companyRepository.GetCompanyByIdAsync(companyId);
            CompanyDto companyDto = new CompanyDto
            {
                id = company.UserId,
                email = company.email,
                longitude = company.longitude,
                latitude = company.latitude,
                name = company.name,
                phoneNumber = company.phoneNumber,
                website = company.website,
            };

            foreach (VacancyDtos vacancy in vacancyList)
            {
                vacancy.company = companyDto;
            }

            return vacancyList;
        }

        public async Task<Guid> CreateVacancyAsync(CreateVacancy vacancy, string companyId)
        {
            // return await _vacancyRepository.CreateVacancyAsync(vacancy, companyId);
            int currentVacanciesCount = await _vacancyRepository.GetMyVacanciesCountAsync(Guid.Parse(companyId));
            
            DateTime dateTime = DateTime.UtcNow.Date;
            DateOnly currentDate = DateOnly.FromDateTime(dateTime);
            Deal? currentDeal = await _dealRepository.CurrentActiveDealAsync(currentDate, Guid.Parse(companyId));

            if (currentDeal is null)
            {
                throw new Exception("Deal not found");
            }

            if (currentVacanciesCount >= currentDeal.tariff.vacancy_count)
            {
                throw new Exception("Vacancy count exceeded");
            }
            
            Guid vacancyId = await _vacancyRepository.CreateVacancyAsync(vacancy, companyId);
            
            VacancyDtos fullVacancy = await BuildFullVacancyAsync(vacancyId);
            
            await _vacancyCreatedTopicProducer.Produce(new VacancyCreatedEvent(fullVacancy));
            _logger.LogInformation("VacancyCreatedEvent published for vacancy {VacancyId}", vacancyId);
            
            return vacancyId;
        }

        public async Task UpdateVacancyAsync(UpdateVacancy vacancy, string companyId)
        {
            try
            {
                if (!await CompanyHasVacancy(Guid.Parse(companyId), vacancy.Id))
                {
                    // throw KeyNotFoundException("");
                    return;
                }

                await _vacancyRepository.UpdateVacancyAsync(vacancy, Guid.Parse(companyId));

                VacancyDtos fullResume = await BuildFullVacancyAsync(vacancy.Id);
                await _vacancyUpdatedTopicProducer.Produce(new VacancyUpdatedEvent(fullResume));

                _logger.LogInformation("VacancyUpdatedEvent published for vacancy {VacancyId}", vacancy.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vacancy for {CompanyId}", companyId);
                throw;
            }
        }

        public async Task DeleteVacancyAsync(Guid id, string companyId)
        {
            if (!await CompanyHasVacancy(Guid.Parse(companyId), id))
            {
                // throw KeyNotFoundException("");
                return;
            }
            await _vacancyRepository.DeleteVacancyAsync(id, Guid.Parse(companyId));
            
            await _vacancyDeletedTopicProducer.Produce(new VacancyDeletedEvent(id));
            _logger.LogInformation("VacancyDeletedEvent published for vacancy {VacancyId}", id);
        }

        public async Task<IEnumerable<Guid>> AddVacancyFilterAsync(AddFilter filter, string companyId)
        {
            if (!await CompanyHasVacancy(Guid.Parse(companyId), filter.id))
            {
                // throw KeyNotFoundException("");
                return [];
            }
            
            List<TypeOfActivityResponse> activities = await _filterCacheService.GetFiltersByIdsAsync(filter.typeOfActivity_id);
            try
            {
                await _vacancyFilterAddTopicProducer.Produce(new VacancyFilterAddEvent(filter.id, activities));
                IEnumerable<Guid> filter_id = await _vacancyRepository.AddVacancyFiltersAsync(filter);

                if (!filter_id.Any())
                {
                    throw new KeyNotFoundException("Filter not found");
                }

                return filter_id;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding vacancy filter for {CompanyId}", companyId);
                return [];
            }
        }

        public async Task DeleteVacancyFilterAsync(Guid filterId, string companyId)
        {
            Vacancy_filter? vacancy_filter = await _vacancyRepository.GetVacancyFilterByIdAsync(filterId);
            if (vacancy_filter == null)
            {
                throw new KeyNotFoundException("Filter not found");
            }

            VacancyFilterDeleteEvent @event = new VacancyFilterDeleteEvent(
                vacancy_id: vacancy_filter.vacancy_id,
                activity_id: vacancy_filter.typeOfActivity_id
            );
            try
            {
                await _vacancyFilterDeleteTopicProducer.Produce(@event);
                await _vacancyRepository.DeleteVacancyFilterAsync(filterId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vacancy filter for {CompanyId}", companyId);
                return;
            }

        }

        // public async Task<object> GetStatisticsJsonAsync(string companyId, int start_year, int start_month, int end_year, int end_month)
        // {
        //     var startDate = new DateTime(start_year, start_month, 1);
        //     var endDay = DateTime.DaysInMonth(end_year, end_month);
        //     var endDate = new DateTime(end_year, end_month, endDay);
        //     var startOnly = DateOnly.FromDateTime(startDate);
        //     var endOnly = DateOnly.FromDateTime(endDate);
        //
        //     var vacancies = await _companyRepository.GetVacanciesByCompanyAsync(companyId);
        //     if (!vacancies.Any())
        //     {
        //         return new
        //         {
        //             VacancyCount = 0,
        //             TotalFeedbacks = 0,
        //             AcceptedFeedbacks = 0,
        //             RejectedFeedbacks = 0,
        //             AvgFeedbackPerVacancy = 0.0,
        //             Period = $"{startDate:MMMM yyyy} - {endDate:MMMM yyyy}",
        //             AcceptedWorkers = new List<WorkerDtos>()
        //         };
        //     }
        //
        //     var feedbacks = await _companyRepository.GetFeedbacksByVacanciesAsync(vacancies, startOnly, endOnly);
        //     var total = feedbacks.Count();
        //     var accepted = feedbacks.Count(f => f.status == FeedbackStatus.Accepted);
        //     var rejected = feedbacks.Count(f => f.status == FeedbackStatus.Cancelled);
        //     double avg = Math.Round((double)total / vacancies.Count(), 2);
        //
        //     var acceptedWorkers = feedbacks.Where(f => f.status == FeedbackStatus.Accepted)
        //         .Select(async f => await _workerRepository.GetWorkerByIdAsync(f.resume.worker_id)) // Assume Feedback has resume
        //         .Select(t => t.Result).ToList();
        //
        //     return new
        //     {
        //         VacancyCount = vacancies.Count(),
        //         TotalFeedbacks = total,
        //         AcceptedFeedbacks = accepted,
        //         RejectedFeedbacks = rejected,
        //         AvgFeedbackPerVacancy = avg,
        //         Period = $"{startDate:MMMM yyyy} - {endDate:MMMM yyyy}",
        //         AcceptedWorkers = acceptedWorkers
        //     };
        // }

        
        // public async Task<byte[]> GetStatisticsPdfAsync(string companyId, int start_year, int start_month, int end_year, int end_month)
        // {
        //     dynamic data = await GetStatisticsJsonAsync(companyId, start_year, start_month, end_year, end_month);
        //     byte[] pdf = Document.Create(container =>
        //     {
        //         container.Page(page =>
        //         {
        //             page.Size(PageSizes.A4);
        //             page.Margin(2, Unit.Centimetre);
        //             page.DefaultTextStyle(x => x.FontSize(12));
        //
        //             // Заголовок
        //             page.Header()
        //                 .Text($"Статистика компании за {data.Period}")
        //                 .FontSize(18).Bold().AlignCenter();
        //
        //             // Содержание
        //             page.Content()
        //                 .Column(column =>
        //                 {
        //                     column.Item().Text($"Количество опубликованных вакансий: {data.VacancyCount}")
        //                         .FontSize(14);
        //
        //                     column.Item().Text($"Общее количество откликов: {data.TotalFeedbacks}")
        //                         .FontSize(14);
        //
        //                     column.Item().Text($"Принятые отклики: {data.AcceptedFeedbacks}")
        //                         .FontSize(14);
        //
        //                     column.Item().Text($"Отклоненные отклики: {data.RejectedFeedbacks}")
        //                         .FontSize(14);
        //
        //                     column.Item().Text($"Среднее количество откликов на вакансию: {data.AvgFeedbackPerVacancy}")
        //                         .FontSize(14);
        //
        //                     column.Item().PaddingTop(20).Text("Принятые сотрудники:")
        //                         .FontSize(16).Bold();
        //
        //                     if (data.AcceptedWorkers.Count > 0)
        //                     {
        //                         column.Item().Table(table =>
        //                         {
        //                             table.ColumnsDefinition(columns =>
        //                             {
        //                                 columns.RelativeColumn(3);
        //                                 columns.RelativeColumn(2);
        //                                 columns.RelativeColumn(2);
        //                                 columns.RelativeColumn(1);
        //                             });
        //
        //                             table.Header(header =>
        //                             {
        //                                 header.Cell().Text("ФИО").SemiBold().FontSize(12);
        //                                 header.Cell().Text("Email").SemiBold().FontSize(12);
        //                                 header.Cell().Text("Телефон").SemiBold().FontSize(12);
        //                                 header.Cell().Text("Возраст").SemiBold().FontSize(12);
        //                             });
        //
        //                             foreach (var worker in data.AcceptedWorkers)
        //                             {
        //                                 table.Cell().Text($"{worker.first_name} {worker.second_name} {worker.surname}");
        //                                 table.Cell().Text($"{worker.email}");
        //                                 table.Cell().Text($"{worker.phone}");
        //                                 table.Cell().Text($"{worker.age.ToString()}");
        //                             }
        //                         });
        //                     }
        //                     else
        //                     {
        //                         column.Item().PaddingTop(10)
        //                             .Text("За этот период никто не был принят.")
        //                             .Italic()
        //                             .FontSize(12);
        //                     }
        //                 });
        //
        //             // Футер
        //             page.Footer()
        //                 .AlignCenter()
        //                 .Text("Worky - платформа трудоустройства")
        //                 .FontSize(10)
        //                 .Italic();
        //         });
        //     }).GeneratePdf();
        //     return pdf;
        // }

        public async Task<byte[]> GetFlyerAsync(Guid vacancyId, string url)
        {
            var vacancy = await _vacancyRepository.GetVacancyByIdAsync(vacancyId);
            byte[] flyer = Document.Create(container =>
            {
                container.Page(page =>
                {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(12).FontFamily("DejaVu"));

                        page.Header()
                            .Text(
                                $"Worky - Флайер на ваканцию \"{vacancy.post}\" от компании \"{vacancy.company.name}\"")
                            .AlignCenter().FontSize(18).Bold();

                        page.Content().PaddingVertical(1).Column(descriptor =>
                        {
                            descriptor.Item().Text("Информация о компании").SemiBold().FontSize(14);
                            descriptor.Item().Text($"Название: {vacancy.company.name}");
                            descriptor.Item().Text($"Email: {vacancy.company.email ?? "—"}");
                            descriptor.Item().Text($"Телефон: {vacancy.company.phoneNumber ?? "—"}");
                            descriptor.Item().Text($"Сайт: {vacancy.company.website ?? "—"}");
                            descriptor.Item()
                                .Text(
                                    $"Адрес офиса: {vacancy.company.latitude}, {vacancy.company.longitude}");

                            descriptor.Item().PaddingTop(15).LineHorizontal(1);
                            descriptor.Item().Text("Детали вакансии").SemiBold().FontSize(14);
                            descriptor.Item().Text($"Должность: {vacancy.post}");
                            descriptor.Item().Text($"Описание: {vacancy.description}");
                            descriptor.Item().Text($"Минимальная зарплата: {vacancy.min_salary} ₽");
                            descriptor.Item()
                                .Text(
                                    $"Максимальная зарплата: {vacancy.max_salary?.ToString() ?? "Не указана"} ₽");
                            descriptor.Item().Text($"Опыт работы: {vacancy.experience} лет");

                            descriptor.Item().PaddingTop(15).LineHorizontal(1);
                            descriptor.Item().Text("Фильтры по направлениям").SemiBold().FontSize(14);

                            if (vacancy.activities != null && vacancy.activities.Count > 0)
                            {
                                foreach (var activity in vacancy.activities)
                                {
                                    descriptor.Item().Text($"• {activity.direction} ({activity.type})");
                                }
                            }

                            // descriptor.Item().Image(qrBytes);

                            descriptor.Item().Row(row =>
                            {
                                row.ConstantItem(5, Unit.Centimetre)
                                    .AspectRatio(1)
                                    .Background(Colors.White)
                                    .Svg(size =>
                                    {
                                        var writer = new QRCodeWriter();
                                        var qrCode = writer.encode(url, BarcodeFormat.QR_CODE, (int)size.Width,
                                            (int)size.Height);
                                        var renderer = new SvgRenderer { FontName = "Lato" };
                                        return renderer.Render(qrCode, BarcodeFormat.EAN_13, null).Content;
                                    });
                            });

                            // descriptor.Item()
                            //     .Image(data => data.Bytes(qrBytes))
                            //     .Width(150)
                            //     .Height(150)
                            //     .AlignCenter();
                        });

                        page.Footer()
                            .AlignCenter()
                            .Text("Created by Worky.ru")
                            .Italic()
                            .FontSize(10);
                });
            }).GeneratePdf();
            return flyer;
        }

        public async Task<CompanyProfileDtos> GetProfileAsync(string companyId, string token, CancellationToken cancellationToken = default)
        {
            Company company = await _companyRepository.GetCompanyByIdAsync(Guid.Parse(companyId));
            // UserResponse? user = await _authClient.GetUserByIdAsync(companyId, token, cancellationToken);
            List<Deal?> deals = await _dealRepository.GetDealsByCompanyId(Guid.Parse(companyId), cancellationToken);
            
            CompanyDto companyDto = new CompanyDto()
            {
                id = company.UserId,
                name = company.name,
                email = company.email,
                phoneNumber = company.phoneNumber,
                website = company.website,
                latitude = company.latitude,
                longitude = company.longitude,
            };
            
            List<DealDto> dealDtos = deals.Select(d => new DealDto()
            {
                id = d.id,
                tariff_id = d.tariff_id,
                tariff = d.tariff,
                company_id = d.company_id.ToString(),
                date_start = d.date_start,
                date_end = d.date_end,
                sum = d.tariff.price * ((d.date_end.Year - d.date_start.Year) * 12 + (d.date_end.Month - d.date_start.Month))
            }).ToList();
            
            return new CompanyProfileDtos { company = companyDto, 
                // user = user,
                deals = dealDtos
            };
        }
        
        private async Task<bool> CompanyHasVacancy(Guid companyid, Guid vacancyId)
        {
            var myVacancy = await _vacancyRepository.GetMyVacanciesAsync(companyid.ToString(), vacancyId);
            if (myVacancy.Any())
            {
                return true;
            }
            return false;
        }
        
        private async Task<VacancyDtos> BuildFullVacancyAsync(Guid vacancyId)
        {
            VacancyDtos vacancy = await _vacancyRepository.GetVacancyByIdAsync(vacancyId);
            if (vacancy == null)
            {
                return null;
            }

            List<int> allIds = vacancy.activities.Select(a => a.id).Distinct().ToList() ?? new List<int>();
            if (allIds.Any())
            {
                // var activities = await _filterClient.GetFiltersByIdAsync(allIds);
                List<TypeOfActivityResponse> activities = await _filterCacheService.GetFiltersByIdsAsync(allIds);
                Dictionary<int, TypeOfActivityResponse> activityDict = activities.ToDictionary(a => a.id, a => a);

                vacancy.activities = vacancy.activities
                    .Where(a => activityDict.ContainsKey(a.id))
                    .Select(a => activityDict[a.id])
                    .ToList();
            }

            Guid copmanyId = (Guid)vacancy.company_id!;
            
            Company company = await _companyRepository.GetCompanyByIdAsync(copmanyId);
            CompanyDto companyDto = new CompanyDto()
            {
                id = copmanyId,
                email = company.email,
                longitude = company.longitude,
                latitude = company.latitude,
                name = company.name,
                phoneNumber = company.phoneNumber,
                website = company.website,
            };

            vacancy.company = companyDto;
            return vacancy;
        }
}