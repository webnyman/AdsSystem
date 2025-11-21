using Ads.Web.Data;
using Ads.Web.Services;
using Ads.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AdsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AdsDb")));

// Typed HttpClient för prenumerant-API:t
builder.Services.AddHttpClient<SubscriberApiClient>(client =>
{
    var baseUrl = builder.Configuration["SubscriberApi:BaseUrl"]!;
    client.BaseAddress = new Uri(baseUrl);
});

// DAL + logiklager
builder.Services.AddScoped<IAnnonsorRepository, AnnonsorRepository>();
builder.Services.AddScoped<IAnnonsRepository, AnnonsRepository>();
builder.Services.AddScoped<IAdsService, AdsService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Ads}/{action=Index}/{id?}");

app.Run();
