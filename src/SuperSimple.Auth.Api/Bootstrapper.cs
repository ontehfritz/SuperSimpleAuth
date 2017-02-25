namespace SuperSimple.Auth.Api
{
    using Nancy;
    using Nancy.TinyIoc;
    using Nancy.Bootstrapper;
    using System.Configuration;
    using Repository;

	public class Bootstrapper : DefaultNancyBootstrapper
	{
        protected override void ApplicationStartup(TinyIoCContainer container, 
                                                   IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            StaticConfiguration.DisableErrorTraces = false;
            StaticConfiguration.EnableRequestTracing = true;
        }

        protected override void 
        ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            var repository = 
                new ApiMongoRepository (ConfigurationManager
                                        .AppSettings.Get("db"));
            container.Register<IApiRepository>(repository);
        }

        protected override void RequestStartup (TinyIoCContainer container, 
                                                IPipelines pipelines, 
                                                NancyContext context)
        {
            base.RequestStartup (container, pipelines, context);
        }
	}
}

