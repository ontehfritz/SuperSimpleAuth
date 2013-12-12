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
using MongoDB.Driver;

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
                manage.Domains = repository.GetDomains(manage.Manager.Id);
               

                return View["manage", manage];
            };

            Get ["/domain/new"] = parameters => {
                DomainModel model = new DomainModel();
                model.Manager  = (Manager)this.Context.CurrentUser;
                model.Domain = new Domain();
                return View["domain_new", model];
            };

            Post ["/domain/new"] = parameters => {
                DomainModel model = this.Bind<DomainModel>();
                model.Manager = (Manager)this.Context.CurrentUser;
                var result = this.Validate(model);
                
                if (!result.IsValid)
                {
                    model.Errors = Helpers.GetValidationErrors(result);
                    return View["domain_new", model];
                }

      		    try
      		    {
                    repository.CreateDomain(model.Name, model.Manager);
      		    }
      			catch(MongoDB.Driver.WriteConcernException e)
      		    {
                    Error error = new Error();
                    error.Name = "Duplicate";
                    error.Message = "Cannot create domain, name already exists.";
                    model.Errors = new List<Error>();
                    model.Errors.Add(error);

                    return View["domain_new", model];
      		    }

                return this.Response.AsRedirect("/home");
            };

            Get ["/domain/{name}"] = parameters => {
                DomainModel model = new DomainModel();
                model.Manager  = (Manager)this.Context.CurrentUser;
                model.Domain = repository.GetDomain((string)parameters.name, model.Manager.Id);
                model.Roles = repository.GetRoles(model.Domain.Id).ToList();
                model.Users = repository.GetDomainUsers(model.Domain.Id).ToList();

                return View["domain", model];
            };

            Post["/domain/{name}"] = parameters => {
                DomainModel model = this.Bind<DomainModel>();
                model.Manager  = (Manager)this.Context.CurrentUser;
                Domain domain = repository.GetDomain((string)parameters.name, model.Manager.Id);
                model.Roles = repository.GetRoles(domain.Id).ToList();
                model.Users = repository.GetDomainUsers(domain.Id).ToList();

                domain.WhiteListIps = model.WhiteListIps;

                if(Request.Form.Generate != null)
                {
                    domain.Key = Guid.NewGuid();
                }

                if(Request.Form.Delete != null)
                {
                    repository.DeleteDomain(domain.Name,model.Manager.Id);
                    return this.Response.AsRedirect ("/");
                }

                if(Request.Form.Disable != null)
                {
                    domain.Enabled = false;
                }

                if(Request.Form.Enable != null)
                {
                    domain.Enabled = true;
                }

                try
                {
                    model.Domain = repository.UpdateDomain(domain);
                    model.Messages.Add("Domain successfully updated");
                }
                catch(Exception e)
                {
                    Error error = new Error();
                    error.Name = e.GetType().ToString();
                    error.Message = e.Message;

                    model.Errors.Add(error);
                }

                return View["domain", model];

            };

            Get ["/domain/{name}/role/{role}"] = parameters => {
                RoleModel model = new RoleModel();
                model.Manager  = (Manager)this.Context.CurrentUser;
                model.Domain  = repository.GetDomain((string)parameters.name, model.Manager.Id);
                model.Users = repository.GetDomainUsers(model.Domain.Id).ToList();
                model.Role = repository.GetRole(model.Domain.Id,(string)parameters.role);
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

            
            Post ["/domain/{name}/role/{role}"] = parameters => {
                RoleModel model = this.Bind<RoleModel>();

                model.Manager = (Manager)this.Context.CurrentUser;
                model.Domain = repository.GetDomain((string)parameters.name, model.Manager.Id);

                model.Role = repository.GetRole(model.Domain.Id,(string)parameters.role);

                if(Request.Form.Delete != null)
                {
                    repository.DeleteRole(model.Role);
                    return this.Response.AsRedirect(string.Format("/domain/{0}",(string)parameters.name));
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

                model.Users = repository.GetDomainUsers(model.Domain.Id).ToList();

                return View["role", model];
            };

            Get ["/domain/{name}/role/new"] = parameters => {
                CreateRoleModel model = new CreateRoleModel ();
                model.Manager  = (Manager)this.Context.CurrentUser;
                model.Domain = repository.GetDomain((string)parameters.name, model.Manager.Id);
           
                return View["role_new", model];
            };

            Post ["/domain/{name}/role/new"] = parameters => {
                CreateRoleModel model = this.Bind<CreateRoleModel>();
                model.Manager  = (Manager)this.Context.CurrentUser;
                model.Domain = repository.GetDomain((string)parameters.name, model.Manager.Id);

                var result = this.Validate(model);


                if (!result.IsValid)
                {
                    model.Errors = Helpers.GetValidationErrors(result);
                    return View["role_new", model];
                }

                try
                {
                    repository.CreateRole(model.Domain.Id, model.Name);
                }
                catch(WriteConcernException e)
                {
                    model.Errors = new List<Error>();
                    Error error = new Error();
                    error.Name = "Duplicate";
                    error.Message = "Role already exists.";
                    model.Errors.Add(error);
                    return View["role_new", model];
                }

                return this.Response.AsRedirect("/domain/" + model.Domain.Name);
            };

            Get ["/domain/{name}/claim/{cname}"] = parameters => {
                ClaimModel model = new ClaimModel ();
                model.Name = (string)parameters.cname;
                model.Title = "Claim";
                model.Manager = (Manager)this.Context.CurrentUser;
                model.Domain = repository.GetDomain((string)parameters.name, model.Manager.Id);
                model.Users = repository.GetUsersWithClaim(model.Domain.Id,(string)parameters.cname);
                model.Roles = repository.GetRolesWithClaim(model.Domain.Id,(string)parameters.cname);

                return View["claim", model];
            };

            Post ["/domain/{name}/claim/{cname}"] = parameters => {
                ClaimModel model = new ClaimModel ();
                model.Name = (string)parameters.cname;
                model.Manager = (Manager)this.Context.CurrentUser;
                model.Domain = repository.GetDomain((string)parameters.name, model.Manager.Id);

                if (Request.Form.Delete != null) {
                    model.Domain.Claims.Remove(model.Name);
                    model.Domain = repository.UpdateDomain(model.Domain);
                    Role[] roles = repository.GetRolesWithClaim(model.Domain.Id, model.Name);

                    if(roles != null)
                    {
                        foreach(Role r in roles){
                            r.Claims.Remove(model.Name);
                            repository.UpdateRole(r);
                        }
                    }

                    User[] users = repository.GetUsersWithClaim(model.Domain.Id, model.Name);

                    if(users != null){
                        foreach(User u in users){
                            u.Claims.Remove(model.Name);
                            repository.UpdateUser(u);
                        }
                    }

                    return this.Response.AsRedirect (string.Format ("/domain/{0}", model.Domain.Name));
                }

                return View["claim", model];
            };

            Get ["/domain/{name}/claim/new"] = parameters => {
                ClaimModel model = new ClaimModel ();
                model.Manager = (Manager)this.Context.CurrentUser;
                model.Domain = repository.GetDomain((string)parameters.name, model.Manager.Id);
             
                return View["claim_new", model];
            };

            Post ["/domain/{name}/claim/new"] = parameters => {
                ClaimModel model = this.Bind<ClaimModel>();
                model.Manager = (Manager)this.Context.CurrentUser;
                model.Domain = repository.GetDomain((string)parameters.name, model.Manager.Id);

                var result = this.Validate(model);
                
                if (!result.IsValid)
                {
                    model.Errors = Helpers.GetValidationErrors(result);
                    return View["claim_new", model];
                }

                Domain update = repository.GetDomain(model.Domain.Name, model.Manager.Id);
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
                    repository.UpdateDomain(update);
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

                return this.Response.AsRedirect("/domain/" + model.Domain.Name);
            };

            Get ["/domain/{name}/user/{id}"] = parameters => {
                DomainUserModel model = new DomainUserModel();

                model.Manager = (Manager)this.Context.CurrentUser;
                model.Domain = repository.GetDomain((string)parameters.name, model.Manager.Id);
                model.User = repository.GetUser(Guid.Parse((string)parameters.id));
                model.Roles = repository.GetRoles(model.Domain.Id).ToList();

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

            Post ["/domain/{name}/user/{id}"] = parameters => {
                DomainUserModel model = this.Bind<DomainUserModel>();
                model.Manager = (Manager)this.Context.CurrentUser;
                model.Domain = repository.GetDomain((string)parameters.name, model.Manager.Id);
                model.User = repository.GetUser(Guid.Parse((string)parameters.id));
                model.Roles = repository.GetRoles(model.Domain.Id).ToList();

                List<Role> uroles = new List<Role>();
            
                if(model.NewRoles != null)
                {
                    foreach(string r in model.NewRoles)
                    {
                        uroles.Add(repository.GetRole(model.Domain.Id, r));
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

