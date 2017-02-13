namespace SuperSimple.Auth.Manager
{
    using Nancy.ModelBinding;
    using Nancy.Validation;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Nancy;
    using Nancy.Security;
    using MongoDB.Driver;
    using SuperSimple.Auth.Manager.Repository;
    using SuperSimple.Auth.Api;
    using SuperSimple.Auth.Api.Repository;

    public class HomeModule : NancyModule
    {
        public HomeModule (IRepository repository)
        {
            this.RequiresAuthentication ();

            Get ["/home"] = parameters =>
            {
                var model = new ManageModel ();
                model.Manager = (Manager)this.Context.CurrentUser;
                try
                {
                    model.Domains = repository.GetDomains (model.Manager.Id);
                    foreach (var domain in model.Domains)
                    {
                        domain.UserCount =
                            repository.GetDomainUsers (domain.Id).Count ();
                    }
                    model.AdminDomains = repository.GetDomainsAdmin (model.Manager.Id);
                    foreach (var domain in model.AdminDomains)
                    {
                        domain.UserCount =
                            repository.GetDomainUsers (domain.Id).Count ();
                    }
                }
                catch (Exception exc)
                {
                    throw exc;
                }

                return View ["Manage", model];
            };

            Get ["/domain/new"] = parameters =>
            {
                DomainModel model = new DomainModel ();
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = new Domain ();
                return View ["Domain_new", model];
            };

            Post ["/domain/new"] = parameters =>
            {
                DomainModel model = this.Bind<DomainModel> ();
                model.Manager = (IUser)this.Context.CurrentUser;
                var result = this.Validate (model);

                if (!result.IsValid)
                {
                    model.Errors = Helpers.GetValidationErrors (result);
                    return View ["Domain_new", model];
                }

                try
                {
                    repository.CreateDomain (model.Name, model.Manager);
                }
                catch (MongoDB.Driver.WriteConcernException e)
                {
                    Error error = new Error ();
                    error.Name = "Duplicate";
                    error.Message = "Cannot create domain, name already exists.";
                    model.Errors = new List<Error> ();
                    model.Errors.Add (error);

                    return View ["Domain_new", model];
                }

                return this.Response.AsRedirect ("/home");
            };

            Get ["/domain/{id}"] = parameters =>
            {
                DomainModel model = new DomainModel ();
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);

                if (model.Domain == null || !repository.HasAccess (model.Domain,
                                                                   model.Manager))
                {
                    return View ["NoAccess"];
                }

                model.Roles = repository.GetRoles (model.Domain.Id).ToList ();
                model.Users = repository.GetDomainUsers (model.Domain.Id).ToList ();
                model.Admins = repository.GetAdministrators (model.Domain.Id).ToList ();


                return View ["Domain", model];
            };

            Post ["/domain/{id}"] = parameters =>
            {
                DomainModel model = this.Bind<DomainModel> ();
                model.Manager = (IUser)this.Context.CurrentUser;
                Domain domain = repository.GetDomain ((Guid)parameters.id);

                if (domain == null || !repository.HasAccess (domain, model.Manager))
                {
                    return View ["NoAccess"];
                }

                model.Roles = repository.GetRoles (domain.Id).ToList ();
                model.Users = repository.GetDomainUsers (domain.Id).ToList ();

                domain.WhiteListIps = model.WhiteListIps;

                if (Request.Form.Generate != null)
                {
                    domain.Key = Guid.NewGuid ();
                }

                if (Request.Form.Delete != null)
                {
                    repository.DeleteDomain (domain.Id);
                    return this.Response.AsRedirect ("/");
                }

                if (Request.Form.Disable != null)
                {
                    domain.Enabled = false;
                }

                if (Request.Form.Enable != null)
                {
                    domain.Enabled = true;
                }

                try
                {
                    model.Domain = repository.UpdateDomain (domain);
                    model.Messages.Add ("Domain successfully updated");
                }
                catch (Exception e)
                {
                    Error error = new Error ();
                    error.Name = e.GetType ().ToString ();
                    error.Message = e.Message;

                    model.Errors.Add (error);
                }

                return View ["Domain", model];

            };

            Get ["/domain/{id}/admin/{aid}"] = parameters =>
            {
                AdminModel model = new AdminModel ();
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);
                model.Admin = repository.GetManager ((Guid)parameters.aid);

                if (!model.Domain.IsOwner (model.Manager))
                {
                    return View ["NoAccess"];
                }

                return View ["Admin", model];
            };

            Post ["/remove/{id}/admin/{aid}"] = parameters =>
            {
                AdminModel model = new AdminModel ();
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);

                if (!model.Domain.IsOwner (model.Manager))
                {
                    return View ["NoAccess"];
                }

                Guid domainId = (Guid)parameters.id;
                Guid adminId = (Guid)parameters.aid;
                repository.DeleteAdministrator (domainId, adminId);

                return this.Response
                    .AsRedirect (string.Format ("/domain/{0}",
                        (string)parameters.id));

            };

            Get ["/domain/{id}/admin/new"] = parameters =>
            {
                AdminModel model = new AdminModel ();
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);

                if (model.Domain == null || !repository.HasAccess (model.Domain,
                                                                   model.Manager))
                {
                    return View ["NoAccess"];
                }

                if (!model.Domain.IsOwner (model.Manager))
                {
                    Error error = new Error ();
                    error.Name = "Not Authorized";
                    error.Message = "You are not the owner of this domain.";
                    model.Errors.Add (error);
                }

                return View ["Admin_new", model];
            };

            Post ["/domain/{id}/admin/new"] = parameters =>
            {
                AdminModel model = this.Bind<AdminModel> ();
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);

                if (model.Domain == null || !repository.HasAccess (model.Domain,
                                                                   model.Manager))
                {
                    return View ["NoAccess"];
                }

                if (model.Domain.IsOwner (model.Manager))
                {
                    IUser admin = null;

                    if (model.Manager.UserName.ToLower () !=
                        model.Email.ToLower ())
                    {
                        try
                        {
                            admin = repository.AddAdministrator (model.Domain.Id,
                                            model.Email);
                        }
                        catch (WriteConcernException e)
                        {
                            model.Errors = new List<Error> ();
                            Error error = new Error ();
                            error.Name = "Duplicate";
                            error.Message = "Admin already exists.";
                            model.Errors.Add (error);

                            return View ["Admin_new", model];
                        }
                    }
                    else
                    {
                        model.Errors = new List<Error> ();
                        Error error = new Error ();
                        error.Name = "Duplicate";
                        error.Message = "That is your email.";
                        model.Errors.Add (error);

                        return View ["Admin_new", model];
                    }

                    if (admin != null)
                    {
                        return this.Response
                                .AsRedirect (string.Format ("/domain/{0}",
                                (string)parameters.id));
                    }
                    else
                    {
                        Error error = new Error ();
                        error.Name = "Manager error.";
                        error.Message = "Manager's email could not be found.";
                        model.Errors.Add (error);
                    }
                }
                else
                {
                    Error error = new Error ();
                    error.Name = "Not Authorized";
                    error.Message = "You are not the owner of this domain.";
                    model.Errors.Add (error);
                }

                return View ["Admin_new", model];
            };

            Get ["/domain/{id}/role/{role}"] = parameters =>
            {
                var model = new RoleModel ();
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);

                if (model.Domain == null || !repository.HasAccess (model.Domain,
                                                                   model.Manager))
                {
                    return View ["NoAccess"];
                }

                model.Users = repository.GetDomainUsers (model.Domain.Id).ToList ();
                model.Role = repository.GetRole (model.Domain.Id, (string)parameters.role);
                User [] us = repository.GetUsersInRole (model.Role);

                List<string> usStringified = new List<string> ();

                foreach (User u in us)
                {
                    usStringified.Add (u.Id.ToString ());
                }

                model.Claims = model.Role.Claims;
                model.RoleUsers = usStringified;
                return View ["role", model];
            };

            Post ["/domain/{id}/role/{role}"] = parameters =>
            {
                RoleModel model = this.Bind<RoleModel> ();
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);

                if (model.Domain == null || !repository.HasAccess (model.Domain
                                                                   ,model.Manager))
                {
                    return View ["NoAccess"];
                }

                model.Role = repository.GetRole (model.Domain.Id, (string)parameters.role);

                if (Request.Form.Delete != null)
                {
                    repository.DeleteRole (model.Role);
                    return this.Response.AsRedirect (string.Format ("/domain/{0}", (string)parameters.id));
                }


                model.Role.Claims = model.Claims == null ? new List<string> () : model.Claims.ToList ();
                model.Role = repository.UpdateRole (model.Role);


                User user = null;

                if (model.RoleUsers != null)
                {
                    foreach (string u in model.RoleUsers)
                    {
                        user = repository.GetUser (Guid.Parse (u));

                        if (!user.InRole (model.Role.Name))
                        {
                            if (user.Roles == null)
                            {
                                user.Roles = new List<Role> ();
                            }

                            user.AddRole (model.Role);
                            repository.UpdateUser (user);
                        }
                    }
                }

                if (model.uRoleUsers != null)
                {
                    foreach (string u in model.uRoleUsers)
                    {
                        user = repository.GetUser (Guid.Parse (u));

                        if (user.InRole (model.Role.Name))
                        {
                            if (user.Roles == null)
                            {
                                user.Roles = new List<Role> ();
                            }

                            user.RemoveRole (model.Role);
                            repository.UpdateUser (user);
                        }
                    }
                }

                model.Users = repository.GetDomainUsers (model.Domain.Id).ToList ();

                return View ["Role", model];
            };

            Get ["/domain/{id}/role/new"] = parameters =>
            {
                CreateRoleModel model = new CreateRoleModel ();
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);

                if (model.Domain == null || !repository.HasAccess (model.Domain,
                                                                   model.Manager))
                {
                    return View ["NoAccess"];
                }

                return View ["Role_new", model];
            };

            Post ["/domain/{id}/role/new"] = parameters =>
            {
                CreateRoleModel model = this.Bind<CreateRoleModel> ();
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);

                if (model.Domain == null || !repository.HasAccess (model.Domain,
                                                                   model.Manager))
                {
                    return View ["NoAccess"];
                }

                var result = this.Validate (model);


                if (!result.IsValid)
                {
                    model.Errors = Helpers.GetValidationErrors (result);
                    return View ["Role_new", model];
                }

                try
                {
                    repository.CreateRole (model.Domain.Id, model.Name);
                }
                catch (WriteConcernException e)
                {
                    model.Errors = new List<Error> ();
                    Error error = new Error ();
                    error.Name = "Duplicate";
                    error.Message = "Role already exists.";
                    model.Errors.Add (error);

                    return View ["Role_new", model];
                }

                return this.Response.AsRedirect ("/domain/" + model.Domain.Id);
            };

            Get ["/domain/{id}/claim/{cname}"] = parameters =>
            {
                ClaimModel model = new ClaimModel ();
                model.Name = (string)parameters.cname;
                model.Title = "Claim";
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);

                if (model.Domain == null || !repository.HasAccess (model.Domain,
                                                                   model.Manager))
                {
                    return View ["NoAccess"];
                }

                model.Users = repository.GetUsersWithClaim (model.Domain.Id, (string)parameters.cname);
                model.Roles = repository.GetRolesWithClaim (model.Domain.Id, (string)parameters.cname);

                return View ["Claim", model];
            };

            Post ["/domain/{id}/claim/{cname}"] = parameters =>
            {
                ClaimModel model = new ClaimModel ();
                model.Name = (string)parameters.cname;
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);

                if (model.Domain == null || !repository.HasAccess (model.Domain,
                                                                   model.Manager))
                {
                    return View ["NoAccess"];
                }

                if (Request.Form.Delete != null)
                {
                    model.Domain.Claims.Remove (model.Name);
                    model.Domain = repository.UpdateDomain (model.Domain);
                    Role [] roles = repository.GetRolesWithClaim (model.Domain.Id, model.Name);

                    if (roles != null)
                    {
                        foreach (Role r in roles)
                        {
                            r.Claims.Remove (model.Name);
                            repository.UpdateRole (r);
                        }
                    }

                    User [] users = repository.GetUsersWithClaim (model.Domain.Id, model.Name);

                    if (users != null)
                    {
                        foreach (User u in users)
                        {
                            u.Claims.Remove (model.Name);
                            repository.UpdateUser (u);
                        }
                    }

                    return this.Response.AsRedirect (string.Format ("/domain/{0}", model.Domain.Name));
                }

                return View ["Claim", model];
            };

            Get ["/domain/{id}/claim/new"] = parameters =>
            {
                ClaimModel model = new ClaimModel ();
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);

                if (model.Domain == null || !repository.HasAccess (model.Domain,
                                                                   model.Manager))
                {
                    return View ["NoAccess"];
                }

                return View ["Claim_new", model];
            };

            Post ["/domain/{id}/claim/new"] = parameters =>
            {
                ClaimModel model = this.Bind<ClaimModel> ();
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);

                if (model.Domain == null || !repository.HasAccess (model.Domain,
                                                                   model.Manager))
                {
                    return View ["NoAccess"];
                }

                var result = this.Validate (model);

                if (!result.IsValid)
                {
                    model.Errors = Helpers.GetValidationErrors (result);
                    return View ["Claim_new", model];
                }

                Domain update = repository.GetDomain ((Guid)parameters.id);
                List<string> claims = new List<string> ();

                if (update.Claims != null)
                {
                    claims = new List<string> (update.Claims);
                }


                if (claims.Contains (model.Name))
                {
                    Error error = new Error ();
                    error.Name = "Duplicate";
                    error.Message = "Claim already exists.";
                    model.Errors.Add (error);

                    return View ["Claim_new", model];
                }

                claims.Add (model.Name);
                update.Claims = claims.ToList ();

                try
                {
                    repository.UpdateDomain (update);
                }
                catch (Exception e)
                {
                    Error error = new Error ();
                    error.Name = e.ToString ();
                    error.Message = e.Message;
                    model.Errors.Add (error);
                    return View ["Claim_new", model];
                }

                return this.Response.AsRedirect ("/domain/" + model.Domain.Id);
            };

            Get ["/domain/{id}/user/{uid}"] = parameters =>
            {
                DomainUserModel model = new DomainUserModel ();

                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);

                if (model.Domain == null || !repository.HasAccess (model.Domain,
                                                                   model.Manager))
                {
                    return View ["NoAccess"];
                }

                model.User = repository.GetUser (Guid.Parse ((string)parameters.uid));
                model.Roles = repository.GetRoles (model.Domain.Id).ToList ();
                model.Enabled = model.User.Enabled;

                List<string> uRoles = new List<string> ();

                if (model.User.Roles != null)
                {
                    foreach (Role r in model.User.Roles)
                    {
                        uRoles.Add (r.Name);
                    }
                }

                model.NewRoles = uRoles;
                model.Claims = model.User.Claims;

                return View ["User", model];
            };

            Post ["/domain/{id}/user/{uid}"] = parameters =>
            {
                DomainUserModel model = this.Bind<DomainUserModel> ();
                model.Manager = (IUser)this.Context.CurrentUser;
                model.Domain = repository.GetDomain ((Guid)parameters.id);

                if (model.Domain == null || !repository.HasAccess (model.Domain,
                                                                   model.Manager))
                {
                    return View ["NoAccess"];
                }

                model.User = repository.GetUser (Guid.Parse ((string)parameters.uid));
                model.Roles = repository.GetRoles (model.Domain.Id).ToList ();

                if (Request.Form.Delete != null)
                {
                    repository.DeleteUser (model.Domain.Id, model.User.UserName);
                    DomainModel dmodel = new DomainModel ();
                    dmodel.Domain = model.Domain;
                    dmodel.Manager = model.Manager;
                    dmodel.Roles = repository.GetRoles (dmodel.Domain.Id).ToList ();
                    dmodel.Users = repository.GetDomainUsers (dmodel.Domain.Id).ToList ();
                    dmodel.Messages.Add (string.Format ("User: {0} has been successfully deleted.", model.User.UserName));

                    return View ["Domain", dmodel];
                }

                if (Request.Form.Save != null)
                {
                    List<Role> uroles = new List<Role> ();

                    if (model.NewRoles != null)
                    {
                        foreach (string r in model.NewRoles)
                        {
                            uroles.Add (repository.GetRole (model.Domain.Id, r));
                        }
                    }

                    model.User.Roles = uroles;
                    model.User.Claims = model.Claims;
                    model.User.Enabled = model.Enabled;

                    try
                    {
                        model.User = repository.UpdateUser (model.User);
                        model.Messages.Add ("Successfully updated.");
                    }
                    catch (Exception e)
                    {
                        Error error = new Error ();
                        error.Name = e.GetType ().ToString ();
                        error.Message = e.Message;
                        model.Errors.Add (error);
                    }
                }

                return View ["User", model];
            };
        }
    }
}

