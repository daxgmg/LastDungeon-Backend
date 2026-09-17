namespace LastDungeon.Api.Services;

public interface IEmailService
{
    Task SendVerificationCodeAsync(string toEmail, string code);
}
