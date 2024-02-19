using ComigleApi.Model.Email;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ComigleApi.Services
{
    public class EmailService
    {
        public async Task<SendEmailResponse> SendEmail(string from, string to, string subject, string body)
        {
            var emailResponse = new SendEmailResponse() { Success = true };
            var client = new SendGridClient("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");

            var emailFrom = new EmailAddress(from);
            var emailTo = new EmailAddress(to);
            var emailMessage = MailHelper.CreateSingleEmail(emailFrom, emailTo, subject, "", body);

            var response = await client.SendEmailAsync(emailMessage);
            if (!response.IsSuccessStatusCode)
            {
                emailResponse.Success = false;
                emailResponse.Message = response.Body.ReadAsStringAsync().Result;
            }

            return emailResponse;
        }
    }
}
