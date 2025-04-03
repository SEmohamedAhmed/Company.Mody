
namespace Company.Mody.PL.Helper.Bitly
{
    public interface IBitlyService
    {
        public Task<string> ShortenUrl(string longUrl);
    }
}
