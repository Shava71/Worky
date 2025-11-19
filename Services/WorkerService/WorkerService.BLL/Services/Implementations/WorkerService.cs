using CompanyService.DAL.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using WorkerService.BLL.Events;
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
        private readonly IResumeRepository _resumeRepository;
        private readonly IWorkerRepository _workerRepository;
        private readonly ILogger<WorkerService> _logger;
        private readonly IAuthClient _authClient;
        private readonly IFilterCacheService _filterCacheService;
        
        private readonly ITopicProducer<ResumeCreatedEvent> _resumeCreatedTopicProducer;
        private readonly ITopicProducer<ResumeUpdatedEvent> _resumeUpdatedTopicProducer;
        private readonly ITopicProducer<ResumeDeletedEvent> _resumeDeletedTopicProducer;
        
        private readonly ITopicProducer<ResumeFilterAddEvent> _resumeFilterAddTopicProducer;
        private readonly ITopicProducer<ResumeFilterDeleteEvent> _resumeFilterDeleteTopicProducer;

        public WorkerService(
            IResumeRepository resumeRepository, 
            IWorkerRepository workerRepository, 
            ILogger<WorkerService> logger,
            IAuthClient authClient,
            IFilterCacheService filterCacheService,
            ITopicProducer<ResumeCreatedEvent> resumeCreatedTopicProducer,
            ITopicProducer<ResumeUpdatedEvent> resumeUpdatedTopicProducer,
            ITopicProducer<ResumeDeletedEvent> resumeDeletedTopicProducer,
            ITopicProducer<ResumeFilterAddEvent> resumeFilterAddTopicProducer,
            ITopicProducer<ResumeFilterDeleteEvent> resumeFilterDeleteTopicProducer)
        {
            _resumeRepository = resumeRepository;
            _workerRepository = workerRepository;
            _logger = logger;
            _authClient = authClient;
            _filterCacheService = filterCacheService;
            _resumeCreatedTopicProducer = resumeCreatedTopicProducer;
            _resumeUpdatedTopicProducer = resumeUpdatedTopicProducer;
            _resumeDeletedTopicProducer = resumeDeletedTopicProducer;
            _resumeFilterAddTopicProducer = resumeFilterAddTopicProducer;
            _resumeFilterDeleteTopicProducer = resumeFilterDeleteTopicProducer;
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
        // public async Task<IEnumerable<ResumeDtos>> FilterResumesAsync(GetResumesRequest request)
        // {
        //     var resumes = await _resumeRepository.GetResumesAsync(request);
        //     List<int> allIds = resumes.SelectMany(r => r.activities.Select(a => a.id)).Distinct().ToList();
        //     
        //     if(allIds.Any())
        //     {
        //         List<TypeOfActivityResponse> activities = await _filterClient.GetFiltersByIdAsync(allIds);
        //         var activityDict = activities.ToDictionary(a => a.id, a => a);
        //         
        //         foreach (ResumeDtos resume in resumes)
        //         {
        //             resume.activities = resume.activities.Where(a => activityDict.ContainsKey(a.id))
        //                 .Select(a => activityDict[a.id]).ToList();
        //         }
        //     }
        //     
        //     return resumes;
        // }
        //
        public async Task<ResumeDtos> GetResumeInfoAsync(Guid resumeId)
        {
            ResumeDtos resume = await BuildFullResumeAsync(resumeId);
            
            return resume;
        }
        
        public async Task<IEnumerable<ResumeDtos>?> GetMyResumesAsync(string workerId, Guid? resumeId)
        {
            IEnumerable<ResumeDtos> resumes = await _resumeRepository.GetMyResumesAsync(workerId, resumeId);
            List<ResumeDtos> resumeList = resumes.ToList();

            if (!resumeList.Any())
                return resumeList;

            List<int> allActivityIds = resumeList
                .SelectMany(r => r.activities.Select(a => a.id))
                .Distinct()
                .ToList();

            if (allActivityIds.Any())
            {
                List<TypeOfActivityResponse> activities = await _filterCacheService.GetFiltersByIdsAsync(allActivityIds);
                Dictionary<int, TypeOfActivityResponse> activityDict = activities.ToDictionary(a => a.id, a => a);

                foreach (ResumeDtos resume in resumeList)
                {
                    resume.activities = resume.activities
                        .Where(a => activityDict.ContainsKey(a.id))
                        .Select(a => activityDict[a.id])
                        .ToList();
                }
            }
            
            Worker worker = await _workerRepository.GetWorkerByIdAsync(Guid.Parse(workerId));
            WorkerDtos workerDto = new WorkerDtos
            {
                id = worker.UserId.ToString(),
                first_name = worker.first_name,
                second_name = worker.second_name,
                surname = worker.surname,
                birthday = worker.birthday,
                phone = worker.PhoneNumber,
                email = worker.Email,
            };

            foreach (var resume in resumeList)
            {
                resume.worker = workerDto;
            }

            return resumeList;
        }

        public async Task<Guid> CreateResumeAsync(CreateResume resume, string workerId)
        {
            Guid resumeId = await _resumeRepository.CreateResumeAsync(resume, workerId);
            
            ResumeDtos fullResume = await BuildFullResumeAsync(resumeId);
            
            await _resumeCreatedTopicProducer.Produce(new ResumeCreatedEvent(fullResume));
            _logger.LogInformation("ResumeCreatedEvent published for resume {ResumeId}", resumeId);

            
            return resumeId;
        }

        public async Task UpdateResumeAsync(UpdateResume resume, string workerId)
        {
            await _resumeRepository.UpdateResumeAsync(resume);

            ResumeDtos fullResume = await BuildFullResumeAsync(resume.id);
            await _resumeUpdatedTopicProducer.Produce(new ResumeUpdatedEvent(fullResume));

            _logger.LogInformation("ResumeUpdatedEvent published for resume {ResumeId}", resume.id);
        }

        public async Task DeleteResumeAsync(Guid id, string workerId)
        {
            await _resumeRepository.DeleteResumeAsync(id);

            await _resumeDeletedTopicProducer.Produce(new ResumeDeletedEvent(id));
            _logger.LogInformation("ResumeDeletedEvent published for resume {ResumeId}", id);
        }

        public async Task<IEnumerable<Guid>> AddResumeFilterAsync(AddFilter filter, string workerId)
        {
            if (!await WorkerHasResume(Guid.Parse(workerId), filter.id))
            {
                // throw KeyNotFoundException("");
                return [];
            }
            
            List<TypeOfActivityResponse> activities = await _filterCacheService.GetFiltersByIdsAsync(filter.typeOfActivity_id);
            try
            {
                await _resumeFilterAddTopicProducer.Produce(new ResumeFilterAddEvent(filter.id, activities));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding vacancy filter for {workerId}", workerId);
                return [];
            }
            
            IEnumerable<Guid> filter_id = await _resumeRepository.AddResumeFiltersAsync(filter);
            if (!filter_id.Any())
            {
                return [];
            }
            
            return filter_id;
        }

        public async Task DeleteResumeFilterAsync(Guid filterId, string workerId)
        {
            Resume_filter resume_filter = await _resumeRepository.GetResumeFilterByIdAsync(filterId);
            if (resume_filter == null)
            {
                return;
            }

            ResumeFilterDeleteEvent @event = new ResumeFilterDeleteEvent(
                resume_id: resume_filter.resume_id,
                activity_id: resume_filter.typeOfActivity_id
            );
            try
            {
                await _resumeFilterDeleteTopicProducer.Produce(@event);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vacancy filter for {workerId}", workerId);
                return;
            }
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
        
        private async Task<ResumeDtos> BuildFullResumeAsync(Guid resumeId)
        {
            ResumeDtos resume = await _resumeRepository.GetResumeByIdAsync(resumeId);
            if (resume == null)
            {
                return null;
            }

            List<int> allIds = resume.activities.Select(a => a.id).Distinct().ToList();
            if (allIds.Any())
            {
                // var activities = await _filterClient.GetFiltersByIdAsync(allIds);
                List<TypeOfActivityResponse> activities = await _filterCacheService.GetFiltersByIdsAsync(allIds);
                Dictionary<int, TypeOfActivityResponse> activityDict = activities.ToDictionary(a => a.id, a => a);

                resume.activities = resume.activities
                    .Where(a => activityDict.ContainsKey(a.id))
                    .Select(a => activityDict[a.id])
                    .ToList();
            }

            string workerId = resume.worker_id!;
            
            Worker worker = await _workerRepository.GetWorkerByIdAsync(Guid.Parse(workerId));
            WorkerDtos workerDto = new WorkerDtos
            {
                id = worker.UserId.ToString(),
                first_name = worker.first_name,
                second_name = worker.second_name,
                surname = worker.surname,
                birthday = worker.birthday,
                phone = worker.PhoneNumber,
                email = worker.Email,
            };

            resume.worker = workerDto;
            return resume;
        }
}

