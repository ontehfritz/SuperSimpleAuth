using System;

namespace SuperSimple.Auth.Api
{
	public interface IApiRepository
	{
        User Authenticate (Guid domainKey, string username, string secret, string IP = null);
        User CreateUser (Guid domainKey, string username, string password, string email = null);
        bool Disable (Guid authToken, Guid domainKey, string IP = null);
        User Validate (Guid authToken, Guid domainKey, string IP = null);
        bool UsernameExists (Guid domainKey, string username);
        bool EmailExists (Guid domainKey, string email);
        bool End (Guid domainKey, Guid authToken);
        string Forgot (Guid domainKey, string email);
        bool ChangePassword (Guid domainKey, Guid authToken, string newPassword);
        bool ChangeUserName (Guid domainKey, Guid authToken, string newUserName);
        bool ChangeEmail (Guid domainKey, Guid authToken, string newEmail);
        bool ValidateDomainKey (string domainName, Guid domainKey);
        bool IpAllowed (Guid domainKey, string ip);
	}
}

