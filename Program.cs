

using AleonAPI.Endpoints.Artifact;
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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddCustomSwagger();


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
builder.Services.AddScoped<IArtifactMediaFileService, ArtifactMediaFileService>();
builder.Services.AddScoped<IArtifactService, ArtifactService>();

builder.Services.AddValidation();

builder.Services.AddControllers();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    await AleonAPI.Data.DataSeed.ManageDataAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapHomeEndpoints();
app.MapCustomIdentityEndpoints();
app.UseMiddleware<BlockIdentityEndpoints>();

app.MapSiteEndpoints();
app.MapArtifactMediaFileEndpoints();
app.MapArtifactEndpoints();

var authRouteGroup = app.MapGroup("/api/auth")
    .WithTags("Admin");
authRouteGroup.MapIdentityApi<User>();

app.Run();
