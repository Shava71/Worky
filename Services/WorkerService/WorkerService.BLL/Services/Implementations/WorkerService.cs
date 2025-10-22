using Microsoft.Extensions.Logging;
using WorkerService.BLL.Services.Http.Interfaces;
using WorkerService.BLL.Services.Interfaces;
using WorkerService.DAL.Clients;
using WorkerService.DAL.Contracts;
using WorkerService.DAL.DTO;
using WorkerService.DAL.Entities;
using WorkerService.DAL.HttpClients.Clients;
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
        private readonly IFilterClient _filterClient;
        private IWorkerService _workerServiceImplementation;

        public WorkerService(
            //IVacancyRepository vacancyRepository, 
            IResumeRepository resumeRepository, 
            //IFeedbackRepository feedbackRepository, 
            IWorkerRepository workerRepository, 
           // IAuthRepository authRepository, 
            ILogger<WorkerService> logger,
            IAuthClient authClient,
            IFilterClient filterClient)
        {
            //_vacancyRepository = vacancyRepository;
            _resumeRepository = resumeRepository;
            //_feedbackRepository = feedbackRepository;
            _workerRepository = workerRepository;
            //_authRepository = authRepository;
            _logger = logger;
            _authClient = authClient;
            _filterClient = filterClient;
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
            var resumes = await _resumeRepository.GetResumesAsync(request);
            List<int> allIds = resumes.SelectMany(r => r.activities.Select(a => a.id)).Distinct().ToList();
            
            if(allIds.Any())
            {
                List<TypeOfActivityResponse> activities = await _filterClient.GetFiltersByIdAsync(allIds);
                var activityDict = activities.ToDictionary(a => a.id, a => a);
                
                foreach (ResumeDtos resume in resumes)
                {
                    resume.activities = resume.activities.Where(a => activityDict.ContainsKey(a.id))
                        .Select(a => activityDict[a.id]).ToList();
                }
            }
            
            return resumes;
        }

        public async Task<ResumeDtos> GetResumeInfoAsync(Guid resumeId)
        {
            var resume = await _resumeRepository.GetResumeByIdAsync(resumeId);
            
            List<int> allIds = resume.activities.Select(a => a.id).Distinct().ToList();
            
            if(allIds.Any())
            {
                List<TypeOfActivityResponse> activities = await _filterClient.GetFiltersByIdAsync(allIds);
                var activityDict = activities.ToDictionary(a => a.id, a => a);
                
                
                resume.activities = resume.activities
                    .Where(a => activityDict.ContainsKey(a.id))
                    .Select(a => activityDict[a.id])
                    .ToList();
                
            }
            
            return resume;
        }

        public async Task<IEnumerable<ResumeDtos>> GetMyResumesAsync(string workerId, Guid? resumeId)
        {
            //return await _resumeRepository.GetMyResumesAsync(workerId, resumeId);
            var resumes = await _resumeRepository.GetMyResumesAsync(workerId, resumeId);
            
            List<int> allIds = resumes.SelectMany(r => r.activities.Select(a => a.id)).Distinct().ToList();
            
            if(allIds.Any())
            {
                List<TypeOfActivityResponse> activities = await _filterClient.GetFiltersByIdAsync(allIds);
                var activityDict = activities.ToDictionary(a => a.id, a => a);
                
                foreach (ResumeDtos resume in resumes)
                {
                    resume.activities = resume.activities.Where(a => activityDict.ContainsKey(a.id))
                        .Select(a => activityDict[a.id]).ToList();
                }
            }
            
            return resumes;
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
            if (!await WorkerHasResume(Guid.Parse(workerId), filter.id))
            {
                return [];
            }
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

        private async Task<bool> WorkerHasResume(Guid workerid, Guid resumeId)
        {
            var myResume = await _resumeRepository.GetMyResumesAsync(workerid.ToString(), resumeId);
            if (myResume.Any())
            {
                return true;
            }
            return false;
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