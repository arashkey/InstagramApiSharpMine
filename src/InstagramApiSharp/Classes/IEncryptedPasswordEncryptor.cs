using InstagramApiSharp.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InstagramApiSharp.Classes
{
    /// <summary>
    ///     This is a helper for UWP SDK lower than 16299
    /// </summary>
    public interface IEncryptedPasswordEncryptor
    {
        /// <summary>
        ///     Encrypted password for login
        /// </summary>
        Task<string> GetEncryptedPassword(IInstaApi api, string password, long? providedTime = null);

        /// <summary>
        ///     Encrypted password for changing password
        /// </summary>
        /// <param name="api"></param>
        Task<(string, string, string)> GetEncryptedPassword(IInstaApi api, 
            string oldPassword,
            string newPassword, 
            long? providedTime = null);
    }
}
