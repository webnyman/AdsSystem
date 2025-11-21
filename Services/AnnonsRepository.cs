using Ads.Web.Data;
using Ads.Web.Models;
using Ads.Web.Services.Interfaces;

namespace Ads.Web.Services
{
    public class AnnonsRepository : IAnnonsRepository
    {
        private readonly AdsContext _db;

        public AnnonsRepository(AdsContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Annons annons)
        {
            _db.Annonser.Add(annons);
            await _db.SaveChangesAsync();
        }
    }
}
