using ShopEProduction.Services.SMS.ISMS;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace ShopEProduction.Services.SMS
{
    public class TwilioSmsService : ISmsService
    {
        private readonly IConfiguration _config;

        public TwilioSmsService(IConfiguration config)
        {
            _config = config;
            var accountSid = _config["Twilio:AccountSID"];
            var authToken = _config["Twilio:AuthToken"];
            TwilioClient.Init(accountSid, authToken);
        }

        public async Task SendSmsAsync(string toPhoneNumber, string message)
        {
            var fromPhone = _config["Twilio:PhoneNumber"];

            var messageResult = await MessageResource.CreateAsync(
                to: new PhoneNumber(toPhoneNumber),
                from: new PhoneNumber(fromPhone),
                body: message
            );

            Console.WriteLine($"SMS sent. SID: {messageResult.Sid}");
        }
    }
}