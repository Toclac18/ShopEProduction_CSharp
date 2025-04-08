namespace ShopEProduction.DTOs
{
    public class ForgotPasswordDto
    {
        public string Username { get; set; }
        public string SelectedMethod { get; set; }
        public string OTP { get; set; }
        public bool IsOTPStage { get; set; } = false;
        public bool IsSelectedMethod { get; set; } = false;
    }
}
