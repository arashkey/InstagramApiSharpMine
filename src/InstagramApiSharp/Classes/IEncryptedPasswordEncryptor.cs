using InstagramApiSharp.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InstagramApiSharp.Classes
{
    public interface IEncryptedPasswordEncryptor
    {
        Task<string> GetEncryptedPassword(IInstaApi api, string password, long? providedTime = null);
    }
}
