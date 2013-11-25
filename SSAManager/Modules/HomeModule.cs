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
                model.Manager = (Manager)this.Context.CurrentUser;
                var result = this.Validate(model);
                model.Errors = result.Errors;

                if (!result.IsValid)
                {
                    return View["app_new", model];
                }
        
                repository.CreateApp(model.Name, model.Manager);

                return this.Response.AsRedirect("/home");
            };

            Get ["/app/{name}"] = parameters => {
                AppModel model = new AppModel();
                model.Manager  = (Manager)this.Context.CurrentUser;
                model.App = repository.GetApp((string)parameters.name, model.Manager.Id);
                model.Roles = repository.GetRoles(model.App.Id).ToList();
                model.Users = repository.GetAppUsers(model.App.Id).ToList();

                return View["app", model];
            };

            Post["/app/{name}"] = parameters => {
                AppModel model = this.Bind<AppModel>();
                model.Manager  = (Manager)this.Context.CurrentUser;
                App app = repository.GetApp((string)parameters.name, model.Manager.Id);
                model.Roles = repository.GetRoles(app.Id).ToList();
                model.Users = repository.GetAppUsers(app.Id).ToList();

                app.WhiteListIps = model.WhiteListIps;

                if(Request.Form.Generate != null)
                {
                    app.Key = Guid.NewGuid();
                }

                model.App = repository.UpdateApp(app);

                return View["app", model];

            };

            Get ["/app/{name}/role/{role}"] = parameters => {
                RoleModel model = new RoleModel();
                model.Manager  = (Manager)this.Context.CurrentUser;
                model.App  = repository.GetApp((string)parameters.name, model.Manager.Id);
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
                model.App = repository.GetApp((string)parameters.name, model.Manager.Id);

                Role role = repository.GetRole(model.App.Id,(string)parameters.role);

                if(Request.Form.Delete != null)
                {
                    repository.DeleteRole(role);
                    return this.Response.AsRedirect(string.Format("/app/{0}",(string)parameters.name));
                }

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
                CreateRoleModel model = new CreateRoleModel ();
                model.Manager  = (Manager)this.Context.CurrentUser;
                model.App = repository.GetApp((string)parameters.name, model.Manager.Id);
           
                return View["role_new", model];
            };

            Post ["/app/{name}/role/new"] = parameters => {
                CreateRoleModel model = this.Bind<CreateRoleModel>();
                model.Manager  = (Manager)this.Context.CurrentUser;
                model.App = repository.GetApp((string)parameters.name, model.Manager.Id);


                var result = this.Validate(model);
                model.Errors = result.Errors;

                if (!result.IsValid)
                {
                    return View["role_new", model];
                }

                repository.CreateRole(model.App.Id, model.Name);

                return this.Response.AsRedirect("/app/" + model.App.Name);
            };

            Get ["/app/{name}/claim/{cname}"] = parameters => {
                ClaimModel model = new ClaimModel ();
                model.Name = (string)parameters.cname;
                model.Title = "Claim";
                model.Manager = (Manager)this.Context.CurrentUser;
                model.App = repository.GetApp((string)parameters.name, model.Manager.Id);
                model.Users = repository.GetUsersWithClaim(model.App.Id,(string)parameters.cname);
                model.Roles = repository.GetRolesWithClaim(model.App.Id,(string)parameters.cname);

                return View["claim", model];
            };

            Post ["/app/{name}/claim/{cname}"] = parameters => {
                ClaimModel model = new ClaimModel ();
                model.Name = (string)parameters.cname;
                model.Manager = (Manager)this.Context.CurrentUser;
                model.App = repository.GetApp((string)parameters.name, model.Manager.Id);

                if (Request.Form.Delete != null) {
                    model.App.Claims.Remove(model.Name);
                    model.App = repository.UpdateApp(model.App);
                    Role[] roles = repository.GetRolesWithClaim(model.App.Id, model.Name);

                    if(roles != null)
                    {
                        foreach(Role r in roles){
                            r.Claims.Remove(model.Name);
                            repository.UpdateRole(r);
                        }
                    }

                    User[] users = repository.GetUsersWithClaim(model.App.Id, model.Name);

                    if(users != null){
                        foreach(User u in users){
                            u.Claims.Remove(model.Name);
                            repository.UpdateUser(u);
                        }
                    }

                    return this.Response.AsRedirect (string.Format ("/app/{0}", model.App.Name));
                }

                return View["claim", model];
            };

            Get ["/app/{name}/claim/new"] = parameters => {
                ClaimModel model = new ClaimModel ();
                model.Manager = (Manager)this.Context.CurrentUser;
                model.App = repository.GetApp((string)parameters.name, model.Manager.Id);
             
                return View["claim_new", model];
            };

            Post ["/app/{name}/claim/new"] = parameters => {
                ClaimModel model = this.Bind<ClaimModel>();
                model.Manager = (Manager)this.Context.CurrentUser;
                model.App = repository.GetApp((string)parameters.name, model.Manager.Id);

                var result = this.Validate(model);
                model.Errors = result.Errors;

                if (!result.IsValid)
                {
                    return View["claim_new", model];
                }


                App update = repository.GetApp(model.App.Name, model.Manager.Id);
                List<string> claims = new List<string>(); 

                if(update.Claims != null)
                {
                    claims = new List<string>(update.Claims);
                }

                claims.Add(model.Name);
                update.Claims = claims.ToList();
                repository.UpdateApp(update);

                return this.Response.AsRedirect("/app/" + model.App.Name);
            };

            Get ["/app/{name}/user/{id}"] = parameters => {
                AppUserModel model = new AppUserModel();

                model.Manager = (Manager)this.Context.CurrentUser;
                model.App = repository.GetApp((string)parameters.name, model.Manager.Id);
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
                model.App = repository.GetApp((string)parameters.name, model.Manager.Id);
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

