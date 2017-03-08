namespace SSANancyExample
{
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;
    using SuperSimple.Auth;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        /// <summary>
        /// Change these settings to match local or online server settings
        /// </summary>
        //*****************************************************************//
        private string _domain = "test";
        private string _domainCode = "480ebe37-4653-4f55-9e3a-bccf6944c126";
        private string _url = "http://127.0.0.1:8082";
        //*****************************************************************//

        protected override void ApplicationStartup(TinyIoCContainer container, 
                                                   IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            StaticConfiguration.DisableErrorTraces = false;
            StaticConfiguration.EnableRequestTracing = true;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer 
                                                              container)
        {
            base.ConfigureApplicationContainer(container);

            //some encryption on the keys could be used. 

            var ssa = 
                new SuperSimpleAuth (_domain, 
                                     _domainCode, 
                                     _url);

            container.Register<SuperSimpleAuth>(ssa);
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, 
                                                          NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
            //container.Register<IUserMapper, NancyUserMapper>();
        }
       
        protected override void RequestStartup(TinyIoCContainer container, 
                                               IPipelines pipelines, 
                                               NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            var formsAuthConfiguration = new FormsAuthenticationConfiguration
            {
                RedirectUrl = "~/logon",
                UserMapper = container.Resolve<IUserMapper>(),
            };

            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        }
    }
}

