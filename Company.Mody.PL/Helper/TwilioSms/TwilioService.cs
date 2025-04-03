using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Company.Mody.PL.Helper.TwilioSms
{
    public class TwilioService(IOptions<TwilioSettings> _options) : ITwilioService
    {
        public MessageResource SendSms(Sms sms)
        {
            // initialize connections
            TwilioClient.Init(_options.Value.AccountSID, _options.Value.AuthToken);

            // build Message
            var message = MessageResource.Create(
                body:sms.Body,
                to:sms.To,
                from: _options.Value.PhoneNumber
                
            );
            return message;
        }
    }
}
