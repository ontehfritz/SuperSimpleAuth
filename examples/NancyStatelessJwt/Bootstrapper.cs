namespace NancyStatelessJwt
{
    using System;
    using System.Security.Claims;
    using System.Security.Principal;
    using Nancy;
    using Nancy.Authentication.Stateless;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;
    using System.Collections.Generic;
    using SuperSimple.Auth;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        /// <summary>
        /// Change these settings to match local or online server settings
        /// </summary>
        //*****************************************************************//
        private string _domain = "test";
        private string _domainCode = "9fa95fd4-04e5-4cea-8dca-3a818f525a91";
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
                new SuperSimpleAuth ( _domainCode, 
                                     _url);

            container.Register<SuperSimpleAuth>(ssa);
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, 
                                                          NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

        }

        protected override void RequestStartup(TinyIoCContainer container, 
                                               IPipelines pipelines, 
                                               NancyContext context)
        {
            var configuration =
                new StatelessAuthenticationConfiguration(ctx =>
            {
                var jwtToken = ctx.Request.Headers.Authorization;

                if(string.IsNullOrEmpty(jwtToken))
                {
                    jwtToken = ctx.Request.Query.Authorization;
                }

                if(string.IsNullOrEmpty(jwtToken))
                {
                    jwtToken = ctx.Request.Form.Authorization;
                }

                try
                {
                    var ssa = container.Resolve<SuperSimpleAuth>();

                    var user = ssa.Validate(jwtToken);
                    //claims if using later versions of nancy
                    //var claims = new List<Claim>();
                    //foreach(var role in user.Roles)
                    //{
                    //    foreach(var permission in role.Permissions)
                    //    {
                    //        claims.Add(new Claim(role.Name, permission));
                    //    }
                    //}

                    //var identity = new ClaimsIdentity(
                    //    new  GenericIdentity(user.UserName),
                    //    claims,
                    //    "Jwt",
                    //    "SSA",
                    //    "SSA");
                    var identity = new NancyUserIdentity(user);
                    //return new ClaimsPrincipal(identity);
                    return identity;

                }
                catch (Exception exc)
                {
                    return null;
                }
            });



            StatelessAuthentication.Enable(pipelines, configuration);
           
        }
    }
}
