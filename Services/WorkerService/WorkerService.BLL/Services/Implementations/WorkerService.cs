using Microsoft.Extensions.Logging;
using WorkerService.BLL.Services.Http.Interfaces;
using WorkerService.BLL.Services.Interfaces;
using WorkerService.DAL.Clients;
using WorkerService.DAL.Contracts;
using WorkerService.DAL.DTO;
using WorkerService.DAL.Entities;
using WorkerService.DAL.Repositories.Interfaces;


namespace WorkerService.BLL.Services.Implementations;

public class WorkerService : IWorkerService
{
        //private readonly IVacancyRepository _vacancyRepository;
        private readonly IResumeRepository _resumeRepository;
       // private readonly IFeedbackRepository _feedbackRepository;
        private readonly IWorkerRepository _workerRepository;
        //private readonly IAuthRepository _authRepository;
        private readonly ILogger<WorkerService> _logger;
        private readonly IAuthClient _authClient;
        private IWorkerService _workerServiceImplementation;

        public WorkerService(
            //IVacancyRepository vacancyRepository, 
            IResumeRepository resumeRepository, 
            //IFeedbackRepository feedbackRepository, 
            IWorkerRepository workerRepository, 
           // IAuthRepository authRepository, 
            ILogger<WorkerService> logger,
            IAuthClient authClient)
        {
            //_vacancyRepository = vacancyRepository;
            _resumeRepository = resumeRepository;
            //_feedbackRepository = feedbackRepository;
            _workerRepository = workerRepository;
            //_authRepository = authRepository;
            _logger = logger;
            _authClient = authClient;
        }

        // public async Task<IEnumerable<VacancyDtos>> FilterVacanciesAsync(GetVacanciesRequest request)
        // {
        //     return await _vacancyRepository.GetVacanciesAsync(request);
        // }
        //
        // public async Task<VacancyDtos> GetVacancyInfoAsync(ulong vacancyId)
        // {
        //     return await _vacancyRepository.GetVacancyByIdAsync(vacancyId);
        // }
        public async Task<IEnumerable<ResumeDtos>> FilterResumesAsync(GetResumesRequest request)
        {
            return await _resumeRepository.GetResumesAsync(request);
        }

        public async Task<ResumeDtos> GetResumeInfoAsync(Guid resumeId)
        {
            return await _resumeRepository.GetResumeByIdAsync(resumeId);
            //допилить активити
        }

        public async Task<IEnumerable<ResumeDtos>> GetMyResumesAsync(string workerId, Guid? resumeId)
        {
            return await _resumeRepository.GetMyResumesAsync(workerId, resumeId);
        }

        public async Task<Guid> CreateResumeAsync(CreateResume resume, string workerId)
        {
            return await _resumeRepository.CreateResumeAsync(resume, workerId);
        }

        public async Task UpdateResumeAsync(UpdateResume resume, string workerId)
        {
            // Check ownership
            await _resumeRepository.UpdateResumeAsync(resume);
        }

        public async Task DeleteResumeAsync(Guid id, string workerId)
        {
            // Check ownership
            await _resumeRepository.DeleteResumeAsync(id);
        }

        public async Task<IEnumerable<Guid>> AddResumeFilterAsync(AddFilter filter, string workerId)
        {
            // Check ownership
            return await _resumeRepository.AddResumeFiltersAsync(filter);
        }

        public async Task DeleteResumeFilterAsync(Guid filterId, string workerId)
        {
            // Check ownership
            await _resumeRepository.DeleteResumeFilterAsync(filterId);
        }

        public async Task<WorkerProfileDto> GetProfileAsync(string workerId, string token, CancellationToken cancellationToken = default)
        {
            Worker worker = await _workerRepository.GetWorkerByIdAsync(Guid.Parse(workerId));
            UserResponse? user = await _authClient.GetUserByIdAsync(workerId, token, cancellationToken);
            
            WorkerDtos workerDtos = new WorkerDtos()
            {
                birthday = worker.birthday,
                surname = worker.surname,
                first_name = worker.first_name,
                second_name = worker.second_name,
                image = user?.image,
                id = worker.UserId.ToString(),
            };
            
            return new WorkerProfileDto { worker = workerDtos, UserResponse = user };
        }

        // public async Task<IEnumerable<FeedbackDtos>> GetFeedbacksAsync(string workerId, ulong? vacancyId)
        // {
        //     return await _feedbackRepository.GetFeedbacksAsync(workerId, vacancyId: vacancyId);
        // }
        //
        // public async Task<ulong> MakeFeedbackAsync(MakeFeedbackRequest request, string workerId)
        // {
        //     string creator1 = "worker_user"; // Adapt
        //     string creator2 = "company_user"; // Adapt from request
        //     return await _feedbackRepository.CreateFeedbackAsync(request, creator1, creator2);
        // }
        //
        // public async Task DeleteFeedbackAsync(ulong id, string workerId)
        // {
        //     // Check ownership
        //     await _feedbackRepository.DeleteFeedbackAsync(id);
        // }
}