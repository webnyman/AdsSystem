using Ads.Web.Models;
using Ads.Web.Services.Interfaces;
using Ads.Web.ViewModels;

namespace Ads.Web.Services
{
    public class AdsService : IAdsService
    {
        private readonly IAnnonsorRepository _annonsorRepo;
        private readonly IAnnonsRepository _annonsRepo;
        private readonly SubscriberApiClient _subsClient;

        public AdsService(
            IAnnonsorRepository annonsorRepo,
            IAnnonsRepository annonsRepo,
            SubscriberApiClient subsClient)
        {
            _annonsorRepo = annonsorRepo;
            _annonsRepo = annonsRepo;
            _subsClient = subsClient;
        }

        public Task<List<Annonsor>> GetAllAnnonsorerWithAdsAsync()
        {
            return _annonsorRepo.GetAllWithAdsAsync();
        }

        public async Task<Annons> CreateAdAsync(CreateAdViewModel vm)
        {
            Annonsor? annonsor;

            if (vm.IsSubscriber)
            {
                if (string.IsNullOrWhiteSpace(vm.SubscriptionNumber))
                    throw new ArgumentException("Prenumerationsnummer krävs för prenumerant-annons.");

                annonsor = await _annonsorRepo.GetSubscriberAsync(vm.SubscriptionNumber);

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
                    await _annonsorRepo.AddAsync(annonsor);
                }
                else
                {
                    annonsor.Namn = vm.Namn;
                    annonsor.Telefon = vm.Telefon;
                    annonsor.Utdelningsadress = vm.Utdelningsadress;
                    annonsor.Postnummer = vm.Postnummer;
                    annonsor.Ort = vm.Ort;
                    await _annonsorRepo.UpdateAsync(annonsor);
                }

                // Synka tillbaka till prenumerantsystemet (kontaktuppgifter)
                try
                {
                    var updateDto = new SubscriberContactUpdateDto
                    {
                        PhoneNumber = vm.Telefon,
                        DeliveryAddress = vm.Utdelningsadress,
                        PostalCode = vm.Postnummer,
                        City = vm.Ort
                    };

                    await _subsClient.UpdateContactAsync(vm.SubscriptionNumber!, updateDto);
                }
                catch
                {
                    // Logga ev., men kasta inte om annonsen ska sparas ändå
                }
            }
            else
            {
                annonsor = await _annonsorRepo.GetCompanyAsync(vm.Organisationsnummer, vm.Namn);

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
                    await _annonsorRepo.AddAsync(annonsor);
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
                    await _annonsorRepo.UpdateAsync(annonsor);
                }
            }

            var annons = new Annons
            {
                AnnonsorId = annonsor.Id,
                Rubrik = vm.Rubrik,
                Innehall = vm.Innehall,
                VaransPris = vm.VaransPris,
                Annonspris = vm.IsSubscriber ? 0m : 40m
            };

            await _annonsRepo.AddAsync(annons);

            return annons;
        }
    }
}
