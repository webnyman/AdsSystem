using Ads.Web.Data;
using Ads.Web.Models;
using Ads.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ads.Web.Services
{
    public class AnnonsorRepository : IAnnonsorRepository
    {
        private readonly AdsContext _db;

        public AnnonsorRepository(AdsContext db)
        {
            _db = db;
        }

        public async Task<List<Annonsor>> GetAllWithAdsAsync()
        {
            return await _db.Annonsorer
                .Include(o => o.Annonser)
                .OrderByDescending(o => o.Skapad)
                .ToListAsync();
        }

        public async Task<Annonsor?> GetSubscriberAsync(string subscriptionNumber)
        {
            return await _db.Annonsorer
                .FirstOrDefaultAsync(o => o.Typ == 'P' && o.Prenumerationsnummer == subscriptionNumber);
        }

        public async Task<Annonsor?> GetCompanyAsync(string? orgNr, string namn)
        {
            return await _db.Annonsorer.FirstOrDefaultAsync(o =>
                o.Typ == 'F' &&
                ((!string.IsNullOrWhiteSpace(orgNr) && o.Organisationsnummer == orgNr)
                 || o.Namn == namn));
        }

        public async Task AddAsync(Annonsor annonsor)
        {
            _db.Annonsorer.Add(annonsor);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Annonsor annonsor)
        {
            _db.Annonsorer.Update(annonsor);
            await _db.SaveChangesAsync();
        }
    }
}
