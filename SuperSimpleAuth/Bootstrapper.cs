using System;
using Nancy;
using Nancy.TinyIoc;

namespace SuperSimple.Auth.Api
{
	public class Bootstrapper : DefaultNancyBootstrapper
	{
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            IRepository repository = new MongoRepository ("mongodb://localhost");
            container.Register<IRepository>(repository);
        }

        protected override void RequestStartup (TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines, 
                                                NancyContext context)
        {
            base.RequestStartup (container, pipelines, context);

//            string app_key = context.Request.Headers ["ssa_app_key"];
//            string app = context.Request.Headers ["ssa_app"];
//
//            IRepository repository = container.Resolve<IRepository> ();
//
//            if (repository.ValidateAppKey (app, Guid.Parse (app_key)) == false) {
//                throw new Exception ("App key not valid");
//            }
        }
	}
}

