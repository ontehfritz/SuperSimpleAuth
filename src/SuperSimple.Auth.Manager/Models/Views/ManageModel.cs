namespace SuperSimple.Auth.Manager
{
    public class ManageModel : PageModel
    {
        public Domain[] Domains      { get; set; }
        public Domain[] AdminDomains { get; set; }
    }
}

