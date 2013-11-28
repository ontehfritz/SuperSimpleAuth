using System;

namespace SSAManager
{
    /// <summary>
    /// Interface for database access.
    /// </summary>
    public interface IRepository
    {
        User[] GetDomainUsers(Guid domainId);
        User[] GetManagerUsers(Guid managerId);
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

        Domain GetDomain(string name, Guid managerId);
        Domain[] GetDomains(Guid managerID);
        Domain CreateDomain(string name, Manager manager);
        Domain UpdateDomain(Domain domain);
        void DeleteDomain(string name, Guid managerId);

        Manager GetManager(Guid managerId);
        Manager GetManager(string userName);
        Manager CreateManager(Manager manager);
        void ChangeEmail(Guid id, string secret, string email);
        void ChangePassword(Guid id, string password, string newPassword, string confirmPassword);
        void DeleteManager(Guid id);
    }
}
