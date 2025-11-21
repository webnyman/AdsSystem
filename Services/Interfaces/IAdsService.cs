using Ads.Web.Models;
using Ads.Web.ViewModels;

namespace Ads.Web.Services.Interfaces
{
    public interface IAdsService
    {
        Task<List<Annonsor>> GetAllAnnonsorerWithAdsAsync();
        Task<Annons> CreateAdAsync(CreateAdViewModel vm);
    }
}
