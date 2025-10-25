using System.Globalization;
using Worky.Contracts;
using Worky.DTO;
using Worky.Migrations;
using Worky.Repositories.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZXing;
using ZXing.QrCode;
using ZXing.Rendering;

namespace Worky.Services;

public class CompanyService : ICompnayService
{
        private readonly IResumeRepository _resumeRepository;
        private readonly IVacancyRepository _vacancyRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IWorkerRepository _workerRepository;
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(IResumeRepository resumeRepository,
            IVacancyRepository vacancyRepository,
            IFeedbackRepository feedbackRepository,
            ICompanyRepository companyRepository, 
            IWorkerRepository workerRepository,
            IAuthRepository authRepository,
            ILogger<CompanyService> logger)
        {
            _resumeRepository = resumeRepository;
            _vacancyRepository = vacancyRepository;
            _feedbackRepository = feedbackRepository;
            _companyRepository = companyRepository;
            _workerRepository = workerRepository;
            _authRepository = authRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ResumeDtos>> FilterResumesAsync(GetResumesRequest request)
        {
            return await _resumeRepository.GetResumesAsync(request);
        }

        public async Task<ResumeDtos> GetResumeInfoAsync(ulong resumeId)
        {
            return await _resumeRepository.GetResumeByIdAsync(resumeId);
        }

        public async Task<IEnumerable<VacancyDtos>> GetMyVacanciesAsync(string companyId, ulong? vacancyId)
        {
            return await _vacancyRepository.GetMyVacanciesAsync(companyId, vacancyId);
        }

        public async Task<ulong> CreateVacancyAsync(CreateVacancy vacancy, string companyId)
        {
            // Validation if needed
            return await _vacancyRepository.CreateVacancyAsync(vacancy, companyId);
        }

        public async Task UpdateVacancyAsync(UpdateVacancy vacancy, string companyId)
        {
            // Check ownership
            await _vacancyRepository.UpdateVacancyAsync(vacancy);
        }

        public async Task DeleteVacancyAsync(ulong id, string companyId)
        {
            // Check ownership
            await _vacancyRepository.DeleteVacancyAsync(id);
        }

        public async Task<IEnumerable<ulong>> AddVacancyFilterAsync(AddFilter filter, string companyId)
        {
            // Check ownership
            return await _vacancyRepository.AddVacancyFiltersAsync(filter);
        }

        public async Task DeleteVacancyFilterAsync(ulong filterId, string companyId)
        {
            // Check ownership
            await _vacancyRepository.DeleteVacancyFilterAsync(filterId);
        }

        public async Task<IEnumerable<FeedbackDtos>> GetFeedbacksAsync(string companyId, ulong? resumeId)
        {
            return await _feedbackRepository.GetFeedbacksAsync(companyId, resumeId: resumeId);
        }

        public async Task<ulong> MakeFeedbackAsync(MakeFeedbackRequest request, string companyId)
        {
            string creator1 = "company_user"; // Adapt
            string creator2 = "worker_user"; // Adapt
            return await _feedbackRepository.CreateFeedbackAsync(request, creator1, creator2);
        }

        public async Task DeleteFeedbackAsync(ulong id, string companyId)
        {
            // Check ownership
            await _feedbackRepository.DeleteFeedbackAsync(id);
        }

        public async Task<object> GetStatisticsJsonAsync(string companyId, int start_year, int start_month, int end_year, int end_month)
        {
            var startDate = new DateTime(start_year, start_month, 1);
            var endDay = DateTime.DaysInMonth(end_year, end_month);
            var endDate = new DateTime(end_year, end_month, endDay);
            var startOnly = DateOnly.FromDateTime(startDate);
            var endOnly = DateOnly.FromDateTime(endDate);

            var vacancies = await _companyRepository.GetVacanciesByCompanyAsync(companyId);
            if (!vacancies.Any())
            {
                return new
                {
                    VacancyCount = 0,
                    TotalFeedbacks = 0,
                    AcceptedFeedbacks = 0,
                    RejectedFeedbacks = 0,
                    AvgFeedbackPerVacancy = 0.0,
                    Period = $"{startDate:MMMM yyyy} - {endDate:MMMM yyyy}",
                    AcceptedWorkers = new List<WorkerDtos>()
                };
            }

            var feedbacks = await _companyRepository.GetFeedbacksByVacanciesAsync(vacancies, startOnly, endOnly);
            var total = feedbacks.Count();
            var accepted = feedbacks.Count(f => f.status == FeedbackStatus.Accepted);
            var rejected = feedbacks.Count(f => f.status == FeedbackStatus.Cancelled);
            double avg = Math.Round((double)total / vacancies.Count(), 2);

            var acceptedWorkers = feedbacks.Where(f => f.status == FeedbackStatus.Accepted)
                .Select(async f => await _workerRepository.GetWorkerByIdAsync(f.resume.worker_id)) // Assume Feedback has resume
                .Select(t => t.Result).ToList();

            return new
            {
                VacancyCount = vacancies.Count(),
                TotalFeedbacks = total,
                AcceptedFeedbacks = accepted,
                RejectedFeedbacks = rejected,
                AvgFeedbackPerVacancy = avg,
                Period = $"{startDate:MMMM yyyy} - {endDate:MMMM yyyy}",
                AcceptedWorkers = acceptedWorkers
            };
        }

        public async Task<byte[]> GetStatisticsPdfAsync(string companyId, int start_year, int start_month, int end_year, int end_month)
        {
            dynamic data = await GetStatisticsJsonAsync(companyId, start_year, start_month, end_year, end_month);
            byte[] pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Заголовок
                    page.Header()
                        .Text($"Статистика компании за {data.Period}")
                        .FontSize(18).Bold().AlignCenter();

                    // Содержание
                    page.Content()
                        .Column(column =>
                        {
                            column.Item().Text($"Количество опубликованных вакансий: {data.VacancyCount}")
                                .FontSize(14);

                            column.Item().Text($"Общее количество откликов: {data.TotalFeedbacks}")
                                .FontSize(14);

                            column.Item().Text($"Принятые отклики: {data.AcceptedFeedbacks}")
                                .FontSize(14);

                            column.Item().Text($"Отклоненные отклики: {data.RejectedFeedbacks}")
                                .FontSize(14);

                            column.Item().Text($"Среднее количество откликов на вакансию: {data.AvgFeedbackPerVacancy}")
                                .FontSize(14);

                            column.Item().PaddingTop(20).Text("Принятые сотрудники:")
                                .FontSize(16).Bold();

                            if (data.AcceptedWorkers.Count > 0)
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(2);
                                        columns.RelativeColumn(1);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Text("ФИО").SemiBold().FontSize(12);
                                        header.Cell().Text("Email").SemiBold().FontSize(12);
                                        header.Cell().Text("Телефон").SemiBold().FontSize(12);
                                        header.Cell().Text("Возраст").SemiBold().FontSize(12);
                                    });

                                    foreach (var worker in data.AcceptedWorkers)
                                    {
                                        table.Cell().Text($"{worker.first_name} {worker.second_name} {worker.surname}");
                                        table.Cell().Text($"{worker.email}");
                                        table.Cell().Text($"{worker.phone}");
                                        table.Cell().Text($"{worker.age.ToString()}");
                                    }
                                });
                            }
                            else
                            {
                                column.Item().PaddingTop(10)
                                    .Text("За этот период никто не был принят.")
                                    .Italic()
                                    .FontSize(12);
                            }
                        });

                    // Футер
                    page.Footer()
                        .AlignCenter()
                        .Text("Worky - платформа трудоустройства")
                        .FontSize(10)
                        .Italic();
                });
            }).GeneratePdf();
            return pdf;
        }

        public Task<byte[]> GetFlyerAsync(ulong vacancyId)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> GetFlyerAsync(ulong vacancyId, string url)
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

        public async Task<CompanyProfileDtos> GetProfileAsync(string companyId)
        {
            Company company = await _companyRepository.GetCompanyByIdAsync(companyId);
            Users user = await _authRepository.FindByIdAsync(companyId);
            CompanyDto companyDto = new CompanyDto()
            {
                id = company.id,
                name = company.name,
                email = company.email,
                phoneNumber = company.phoneNumber,
                website = company.website,
                latitude = company.office_coord.Y.ToString(CultureInfo.InvariantCulture),
                longitude = company.office_coord.X.ToString(CultureInfo.InvariantCulture),
            };
            return new CompanyProfileDtos { company = companyDto, user = user };
        }
}