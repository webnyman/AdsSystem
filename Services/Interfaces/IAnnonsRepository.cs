using Ads.Web.Models;

namespace Ads.Web.Services.Interfaces
{
    public interface IAnnonsRepository
    {
        Task AddAsync(Annons annons);
    }
}
