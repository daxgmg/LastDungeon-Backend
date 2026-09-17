using System.Net;
using System.Net.Mail;

namespace LastDungeon.Api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendVerificationCodeAsync(string toEmail, string code)
    {
        var smtpHost = _configuration["Smtp:Host"];
        var smtpPortString = _configuration["Smtp:Port"];
        var smtpUser = _configuration["Smtp:Username"];
        var smtpPass = _configuration["Smtp:Password"];
        var fromEmail = _configuration["Smtp:FromEmail"] ?? "noreply@lastdungeon.com";
        var fromName = _configuration["Smtp:FromName"] ?? "LastDungeon Game";
        var useConsole = _configuration.GetValue<bool>("Smtp:UseConsole", true);

        // Si el modo consola está habilitado o no hay host SMTP configurado, imprimir en logs
        if (useConsole || string.IsNullOrWhiteSpace(smtpHost))
        {
            _logger.LogInformation(
                "\n======================================================\n" +
                " [EMAIL CONSOLE MODE] Código de verificación para: {ToEmail}\n" +
                " CÓDIGO: {Code}\n" +
                " Válido por 10 minutos.\n" +
                "======================================================",
                toEmail, code);
            return;
        }

        int smtpPort = int.TryParse(smtpPortString, out var parsedPort) ? parsedPort : 587;
        var enableSsl = _configuration.GetValue<bool>("Smtp:EnableSsl", true);

        try
        {
            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = enableSsl,
                Credentials = !string.IsNullOrWhiteSpace(smtpUser) && !string.IsNullOrWhiteSpace(smtpPass)
                    ? new NetworkCredential(smtpUser, smtpPass)
                    : null
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = "Tu código de acceso a LastDungeon",
                Body = $"<h1>Bienvenido a LastDungeon</h1><p>Tu código de verificación de 6 dígitos es:</p><h2><strong>{code}</strong></h2><p>Este código expirará en 10 minutos.</p>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Correo de verificación enviado exitosamente a {ToEmail}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo de verificación a {ToEmail}. Código fallback: {Code}", toEmail, code);
            // Fallback en consola en caso de fallo SMTP para no bloquear al usuario en desarrollo
            _logger.LogWarning("[FALLBACK LOG] Código de verificación para {ToEmail}: {Code}", toEmail, code);
        }
    }
}
