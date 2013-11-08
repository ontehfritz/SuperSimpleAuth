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
        User GetUser(Guid userId);
        User UpdateUser(User user);
        void DeleteUser(Guid appId, string userName);

        Role[] GetRoles(Guid appId);
        Role GetRole(Guid appId, string name);
        Role CreateRole(Role role);
        Role UpdateRole(Role role);

        App GetApp(Guid managerId, string appName);
        App[] GetApps(Guid managerID);
        App CreateApp(App app);
        App UpdateApp(App app);
        void DeleteApp(Guid managerId, string appName);

        Manager GetManager(Guid managerId);
        Manager GetManager(string userName);
        Manager CreateManager(Manager manager);
        Manager UpdateManager(Manager manager);
    }
}
