using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForSight.TimeZonesService.Handlers.Services
{
    public class EncryptionService : IEncryptionService
    {
        public Task<string> DecryptString(string stringToDecrypt, string key)
        {
            throw new NotImplementedException();
        }

        public Task<string> EncryptString(string stringToEncrypt, string key)
        {
            throw new NotImplementedException();
        }
    }
}
