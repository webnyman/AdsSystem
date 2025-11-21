using Ads.Web.Models;
using Ads.Web.Services;
using Ads.Web.Services.Interfaces;
using Ads.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ads.Web.Controllers
{
    public class AdsController : Controller
    {
        private readonly IAdsService _adsService;
        private readonly SubscriberApiClient _subs;

        public AdsController(IAdsService adsService, SubscriberApiClient subs)
        {
            _adsService = adsService;
            _subs = subs;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _adsService.GetAllAnnonsorerWithAdsAsync();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new CreateAdViewModel());

        // AJAX: slå upp prenumerantinfo (denna kan ligga kvar som tidigare)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAdViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _adsService.CreateAdAsync(vm);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
