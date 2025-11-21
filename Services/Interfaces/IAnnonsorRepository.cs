using Ads.Web.Models;

namespace Ads.Web.Services.Interfaces
{
    public interface IAnnonsorRepository
    {
        Task<List<Annonsor>> GetAllWithAdsAsync();
        Task<Annonsor?> GetSubscriberAsync(string subscriptionNumber);
        Task<Annonsor?> GetCompanyAsync(string? orgNr, string namn);
        Task AddAsync(Annonsor annonsor);
        Task UpdateAsync(Annonsor annonsor);
    }
}
