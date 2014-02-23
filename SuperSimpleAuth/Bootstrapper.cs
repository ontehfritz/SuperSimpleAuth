using System;
using Nancy;
using Nancy.TinyIoc;
using Nancy.Bootstrapper;
using System.Configuration;

namespace SuperSimple.Auth.Api
{
	public class Bootstrapper : DefaultNancyBootstrapper
	{
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            StaticConfiguration.DisableErrorTraces = false;
            StaticConfiguration.EnableRequestTracing = true;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            IRepository repository = new MongoRepository (ConfigurationManager.AppSettings.Get("db"));
            container.Register<IRepository>(repository);
        }

        protected override void RequestStartup (TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines, 
                                                NancyContext context)
        {
            base.RequestStartup (container, pipelines, context);
        }
	}
}

