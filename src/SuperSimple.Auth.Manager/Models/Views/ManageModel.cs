namespace SuperSimple.Auth.Manager
{
    using Repository;

    public class ManageModel : PageModel
    {
        public Domain[] Domains      { get; set; }
        public Domain[] AdminDomains { get; set; }
    }
}

