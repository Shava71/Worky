using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MySqlConnector;
using Worky.Context;
using Worky.Contracts;
using Worky.Migrations;
using Worky.Models;
using Worky.Services;

using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.HttpSys;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Worky.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class AuthorizationController : Controller
{
    private readonly IConfiguration _configuration;
    WorkyDbContext _dbContext;
    UserManager<Users> _userManager;
    SignInManager<Users> _signInManager;
    RoleManager<Roles> _roleManager;
    ILogger<AuthorizationController> _logger;
    IJwtService _jwtService;
    
    
    public AuthorizationController(WorkyDbContext dbContext, ILogger<AuthorizationController> logger, IJwtService jwtService
    , RoleManager<Roles> roleManager, SignInManager<Users> signInManager, UserManager<Users> userManager, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _logger = logger;
        _jwtService = jwtService;
        
        _roleManager = roleManager;
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestContract registerRequest)
    {
      
        try
        {
            var connectionString0 = _configuration.GetConnectionString("DefaultConnection");
            
                var user = new Users
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = registerRequest.UserName,
                    Email = registerRequest.Email,
                    PhoneNumber = registerRequest.PhoneNumber,
                }; 
                // Добавление пользователя
                var resultCreateUser = await _userManager.CreateAsync(user, registerRequest.PasswordHash);

                if (!resultCreateUser.Succeeded)
                {
                    return BadRequest(resultCreateUser.Errors);
                }
                // Добавление роли
                Roles userRole = _roleManager.FindByNameAsync(registerRequest.Role.ToString()).Result;
                if (userRole == null)
                {
                    return BadRequest("user role doesn't exist");
                }

                await _userManager.AddToRoleAsync(user, userRole.Name);
            using (IDbConnection db = new MySqlConnection(connectionString0))
            {
                if (registerRequest.Role == "Company")
                {

                    if (registerRequest.longitude is null || registerRequest.latitude is null)
                    {
                        return BadRequest("Longitude and Latitude are required.");
                    }
                    
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
                        Name = registerRequest.name,
                        Email = registerRequest.email_info,
                        PhoneNumber = registerRequest.phone_info,
                        PointWKT = $"POINT({registerRequest.longitude} {registerRequest.latitude})",
                        Website = registerRequest.website,
                        createdBy = user.UserName
                    };
                    try
                    {
                        await db.ExecuteAsync(sql, parameters);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "failed to create company in db");
                        await _userManager.DeleteAsync(user);
                        return BadRequest(500);
                    }
                    


                }
                else if (registerRequest.Role == "Worker")
                {
                    Worker newWorker = new Worker()
                    {
                        id = user.Id,
                        first_name = registerRequest.first_name,
                        second_name = registerRequest.second_name,
                        surname = registerRequest.surname,
                        birthday = registerRequest.birthday.Value
                    };
                    try
                    {
                        await _dbContext.Workers.AddAsync(newWorker);
                        await _dbContext.SaveChangesAsync();

                        string sqlQuery = @"UPDATE Worker set createdBy = @CreatedBy where id = @id;";
                        await db.ExecuteAsync(sqlQuery, new { Createdby = user.UserName, id = user.Id });
                        
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "failed to create worker in db");
                        await _userManager.DeleteAsync(user);
                        return BadRequest(500);
                    }
                }
            }
                //Создание юзера, удалить к ВКР
                var passwordUser = _dbContext.Users.Where(u => u.Email == registerRequest.Email)
                    .Select(u => u.PasswordHash).FirstOrDefault();

                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    string nameUser = user.UserName;
                    string dropUserQuery = "DROP USER IF EXISTS " + $"'{nameUser}'@'localhost';";
                    await db.ExecuteAsync(dropUserQuery);
                    string sqlQuery = $"CREATE USER '{nameUser}'@'localhost' IDENTIFIED BY '{passwordUser}';";
                    await db.ExecuteAsync(sqlQuery);

                    string grantRole = registerRequest.Role.ToString() switch
                    {
                        "Company" => "company_role",
                        "Worker" => "worker_role",
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    string grantRoleQuery = $"GRANT '{grantRole}' TO '{nameUser}'@'localhost';";
                    await db.ExecuteAsync(grantRoleQuery);

                    string SetRoleQuery = $"set default role {grantRole} for '{nameUser}'@'localhost';";
                    await db.ExecuteAsync(SetRoleQuery);
                }

         
                // _userManager.
                return Ok(new
                {
                    Message = "User created a new account with password.",
                    UserId = user.Id,
                });
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "failed to register request");
            return BadRequest(500);
        }
       
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        try
        {
            var user = _userManager.FindByEmailAsync(loginRequest.Email).Result;
            if (user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                
                var jwt = _jwtService.GenerateToken(Guid.Parse(user.Id), userRoles);
                
                
                return Ok(new LoginResponse
                {
                    Id = user.Id,
                    Token = jwt,
                    Role = userRoles,
                });
            }
            return Unauthorized();
          
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "failed to login request");
            return BadRequest(500);
        }
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("Claims")]
    public async Task<IActionResult> GetClaims()
    {
        try
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var role = User.FindFirst(ClaimTypes.Role).Value;
            return Ok(new { Id = id, Role = role });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "failed to get claims request");
            return BadRequest(500);
        }
        
    }
}