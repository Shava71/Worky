using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using Worky.Contracts;
using Worky.Migrations;
using Worky.Models;
using Worky.Repositories.Interfaces;

namespace Worky.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IWorkerRepository _workerRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepository, ICompanyRepository companyRepository, IWorkerRepository workerRepository, IJwtService jwtService, ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _companyRepository = companyRepository;
            _workerRepository = workerRepository;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<IActionResult> RegisterAsync(RegisterRequestContract request)
        {
            try
            {
                var user = new Users
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = request.UserName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber
                };

                var result = await _authRepository.CreateUserAsync(user, request.PasswordHash);
                if (!result.Succeeded) return new BadRequestObjectResult(result.Errors);

                var role = await _authRepository.FindRoleByNameAsync(request.Role.ToString());
                if (role == null) return new BadRequestObjectResult("user role doesn't exist");

                await _authRepository.AddToRoleAsync(user, role.Name);

                if (request.Role == "Company")
                {
                    if (request.longitude == null || request.latitude == null) return new BadRequestObjectResult("Longitude and Latitude are required.");
                    
                    string sql = @"
                            INSERT INTO company (
                                id,
                                name,
                                email,
                                phoneNumber,
                                office_coord,
                                website,
                                createdBy
                            )
                            VALUES (
                                @Id,
                                @Name,
                                @Email,
                                @PhoneNumber,
                                ST_PointFromText(@PointWKT),
                                @Website,
                                @createdBy
                            );";
                    
                    var parameters = new
                    {
                        Id = user.Id,
                        Name = request.name,
                        Email = request.email_info,
                        PhoneNumber = request.phone_info,
                        PointWKT = $"POINT({request.longitude} {request.latitude})",
                        Website = request.website,
                        createdBy = user.UserName
                    };
                    // await _companyRepository.CreateCompanyAsync(company);
                    await _authRepository.ExecuteSqlWithParamAsync(sql, parameters);
                }
                else if (request.Role == "Worker")
                {
                    var worker = new Worker
                    {
                        id = user.Id,
                        first_name = request.first_name,
                        second_name = request.second_name,
                        surname = request.surname,
                        birthday = request.birthday.Value
                    };
                    await _workerRepository.CreateWorkerAsync(worker);
                }

                // DB user creation
                var passwordHash = user.PasswordHash; // Assume it's accessible or query
                string nameUser = user.UserName;
                await _authRepository.ExecuteSqlAsync($"DROP USER IF EXISTS '{nameUser}'@'localhost';");
                await _authRepository.ExecuteSqlAsync($"CREATE USER '{nameUser}'@'localhost' IDENTIFIED BY '{passwordHash}';");

                string grantRole = request.Role switch
                {
                    "Company" => "company_role",
                    "Worker" => "worker_role",
                    _ => throw new ArgumentOutOfRangeException()
                };
                await _authRepository.ExecuteSqlAsync($"GRANT '{grantRole}' TO '{nameUser}'@'localhost';");
                await _authRepository.ExecuteSqlAsync($"SET DEFAULT ROLE {grantRole} FOR '{nameUser}'@'localhost';");

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
            var user = await _authRepository.FindByEmailAsync(request.Email);
            if (user != null && await _authRepository.CheckPasswordAsync(user, request.Password))
            {
                var roles = await _authRepository.GetRolesAsync(user);
                var jwt = _jwtService.GenerateToken(Guid.Parse(user.Id), roles);
                return new LoginResponse { Id = user.Id, Token = jwt, Role = roles };
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