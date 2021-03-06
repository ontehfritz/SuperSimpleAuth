namespace SuperSimple.Auth.Manager.Repository
{
    using System;
    using Api.Repository;
    using SuperSimple.Auth.Api;

    /// <summary>
    /// Interface for database access.
    /// </summary>
    public interface IRepository
    {
        User[] GetDomainUsers(Guid domainId);
        User[] GetUsersInRole(Role role);
        User[] GetUsersWithClaim(Guid domainId, string claim);
        User GetUser(Guid userId);
        User GetUser(Guid domainId, string username);
        User UpdateUser(User user);
        void DeleteUser(Guid domainId, string userName);
       
        Role[] GetRoles(Guid domainId);
        Role[] GetRolesWithClaim(Guid domainId, string claim);
        Role GetRole(Guid domainId, string name);
        Role CreateRole(Guid domainId, string name);
        Role UpdateRole(Role role);
        void DeleteRole(Role role);

        Domain SsaDomain { get; }
        Domain GetDomain(Guid id);
        Domain[] GetDomains(Guid managerID);
        Domain[] GetDomainsAdmin(Guid managerID);
        Domain CreateDomain(string name, IUser manager);
        Domain UpdateDomain(Domain domain);
        void DeleteDomain(Guid id);
        bool HasAccess(Domain domain, IUser manager);
        string GetOwnerName(Domain domain);

        IUser[] GetAdministrators(Guid domainId);
        IUser AddAdministrator(Guid domainId, string email);
        void DeleteAdministrator(Guid domainId, Guid adminId);

        IUser GetManager(Guid managerId);
        IUser GetManager(string userName);
        IUser CreateManager(string userName, string secret);
        void ChangeEmail(Guid id, string secret, string email);
        void ChangePassword(Guid id, string password, string newPassword, string confirmPassword);
        void DeleteManager(Guid id, string password);
        string ForgotPassword(string email);
    }
}
