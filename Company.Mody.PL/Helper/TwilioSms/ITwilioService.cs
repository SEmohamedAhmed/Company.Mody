using Twilio.Rest.Api.V2010.Account;

namespace Company.Mody.PL.Helper.TwilioSms
{
    public interface ITwilioService
    {
        public MessageResource SendSms(Sms sms);
    }
}
