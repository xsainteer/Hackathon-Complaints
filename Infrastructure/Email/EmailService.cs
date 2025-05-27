using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Infrastructure.Email;

public class EmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpOptions, SmtpClient smtpClient)
    {
        _smtpSettings = smtpOptions.Value;
        _smtpClient = smtpClient;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var message = new MailMessage();
        message.From = new MailAddress(_smtpSettings.Username);
        message.To.Add(new MailAddress(toEmail));
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = true;

        await _smtpClient.SendMailAsync(message);
    }
}
