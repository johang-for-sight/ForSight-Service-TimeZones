namespace ForSight.TimeZonesService.Handlers.Services
{
    public interface IEncryptionService
    {
        Task<string> EncryptString(string stringToEncrypt, string key);
        Task<string> DecryptString(string stringToDecrypt, string key);
    }
}
