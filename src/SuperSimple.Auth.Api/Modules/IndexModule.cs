namespace SuperSimple.Auth.Api
{
    using Nancy;
    public class IndexModule : NancyModule
    {
        public IndexModule ()
        {
            Get ["/"] = parameters =>
            {
                return View ["index"];
            };

            Get ["/test"] = parameters =>
            {
                return Response.AsJson (Request.Headers);
            };
        }
    }
}
