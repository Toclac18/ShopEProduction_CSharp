namespace ShopEProduction.Services.OTP.IOTP
{
    public interface IOTPService
    {
        Task<bool> VerifyOTPAsync(string sessionId, string otp, string sessionOtp);
        Task<KeyValuePair<String, String>> GenerateOtp(string sessionId);
    }
}
