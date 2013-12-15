using System;
using System.Collections.Generic;

namespace SSAManager
{
    public class SettingsModel : IPageModel
    {
        public List<Error> Errors { get; set; }
        public List<string> Messages { get; set; }
        public string Title { get; set; }
        public Manager Manager { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string DeletePassword { get; set; }
        public string Email { get; set; }
    }
}

