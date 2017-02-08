namespace SuperSimple.Auth.Manager
{
    public class SettingsModel : PageModel
    {
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string DeletePassword { get; set; }
        public string Email { get; set; }
    }
}

