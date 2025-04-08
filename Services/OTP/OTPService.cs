using ShopEProduction.Services.OTP.IOTP;

namespace ShopEProduction.Services.OTP
{
    public class OTPService : IOTPService
    {
        public async Task<KeyValuePair<string, string>> GenerateOtp(string sessionId)
        {
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            return await Task.FromResult(new KeyValuePair<string, string>(sessionId, otp.ToString()));
        }


        public async Task<bool> VerifyOTPAsync(string sessionId, string otp, string sessionOtp)
        {
            KeyValuePair<string, string> sessionOtpPair = new KeyValuePair<string, string>(sessionId, sessionOtp);
            if (sessionOtpPair.Key == sessionId && sessionOtpPair.Value == otp)
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }
    }
}
