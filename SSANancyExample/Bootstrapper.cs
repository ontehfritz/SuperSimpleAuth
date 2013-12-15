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
                new SuperSimpleAuth ("testing", 
                    "e7cfb785-4b37-4350-8897-cbd85af8fde9");

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

