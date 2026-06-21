using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MyMvcApp.Models;
using MyMvcApp.Models.Settings;

namespace MyMvcApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendAppointmentConfirmationAsync(Appointment appointment)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(new MailboxAddress(appointment.Name, appointment.Email));
            message.Subject = "Your Prime Dental Appointment is Confirmed";

            var body = new BodyBuilder
            {
                HtmlBody = BuildHtmlBody(appointment)
            };

            message.Body = body.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.SenderEmail, _settings.AppPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        private static string BuildHtmlBody(Appointment appointment)
        {
            return $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; border-radius: 8px; overflow: hidden;'>
                <div style='background-color: #8a7aff; padding: 24px; text-align: center;'>
                    <h2 style='color: #ffffff; margin: 0;'>Prime Dental</h2>
                </div>
                <div style='padding: 24px;'>
                    <h3 style='color: #333;'>Hi {appointment.Name}, your appointment is confirmed!</h3>
                    <p style='color: #555;'>Here are your appointment details:</p>
                    <table style='width: 100%; border-collapse: collapse; margin-top: 12px;'>
                        <tr>
                            <td style='padding: 8px 0; color: #777;'>Service</td>
                            <td style='padding: 8px 0; font-weight: bold;'>{appointment.Service}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px 0; color: #777;'>Date</td>
                            <td style='padding: 8px 0; font-weight: bold;'>{appointment.AppointmentDate:MMMM d, yyyy}</td>
                        </tr>
                        <tr>
                            <td style='padding: 8px 0; color: #777;'>Name</td>
                            <td style='padding: 8px 0; font-weight: bold;'>{appointment.Name}</td>
                        </tr>
                    </table>
                    <p style='margin-top: 24px; color: #555;'>
                        If you need to reschedule or have any questions, just reply to this email.
                    </p>
                </div>
                <div style='background-color: #f7f7f7; padding: 16px; text-align: center; color: #999; font-size: 12px;'>
                    Prime Dental &mdash; Al-Remal, Khaled Bin Al-Waleed, Gaza
                </div>
            </div>";
        }
    }
}
