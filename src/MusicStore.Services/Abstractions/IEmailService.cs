namespace MusicStore.Services.Abstractions;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
}
