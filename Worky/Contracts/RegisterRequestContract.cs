using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;
using Worky.Models;

namespace Worky.Contracts;

public class RegisterRequestContract
{
    [Required(ErrorMessage = "UserId is required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    public string Email { get; set; }

    [Phone] public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string PasswordHash { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; }

    public string? name { get; set; }

    // public Point? Office_coord { get; set; }
    public string? latitude { get; set; }
    public string? longitude { get; set; }
    [EmailAddress] public string? email_info { get; set; }
    [Phone] public string? phone_info { get; set; }
    [Url] public string? website { get; set; }

    public string? second_name { get; set; }
    public string? first_name { get; set; }
    public string? surname { get; set; }
    public DateOnly? birthday { get; set; }
}