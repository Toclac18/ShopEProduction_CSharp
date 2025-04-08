namespace ShopEProduction.Services.SMS.ISMS
{
    public interface ISmsService
    {
        Task SendSmsAsync(string toPhoneNumber, string message);
    }

}
