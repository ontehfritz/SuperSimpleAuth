using System;

namespace SSAManager
{
    /// <summary>
    /// Interface for database access.
    /// </summary>
    public interface IRepository
    {
        User[] GetAppUsers(Guid appId);
        User[] GetManagerUsers(Guid managerId);
        User[] GetUsersInRole(Role role);
        User[] GetUsersWithClaim(Guid appId, string claim);
        User GetUser(Guid userId);
        User GetUser(Guid appId, string username);
        User UpdateUser(User user);
        void DeleteUser(Guid appId, string userName);
       
        Role[] GetRoles(Guid appId);
        Role[] GetRolesWithClaim(Guid appId, string claim);
        Role GetRole(Guid appId, string name);
        Role CreateRole(Guid appId, string name);
        Role UpdateRole(Role role);
        void DeleteRole(Role role);

        App GetApp(string name, Guid managerId);
        App[] GetApps(Guid managerID);
        App CreateApp(string name, Manager manager);
        App UpdateApp(App app);
        void DeleteApp(string name, Guid managerId);

        Manager GetManager(Guid managerId);
        Manager GetManager(string userName);
        Manager CreateManager(Manager manager);
        Manager UpdateManager(Manager manager);
        void DeleteManager(Guid id);
    }
}
