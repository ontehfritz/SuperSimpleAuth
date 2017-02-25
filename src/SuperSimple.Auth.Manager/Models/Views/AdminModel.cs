namespace SuperSimple.Auth.Manager
{
    using Api;
    using Repository;

    public class AdminModel : PageModel
    {
        public string Email     { get; set; }        
        public Domain Domain    { get; set; }
        public IUser Admin       { get; set; }
    }
}

