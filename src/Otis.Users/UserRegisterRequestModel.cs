using System.ComponentModel.DataAnnotations;

namespace Otis.Users;

public sealed class UserRegisterRequestModel
{
    [Required]
    [MinLength(3)]
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    public string? EmailAddress { get; set; }
};
