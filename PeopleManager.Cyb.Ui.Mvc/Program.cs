using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using PeopleManager.Cyb.Ui.Mvc.Settings;
using PeopleManager.Cyb.Ui.Mvc.Stores;
using PeopleManager.Sdk;
using Vives.Security.Model.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var apiSettings = new ApiSettings();
builder.Configuration.GetSection(nameof(ApiSettings)).Bind(apiSettings);

builder.Services.AddHttpClient("PeopleManagerApi", options =>
{
    if (!string.IsNullOrWhiteSpace(apiSettings.BaseAddress))
    {
        options.BaseAddress = new Uri(apiSettings.BaseAddress);
    }
});

//Register SDK
builder.Services.AddScoped<AssignmentSdk>();
builder.Services.AddScoped<IdentitySdk>();
builder.Services.AddScoped<PersonSdk>();

builder.Services.AddScoped<ITokenStore, TokenStore>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Account/SignIn";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
