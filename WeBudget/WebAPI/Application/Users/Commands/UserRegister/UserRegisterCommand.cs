using MediatR;
using System.ComponentModel.DataAnnotations;
using WebAPI.Shared.Abstractions;

namespace WebAPI.Application.Users
{
    public record UserRegisterCommand : IRequest<Result>
    {
        [Required]
        public string Nickname { get; init; } = string.Empty;

        [Required]
        public string Email { get; init; } = string.Empty;

        [Required]
        public string Password { get; init; } = string.Empty;

        public string? ConfirmationLink { get; set; } = string.Empty;
    }
}
