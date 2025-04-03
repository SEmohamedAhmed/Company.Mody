using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Company.Mody.PL.Helper.MailKitHelper

{
    public class MailService(IOptions<MailSettings> _options) : IMailService
    {


        public void SendEmail(Email email)
        {

            var mail = new MimeMessage();

            mail.Subject = email.Subject;
            mail.From.Add(MailboxAddress.Parse(_options.Value.Email));
            mail.To.Add(MailboxAddress.Parse(email.To));


            var builder = new BodyBuilder();
            builder.TextBody = email.Body;
            mail.Body = builder.ToMessageBody();

            var smtp = new SmtpClient();
            smtp.Connect(_options.Value.Host, _options.Value.Port, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.Value.Email, _options.Value.Password);



            smtp.Send(mail);

        }
    }
}
