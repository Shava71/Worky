using System.Text.Json;
using AuthService.Application.Events;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Outbox;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AuthService.Infrastructure.Repository;
using AuthService.Infrastructure.Repository.Interface;

namespace AuthService.Application.Services;

public class UserService : IUserService
{
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository authRepository, IJwtService jwtService, ILogger<UserService> logger)
        {
            _userRepository = authRepository;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<IActionResult> RegisterAsync(RegisterRequestContract request)
        {
            try
            {
                User user = new User(request.UserName, request.Email, request.PasswordHash, request.PhoneNumber);

                object @event;
                string topic;
                if (request.Role.Equals("Worker"))
                {
                    @event = new UserWorkerCreatedEvent()
                    {
                        UserId = user.Id.ToString(),
                        second_name = request.second_name!,
                        first_name = request.first_name!,
                        surname = request.surname!,
                        birthday = request.birthday!.Value,
                    };
                    topic = "user.worker.created";
                }
                else if (request.Role.Equals("Company"))
                {
                    @event = new UserCompanyCreatedEvent()
                    {
                        UserId = user.Id.ToString(),
                        email_info = request.email_info!,
                        latitude = request.latitude!,
                        longitude = request.longitude!,
                        name = request.name!,
                        phone_info = request.phone_info!,
                        website = request.website!,
                    };
                    topic = "user.company.created";
                }
                else
                {
                    throw new ArgumentException($"Unsupported role: {request.Role}");
                }
                
                string JsonPayload = JsonSerializer.Serialize(@event);
                
                OutboxMessage outboxMessage = new OutboxMessage(_topic: topic, 
                    _type:@event.GetType().Name, 
                    _payload: JsonPayload);
                
                await _userRepository.CreateUserWithOutboxMessageAsync(user, message: outboxMessage);
                //await _userRepository.CreateUserAsync(user);

                Role role = await _userRepository.FindRoleByNameAsync(request.Role);
                if (role == null) return new BadRequestObjectResult("user role doesn't exist");

                await _userRepository.AddToRoleAsync(user, role);

                // if (request.Role == "Company")
                // {
                //     if (request.longitude == null || request.latitude == null) return new BadRequestObjectResult("Longitude and Latitude are required.");
                //     
                //     string sql = @"
                //             INSERT INTO company (
                //                 id,
                //                 name,
                //                 email,
                //                 phoneNumber,
                //                 office_coord,
                //                 website,
                //                 createdBy
                //             )
                //             VALUES (
                //                 @Id,
                //                 @Name,
                //                 @Email,
                //                 @PhoneNumber,
                //                 ST_PointFromText(@PointWKT),
                //                 @Website,
                //                 @createdBy
                //             );";
                //     
                //     var parameters = new
                //     {
                //         Id = user.Id,
                //         Name = request.name,
                //         Email = request.email_info,
                //         PhoneNumber = request.phone_info,
                //         PointWKT = $"POINT({request.longitude} {request.latitude})",
                //         Website = request.website,
                //         createdBy = user.UserName
                //     };
                //     // await _companyRepository.CreateCompanyAsync(company);
                //     await _authRepository.ExecuteSqlWithParamAsync(sql, parameters);
                // }
                // else if (request.Role == "Worker")
                // {
                //     var worker = new Worker
                //     {
                //         id = user.Id,
                //         first_name = request.first_name,
                //         second_name = request.second_name,
                //         surname = request.surname,
                //         birthday = request.birthday.Value
                //     };
                //     await _workerRepository.CreateWorkerAsync(worker);
                // }

                // // DB user creation
                // var passwordHash = user.PasswordHash; // Assume it's accessible or query
                // string nameUser = user.UserName;
                // await _authRepository.ExecuteSqlAsync($"DROP USER IF EXISTS '{nameUser}'@'localhost';");
                // await _authRepository.ExecuteSqlAsync($"CREATE USER '{nameUser}'@'localhost' IDENTIFIED BY '{passwordHash}';");
                //
                // string grantRole = request.Role switch
                // {
                //     "Company" => "company_role",
                //     "Worker" => "worker_role",
                //     _ => throw new ArgumentOutOfRangeException()
                // };
                // await _authRepository.ExecuteSqlAsync($"GRANT '{grantRole}' TO '{nameUser}'@'localhost';");
                // await _authRepository.ExecuteSqlAsync($"SET DEFAULT ROLE {grantRole} FOR '{nameUser}'@'localhost';");

                return new OkObjectResult(new { Message = "User created", UserId = user.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "failed to register");
                return new BadRequestResult();
            }
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            User user = await _userRepository.FindByEmailAsync(request.Email);
            if (user != null && await _userRepository.CheckPasswordAsync(user, request.Password))
            {
                List<string> roles = await _userRepository.GetRolesAsync(user);
                var jwt = _jwtService.GenerateToken(user.Id, roles);
                return new LoginResponse { Id = user.Id.ToString(), Token = jwt, Role = roles };
            }
            return null;
        }

        // public async Task<object> GetClaimsAsync(string userId)
        // {
        //     // Assume claims from token or db
        //     var user = await _authRepository.FindByIdAsync(userId);
        //     var role = (await _authRepository.GetRolesAsync(user)).FirstOrDefault();
        //     return new { Id = userId, Role = role };
        // }
}