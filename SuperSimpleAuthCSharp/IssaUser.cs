using System;
using System.Collections.Generic;

namespace SuperSimple.Auth
{

    public interface ISsaUser
    {
        Guid Id { get; set; }
        string UserName { get; set;}
        Guid AuthToken { get; set; }
        IEnumerable<string> Claims { get; set; }
        IEnumerable<string> Roles { get; set; }
    }
}

