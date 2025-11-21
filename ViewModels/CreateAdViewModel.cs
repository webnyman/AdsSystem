namespace Ads.Web.ViewModels
{
    public class CreateAdViewModel
    {
        // Väljare
        public bool IsSubscriber { get; set; }
        public bool IsCompany => !IsSubscriber;

        // Prenumerant
        public string? SubscriptionNumber { get; set; }

        // Gemensamma fält som fylls (antingen via API eller manuellt)
        public string Namn { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string Utdelningsadress { get; set; } = string.Empty;
        public string Postnummer { get; set; } = string.Empty;
        public string Ort { get; set; } = string.Empty;

        // Företagsspecifikt
        public string? Organisationsnummer { get; set; }
        public string? FakturaAdress { get; set; }
        public string? FakturaPostnummer { get; set; }
        public string? FakturaOrt { get; set; }

        // Annonsfält
        public string Rubrik { get; set; } = string.Empty;
        public string Innehall { get; set; } = string.Empty;
        public decimal VaransPris { get; set; }
    }
}
