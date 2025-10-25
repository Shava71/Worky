using Worky.Contracts;
using Worky.DTO;
using Worky.Repositories.Interfaces;

namespace Worky.Services;

public class WorkerService : IWorkerService
{
        private readonly IVacancyRepository _vacancyRepository;
        private readonly IResumeRepository _resumeRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IWorkerRepository _workerRepository;
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<WorkerService> _logger;

        public WorkerService(IVacancyRepository vacancyRepository, IResumeRepository resumeRepository, IFeedbackRepository feedbackRepository, IWorkerRepository workerRepository, IAuthRepository authRepository, ILogger<WorkerService> logger)
        {
            _vacancyRepository = vacancyRepository;
            _resumeRepository = resumeRepository;
            _feedbackRepository = feedbackRepository;
            _workerRepository = workerRepository;
            _authRepository = authRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<VacancyDtos>> FilterVacanciesAsync(GetVacanciesRequest request)
        {
            return await _vacancyRepository.GetVacanciesAsync(request);
        }

        public async Task<VacancyDtos> GetVacancyInfoAsync(ulong vacancyId)
        {
            return await _vacancyRepository.GetVacancyByIdAsync(vacancyId);
        }

        public async Task<IEnumerable<ResumeDtos>> GetMyResumesAsync(string workerId, ulong? resumeId)
        {
            return await _resumeRepository.GetMyResumesAsync(workerId, resumeId);
        }

        public async Task<ulong> CreateResumeAsync(CreateResume resume, string workerId)
        {
            return await _resumeRepository.CreateResumeAsync(resume, workerId);
        }

        public async Task UpdateResumeAsync(UpdateResume resume, string workerId)
        {
            // Check ownership
            await _resumeRepository.UpdateResumeAsync(resume);
        }

        public async Task DeleteResumeAsync(ulong id, string workerId)
        {
            // Check ownership
            await _resumeRepository.DeleteResumeAsync(id);
        }

        public async Task<IEnumerable<ulong>> AddResumeFilterAsync(AddFilter filter, string workerId)
        {
            // Check ownership
            return await _resumeRepository.AddResumeFiltersAsync(filter);
        }

        public async Task DeleteResumeFilterAsync(ulong filterId, string workerId)
        {
            // Check ownership
            await _resumeRepository.DeleteResumeFilterAsync(filterId);
        }

        public async Task<WorkerProfileDto> GetProfileAsync(string workerId)
        {
            var worker = await _workerRepository.GetWorkerByIdAsync(workerId);
            var user = await _authRepository.FindByIdAsync(workerId);
            return new WorkerProfileDto { worker = worker, user = user };
        }

        public async Task<IEnumerable<FeedbackDtos>> GetFeedbacksAsync(string workerId, ulong? vacancyId)
        {
            return await _feedbackRepository.GetFeedbacksAsync(workerId, vacancyId: vacancyId);
        }

        public async Task<ulong> MakeFeedbackAsync(MakeFeedbackRequest request, string workerId)
        {
            string creator1 = "worker_user"; // Adapt
            string creator2 = "company_user"; // Adapt from request
            return await _feedbackRepository.CreateFeedbackAsync(request, creator1, creator2);
        }

        public async Task DeleteFeedbackAsync(ulong id, string workerId)
        {
            // Check ownership
            await _feedbackRepository.DeleteFeedbackAsync(id);
        }
}