namespace SuperSimple.Auth.Api
{
    using System;
    using System.Collections.Generic;
    using Repository;

    public interface IUser
    {
        Guid Id                 { get; set; }
        Guid DomainId           { get; set; }
        string UserName         { get; set; }
        string Email            { get; set; }
        string Secret           { get; set; }
        string Key              { get; set; }
        Guid AuthToken          { get; set; }
        bool Enabled            { get; set; }
        List<Role> Roles        { get; set; }
        List<string> Claims     { get; set; }
        string CurrentIp        { get; set; }
        DateTime ?CurrentLogon  { get; set; }
        string LastIp           { get; set; }
        DateTime ?LastLogon     { get; set; }
        DateTime ?LastRequest   { get; set; }
        int LogonCount          { get; set; }
        DateTime CreatedAt      { get; set; }
        DateTime ModifiedAt     { get; set; }

        string[] GetRoles();
        void AddRole(Role role);
        void RemoveRole(Role role);
        string[] GetClaims();
        bool InRole(string role);
        bool HasClaim(string claim);
    }
}