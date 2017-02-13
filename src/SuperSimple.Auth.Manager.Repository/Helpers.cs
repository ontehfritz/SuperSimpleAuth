using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SuperSimple.Auth.Manager.Repository
{
    public static class Encrypt
    {
        /// <summary>
        /// Values the specified array.
        /// </summary>
        /// <param name="array">Array.</param>

        /// <summary>
        public static string Hash(string Salt, string Password) 
        {
            Rfc2898DeriveBytes hash = new Rfc2898DeriveBytes(Password,
                System.Text.Encoding.Default.GetBytes(Salt), 10000);

            return Convert.ToBase64String(hash.GetBytes(25));
        }

    }
}

