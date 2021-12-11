using System.ComponentModel.DataAnnotations;

namespace SchoolExam.Web.Authentication;

public class AuthenticateModel
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}