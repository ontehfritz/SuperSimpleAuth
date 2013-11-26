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
using System;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Responses.Negotiation;
using Nancy.Security;

namespace SSAManager
{
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
                
                if (!result.IsValid)
                {
                    model.Errors = Helpers.GetValidationErrors(result);
                    return View["app_new", model];
                }

      		    try
      		    {
                    repository.CreateApp(model.Name, model.Manager);
      		    }
      			catch(MongoDB.Driver.WriteConcernException e)
      		    {
                    Error error = new Error();
                    error.Name = "Duplicate";
                    error.Message = "Cannot create application, name already exists.";
                    model.Errors = new List<Error>();
                    model.Errors.Add(error);

      			    return View["app_new", model];
      		    }

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

                if(Request.Form.Delete != null)
                {
                    repository.DeleteApp(app.Name,model.Manager.Id);
                    return this.Response.AsRedirect ("/");
                }

                if(Request.Form.Disable != null)
                {
                    app.Enabled = false;
                }

                if(Request.Form.Enable != null)
                {
                    app.Enabled = true;
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

                model.Role = repository.GetRole(model.App.Id,(string)parameters.role);

                if(Request.Form.Delete != null)
                {
                    repository.DeleteRole(model.Role);
                    return this.Response.AsRedirect(string.Format("/app/{0}",(string)parameters.name));
                }


                model.Role.Claims = model.Claims == null ? new List<string>() : model.Claims.ToList();
                model.Role = repository.UpdateRole(model.Role);


                User user = null;

                if(model.RoleUsers != null)
                {
                    foreach(string u in model.RoleUsers){
                        user = repository.GetUser(Guid.Parse(u));

                        if(!user.InRole(model.Role.Name))
                        {
                            if(user.Roles == null)
                            {
                                user.Roles = new List<Role>();
                            }

                            user.AddRole(model.Role);
                            repository.UpdateUser(user);
                        }
                    }
                }

                if(model.uRoleUsers != null)
                {
                    foreach(string u in model.uRoleUsers){
                        user = repository.GetUser(Guid.Parse(u));

                        if(user.InRole(model.Role.Name))
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


                if (!result.IsValid)
                {
                    model.Errors = Helpers.GetValidationErrors(result);
                    return View["role_new", model];
                }

                try
                {
                    repository.CreateRole(model.App.Id, model.Name);
                }
                catch(Exception e)
                {
                    model.Errors = new List<Error>();
                    Error error = new Error();
                    error.Name = e.ToString();
                    error.Message = e.Message;
                    model.Errors.Add(error);
                    return View["role_new", model];
                }

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
                
                if (!result.IsValid)
                {
                    model.Errors = Helpers.GetValidationErrors(result);
                    return View["claim_new", model];
                }

                App update = repository.GetApp(model.App.Name, model.Manager.Id);
                List<string> claims = new List<string>(); 

                if(update.Claims != null)
                {
                    claims = new List<string>(update.Claims);
                }


                if(claims.Contains(model.Name))
                {
                    Error error = new Error();
                    error.Name = "Duplicate";
                    error.Message = "Claim already exists.";

                    model.Errors = new List<Error>();
                    model.Errors.Add(error);

                    return View["claim_new", model];
                }

                claims.Add(model.Name);
                update.Claims = claims.ToList();

                try
                {
                    repository.UpdateApp(update);
                }
                catch(Exception e)
                {
                    model.Errors = new List<Error>();
                    Error error = new Error();
                    error.Name = e.ToString();
                    error.Message = e.Message;
                    model.Errors.Add(error);
                    return View["claim_new", model];
                }

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

