using System;
using System.Web.Security;

namespace SuperSimple.Auth.Api
{
	public interface IRepository
	{
        User Authenticate (Guid appKey, string username, string secret, string IP = null);
		User CreateUser (Guid appKey, User user);
		User UpdateUser (Guid appKey, User user);
        User Validate (Guid authToken, Guid appKey, string IP = null);
        bool End (Guid appKey, Guid authToken);
        bool ValidateAppKey (string appName, Guid appKey);
	}
}

