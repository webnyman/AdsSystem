using Ads.Web.Data;
using Ads.Web.Models;
using Ads.Web.Services;
using Ads.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ads.Web.Controllers
{
    public class AdsController : Controller
    {
        private readonly AdsContext _db;
        private readonly SubscriberApiClient _subs;

        public AdsController(AdsContext db, SubscriberApiClient subs)
        {
            _db = db;
            _subs = subs;
        }

        // Lista alla annonser
        public async Task<IActionResult> Index()
        {
            var list = await _db.Annonsorer
                .Include(o => o.Annonser)
                .OrderByDescending(o => o.Skapad)
                .ToListAsync();

            return View(list);
        }

        // GET: Skapa
        [HttpGet]
        public IActionResult Create() => View(new CreateAdViewModel());

        // AJAX: slå upp prenumerantinfo
        [HttpGet]
        public async Task<IActionResult> LookupSubscriber(string prenr)
        {
            if (string.IsNullOrWhiteSpace(prenr))
                return BadRequest(new { message = "Ange prenumerationsnummer." });

            var dto = await _subs.GetAdInfoAsync(prenr);
            if (dto is null || !dto.AllowedToAdvertise)
                return NotFound(new { message = "Prenumerant hittades ej eller får ej annonsera." });

            return Json(new
            {
                namn = dto.FullName,
                telefon = dto.PhoneNumber,
                adress = dto.DeliveryAddress,
                postnr = dto.PostalCode,
                ort = dto.City
            });
        }

        // POST: Skapa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAdViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // 1) Säkra/eller hämta annonsör
            Annonsor? annonsor = null;

            if (vm.IsSubscriber)
            {
                // Försök hitta existerande annonsör på prenr
                if (string.IsNullOrWhiteSpace(vm.SubscriptionNumber))
                {
                    ModelState.AddModelError(nameof(vm.SubscriptionNumber), "Prenumerationsnummer krävs.");
                    return View(vm);
                }

                annonsor = await _db.Annonsorer
                    .FirstOrDefaultAsync(o => o.Typ == 'P' && o.Prenumerationsnummer == vm.SubscriptionNumber);

                if (annonsor is null)
                {
                    annonsor = new Annonsor
                    {
                        Typ = 'P',
                        Prenumerationsnummer = vm.SubscriptionNumber,
                        Namn = vm.Namn,
                        Telefon = vm.Telefon,
                        Utdelningsadress = vm.Utdelningsadress,
                        Postnummer = vm.Postnummer,
                        Ort = vm.Ort
                    };
                    _db.Annonsorer.Add(annonsor);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    // Uppdatera snapshot-data om användaren justerat fält
                    annonsor.Namn = vm.Namn;
                    annonsor.Telefon = vm.Telefon;
                    annonsor.Utdelningsadress = vm.Utdelningsadress;
                    annonsor.Postnummer = vm.Postnummer;
                    annonsor.Ort = vm.Ort;
                    _db.Annonsorer.Update(annonsor);
                    await _db.SaveChangesAsync();
                }

                // 🔹 NYTT: Försök även uppdatera prenumerantens kontaktuppgifter i prenumerantsystemet
                try
                {
                    var updateDto = new SubscriberContactUpdateDto
                    {
                        PhoneNumber = vm.Telefon,
                        DeliveryAddress = vm.Utdelningsadress,
                        PostalCode = vm.Postnummer,
                        City = vm.Ort
                    };

                    await _subs.UpdateContactAsync(vm.SubscriptionNumber!, updateDto);
                    // Ev. logging om du vill, men ingen hård fail här.
                }
                catch
                {
                    // Här kan du logga om du vill, men låt inte annons-skapandet krascha.
                }
            }
            else
            {
                // Företag: matcha på orgnr eller namn
                annonsor = await _db.Annonsorer.FirstOrDefaultAsync(o =>
                    o.Typ == 'F' &&
                    (!string.IsNullOrWhiteSpace(vm.Organisationsnummer) && o.Organisationsnummer == vm.Organisationsnummer
                     || o.Namn == vm.Namn));

                if (annonsor is null)
                {
                    annonsor = new Annonsor
                    {
                        Typ = 'F',
                        Namn = vm.Namn,
                        Organisationsnummer = vm.Organisationsnummer,
                        Telefon = vm.Telefon,
                        Utdelningsadress = vm.Utdelningsadress,
                        Postnummer = vm.Postnummer,
                        Ort = vm.Ort,
                        FakturaAdress = vm.FakturaAdress,
                        FakturaPostnummer = vm.FakturaPostnummer,
                        FakturaOrt = vm.FakturaOrt
                    };
                    _db.Annonsorer.Add(annonsor);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    annonsor.Namn = vm.Namn;
                    annonsor.Organisationsnummer = vm.Organisationsnummer;
                    annonsor.Telefon = vm.Telefon;
                    annonsor.Utdelningsadress = vm.Utdelningsadress;
                    annonsor.Postnummer = vm.Postnummer;
                    annonsor.Ort = vm.Ort;
                    annonsor.FakturaAdress = vm.FakturaAdress;
                    annonsor.FakturaPostnummer = vm.FakturaPostnummer;
                    annonsor.FakturaOrt = vm.FakturaOrt;
                    _db.Annonsorer.Update(annonsor);
                    await _db.SaveChangesAsync();
                }
            }

            // 2) Skapa annons (prisregeln)
            var annons = new Annons
            {
                AnnonsorId = annonsor.Id,
                Rubrik = vm.Rubrik,
                Innehall = vm.Innehall,
                VaransPris = vm.VaransPris,
                Annonspris = vm.IsSubscriber ? 0m : 40m
            };

            _db.Annonser.Add(annons);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
