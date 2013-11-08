using Nancy.ModelBinding;
using Nancy.Validation;
using System.Collections.Generic;
using MongoDB.Driver.Builders;
using System.Runtime.InteropServices;
using MongoDB.Driver.GeoJsonObjectModel;
using System.Linq;
using System.Collections.Specialized;
using System.Web.Security;
using System.Security.Cryptography.X509Certificates;

namespace SSAManager
{
    using System;
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Responses.Negotiation;
    using Nancy.Security;

    public class HomeModule : NancyModule
    {
        IRepository repository; 

        public HomeModule(IRepository repository){

            this.repository = repository;

            this.RequiresAuthentication ();
            
            Get["/home"] = parameters => {
                ManageModel manage = new ManageModel();
                manage.Manager  = (Manager)this.Context.CurrentUser;
                manage.Apps = repository.GetApps(manage.Manager.Id);
               

                return View["manage", manage];
            };

            Get ["/app/new"] = parameters => {
                AppModel model = new AppModel();
                model.Manager  = (Manager)this.Context.CurrentUser;
                model.App = new App();
                return View["app_new", model];
            };

            Post ["/app/new"] = parameters => {
                AppModel model = this.Bind<AppModel>();
                App app = new App();
                app.Name = model.Name;
                model.Manager = (Manager)this.Context.CurrentUser;
                var result = this.Validate(app);
                model.Errors = result.Errors;

                if (!result.IsValid)
                {
                    return View["app_new", model];
                }
               
                app.Key = Guid.NewGuid();
                app.ManagerId = model.Manager.Id;
                app.ModifiedBy = model.Manager.UserName;

                repository.CreateApp(app);

                return this.Response.AsRedirect("/home");
            };

            Get ["/app/{name}"] = parameters => {
                AppModel model = new AppModel();
                model.Manager  = (Manager)this.Context.CurrentUser;
                model.App = repository.GetApp(model.Manager.Id,(string)parameters.name);
                model.Roles = repository.GetRoles(model.App.Id).ToList();
                model.Users = repository.GetAppUsers(model.App.Id).ToList();

                return View["app", model];
            };

            Post["/app/{name}"] = parameters => {
                AppModel model = this.Bind<AppModel>();
                model.Manager  = (Manager)this.Context.CurrentUser;
                App app = repository.GetApp(model.Manager.Id,(string)parameters.name);
                model.Roles = repository.GetRoles(app.Id).ToList();
                model.Users = repository.GetAppUsers(app.Id).ToList();


                app.WhiteListIps = model.App.WhiteListIps;

                if(Request.Form.Generate != null)
                {
                    app.Key = Guid.NewGuid();
                }

                repository.UpdateApp(app);

                return View["app", model];

            };

            Get ["/app/{name}/role/{role}"] = parameters => {
                RoleModel model = new RoleModel();
                model.Manager  = (Manager)this.Context.CurrentUser;
                model.App  = repository.GetApp(model.Manager.Id,(string)parameters.name);
                model.Users = repository.GetAppUsers(model.App.Id).ToList();
                model.Role = repository.GetRole(model.App.Id,(string)parameters.role);
                User[] us = repository.GetUsersInRole(model.Role);

                List<string> usStringified = new List<string>();

                foreach(User u in us)
                {
                    usStringified.Add(u.Id.ToString());
                }

                model.Claims = model.Role.Claims;
                model.RoleUsers = usStringified;
                return View["role", model];
            };

            
            Post ["/app/{name}/role/{role}"] = parameters => {
                RoleModel model = this.Bind<RoleModel>();

                model.Manager = (Manager)this.Context.CurrentUser;
                model.App = repository.GetApp(model.Manager.Id,(string)parameters.name);

                Role role = repository.GetRole(model.App.Id,(string)parameters.role);

                if(model.Claims != null)
                {
                    role.Claims = model.Claims.ToList();
                    model.Role = repository.UpdateRole(role);
                }
                else
                {
                    model.Role = role; 
                }

                User user = null;

                if(model.RoleUsers != null)
                {
                    foreach(string u in model.RoleUsers){
                        user = repository.GetUser(Guid.Parse(u));

                        if(!user.InRole(role.Name))
                        {
                            if(user.Roles == null)
                            {
                                user.Roles = new List<Role>();
                            }

                            user.AddRole(role);
                            repository.UpdateUser(user);
                        }
                    }
                }

                if(model.uRoleUsers != null)
                {
                    foreach(string u in model.uRoleUsers){
                        user = repository.GetUser(Guid.Parse(u));

                        if(user.InRole(role.Name))
                        {
                            if(user.Roles == null)
                            {
                                user.Roles = new List<Role>();
                            }

                            user.RemoveRole(model.Role);
                            repository.UpdateUser(user);
                        }
                    }
                }

                model.Users = repository.GetAppUsers(model.App.Id).ToList();

                return View["role", model];
            };

            Get ["/app/{name}/role/new"] = parameters => {
                Role role = new Role ();
                role.Manager  = (Manager)this.Context.CurrentUser;
                App app = repository.GetApp(role.Manager.Id,(string)parameters.name);
                role.AppId = app.Id;
                role.AppName = app.Name;

                return View["role_new", role];
            };

            Post ["/app/{name}/role/new"] = parameters => {
                Role role = this.Bind<Role>();
                role.Manager  = (Manager)this.Context.CurrentUser;
                App app = repository.GetApp(role.Manager.Id,(string)parameters.name);
                role.AppId = app.Id;
                role.AppName = app.Name;

                var result = this.Validate(role);
                role.Errors = result.Errors;

                if (!result.IsValid)
                {
                    return View["role_new", role];
                }

                repository.CreateRole(role);

                return this.Response.AsRedirect("/app/" + app.Name);
            };

            Get ["/app/{name}/claim/new"] = parameters => {
                Claim claim = new Claim ();
                claim.AppName = (string)parameters.name;

                return View["claim_new", claim];
            };

            Post ["/app/{name}/claim/new"] = parameters => {
                ClaimModel model = this.Bind<ClaimModel>();

                Manager manager = (Manager)this.Context.CurrentUser;
                Claim claim = this.Bind<Claim>();
                claim.AppName = (string)parameters.name;

                var result = this.Validate(claim);
                claim.Errors = result.Errors;

                if (!result.IsValid)
                {
                    return View["claim_new", claim];
                }


                App update = repository.GetApp(manager.Id,claim.AppName);
                List<string> claims = new List<string>(); 

                if(update.Claims != null)
                {
                    claims = new List<string>(update.Claims);
                }

                claims.Add(claim.Name);
                update.Claims = claims.ToList();
                repository.UpdateApp(update);

                return this.Response.AsRedirect("/app/" + claim.AppName);
            };

            Get ["/app/{name}/user/{id}"] = parameters => {
                AppUserModel model = new AppUserModel();

                model.Manager = (Manager)this.Context.CurrentUser;
                model.App = repository.GetApp(model.Manager.Id,(string)parameters.name);
                model.User = repository.GetUser(Guid.Parse((string)parameters.id));
                model.Roles = repository.GetRoles(model.App.Id).ToList();

                List<string> uRoles = new List<string>();

                if(model.User.Roles != null)
                {
                    foreach(Role r in model.User.Roles)
                    {
                        uRoles.Add(r.Name);
                    }
                }

                model.NewRoles = uRoles;
                model.Claims = model.User.Claims;
               
                return View["user", model];
            };

            Post ["/app/{name}/user/{id}"] = parameters => {
                AppUserModel model = this.Bind<AppUserModel>();
                model.Manager = (Manager)this.Context.CurrentUser;
                model.App = repository.GetApp(model.Manager.Id,(string)parameters.name);
                model.User = repository.GetUser(Guid.Parse((string)parameters.id));
                model.Roles = repository.GetRoles(model.App.Id).ToList();

                List<Role> uroles = new List<Role>();
            
                if(model.NewRoles != null)
                {
                    foreach(string r in model.NewRoles)
                    {
                        uroles.Add(repository.GetRole(model.App.Id, r));
                    }
                }
               
                model.User.Roles = uroles;
                model.User.Claims = model.Claims;
               
                try
                {
                    model.User = repository.UpdateUser(model.User);
                }
                catch(Exception e)
                {
                    model.Debug = e.Message;
                }

                return View["user", model];

            };

        }
    }
}

