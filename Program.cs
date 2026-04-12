

using AleonAPI.Endpoints.CustomIdentityEndpoints;
using AleonAPI.Endpoints.Home;
using AleonAPI.Endpoints.Sites;
using AleonAPI.Services;
using AleonAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = DataUtility.GetConnectionString(builder.Configuration);

builder.Services.AddCustomSwagger();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

//add Identity endpoints,
builder.Services.AddIdentityApiEndpoints<User>(options =>
        options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//add identity authorizaiton 
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//Email Sender Service
builder.Services.AddTransient<IEmailSender, ConsoleEmailService>();

builder.Services.AddScoped<ISiteService, SiteService>();

builder.Services.AddValidation();

builder.Services.AddControllers();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    await DataSeed.ManageDataAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapHomeEndpoints();
app.MapCustomIdentityEndpoints();
app.UseMiddleware<BlockIdentityEndpoints>();

app.MapSiteEndpoints();

var authRouteGroup = app.MapGroup("/api/auth")
    .WithTags("Admin");
authRouteGroup.MapIdentityApi<User>();

app.Run();
