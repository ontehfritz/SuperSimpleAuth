namespace SuperSimple.Auth.Manager
{
    using Api.Repository;
    using SuperSimple.Auth.Api;
    using SuperSimple.Auth.Manager.Repository;

    public class AdminModel : PageModel
    {
        public string Email     { get; set; }        
        public Domain Domain    { get; set; }
        public IUser Admin       { get; set; }
    }
}

