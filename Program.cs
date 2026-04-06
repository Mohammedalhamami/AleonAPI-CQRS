

using AleonAPI.Endpoints.Home;
using Microsoft.AspNetCore.Identity;
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

builder.Services.AddValidation();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapHomeEndpoints();
app.UseMiddleware<BlockIdentityEndpoints>();

var authRouteGroup = app.MapGroup("/api/auth")
    .WithTags("Admin");
authRouteGroup.MapIdentityApi<User>();

app.Run();
