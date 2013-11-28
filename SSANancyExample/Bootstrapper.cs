using System;
using Nancy;
using Nancy.TinyIoc;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using SSANancyExample;
using SuperSimple.Auth;


namespace SSANancyExample
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            //some encryption on the keys could be used. 
            SuperSimpleAuth ssa = 
                new SuperSimpleAuth ("testing", "3a3891af-0125-4d12-9230-d11bedfb45b0","http://127.0.0.1:8082/");

            container.Register<SuperSimpleAuth>(ssa);
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, 
                                                          NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
            container.Register<IUserMapper, NancyUserMapper>();
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

