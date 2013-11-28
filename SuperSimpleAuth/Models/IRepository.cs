using System;
using System.Web.Security;

namespace SuperSimple.Auth.Api
{
	public interface IRepository
	{
        User Authenticate (Guid domainKey, string username, string secret, string IP = null);
        User CreateUser (Guid domainKey, string username, string password, string email = null);
        User UpdateUser (Guid domainKey, User user);
        User Validate (Guid authToken, Guid domainKey, string IP = null);
        bool UsernameExists (Guid domainKey, string username);
        bool EmailExists (Guid domainKey, string email);
        bool End (Guid domainKey, Guid authToken);
        bool ValidateDomainKey (string domainName, Guid domainKey);
	}
}

