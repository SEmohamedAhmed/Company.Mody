using System.Net;
using System.Net.Mail;

namespace Company.Mody.PL.Helper
{
    public static class EmailSettings
    {
        public static void SendEmail(Email email)
        {
            var client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            
            // sender info
            client.Credentials = new NetworkCredential("modyeldeeb96@gmail.com", "bgkkszfngusottlz"); // pass for this app not email pass
            
            

            client.Send("modyeldeeb96@gmail.com", 
                        email.To,
                        email.Subject, 
                        email.Body
                );


        }
    }
}
