namespace SuperSimple.Auth.Manager
{
    using System.Configuration;
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;
    using SuperSimple.Auth.Api.Repository;
    using SuperSimple.Auth.Manager.Repository;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup (TinyIoCContainer container,
                                                   IPipelines pipelines)
        {
            base.ApplicationStartup (container, pipelines);
            StaticConfiguration.DisableErrorTraces = false;
            StaticConfiguration.EnableRequestTracing = true;
        }

        protected override void ConfigureApplicationContainer (TinyIoCContainer
                                                              container)
        {
            base.ConfigureApplicationContainer (container);

            IApiRepository apiRepository =
                new ApiMongoRepository (ConfigurationManager
                                       .AppSettings.Get ("db"));

            container.Register<IApiRepository> (apiRepository);
            IRepository repository =
                new MongoRepository (ConfigurationManager.AppSettings.Get ("db"),
                                     apiRepository);
            container.Register<IRepository> (repository);


        }


        protected override void ConfigureRequestContainer (TinyIoCContainer container,
                                                          NancyContext context)
        {
            base.ConfigureRequestContainer (container, context);
            container.Register<IUserMapper, ManagerMapper> ();
        }

        protected override void RequestStartup (TinyIoCContainer container,
                                               IPipelines pipelines,
                                               NancyContext context)
        {
            base.RequestStartup (container, pipelines, context);

            var formsAuthConfiguration = new FormsAuthenticationConfiguration
            {
                RedirectUrl = "~/logon",
                UserMapper = container.Resolve<IUserMapper> (),
            };

            FormsAuthentication.Enable (pipelines, formsAuthConfiguration);
        }
    }
}

