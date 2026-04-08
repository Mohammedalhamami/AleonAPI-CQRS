using System.Text;
using System.Web;
using AleonAPI.Endpoints.CustomIdentityEndpoints.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;

namespace AleonAPI.Endpoints.CustomIdentityEndpoints;

public static class CustomIdentityEndpoint
{
   public static IEndpointRouteBuilder MapCustomIdentityEndpoints(this IEndpointRouteBuilder route)
   {
      var group = route.MapGroup("api/auth")
         .WithTags("Admin");

      group.MapPost("/register-admin", RegisterUser)
         .WithName("RegisterAdmin")
         .WithSummary("Register Admin User")
         .WithDescription("Register a new user must have admin role");

      group.MapPost("rest-password", ResetPassword)
         .WithName("ResetPassword")
         .WithSummary("")
         .WithDescription("reset password for a user")
         .Produces(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status400BadRequest);

         group.MapPost("forgot-password", ForgotPassword)
         .WithName("ForgotPassword")
         .WithSummary("")
         .WithDescription("send password reset link to user email")
         .Produces(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status400BadRequest);


      return route;
      //.RequireAuthorization("AdminOnly");
   }

   private static async Task<IResult> RegisterUser(
      RegisterUserRequest dto,
      UserManager<User> userManager,
      RoleManager<IdentityRole> roleManager,
      IEmailSender emailSender,
      IConfiguration config)
   {
      if (await userManager.FindByEmailAsync(dto.Email) is not null)
      {
         return Results.BadRequest(new { Error = $"user with email {dto.Email} already exists" });
      }

      var user = new User
      {
         Email = dto.Email,
         UserName = dto.Email,
         FirstName = dto.FirstName,
         LastName = dto.LastName,
      };

      var tempPassword = "!TempPassword1234";

      var created = await userManager.CreateAsync(user, tempPassword);

      if (!created.Succeeded)
      {
         return Results.BadRequest(new { Error = created.Errors });
      }

      var role = "RESEARCHER";

      if (!await roleManager.RoleExistsAsync(role))
      {
         await roleManager.CreateAsync(new IdentityRole(role));

         await userManager.AddToRoleAsync(user, role);
      }

      await userManager.AddToRoleAsync(user, role);

      var token = await userManager.GeneratePasswordResetTokenAsync(user);
      var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

      var baseURL = config["BaseURL"] ?? "https://localhost:7034";

      await emailSender.SendEmailAsync(
         dto.Email,
         "Account created",
         $"""
             Your account has been created. Please change your password by visiting: {baseURL}/Identity/Account/Manage

             {baseURL}/Setpassword.html?email={dto.Email}&resetCode={encodedToken}
          """
      );

      return Results.Ok(new { Message = $"user with email {user.Email} created, Password reset link sent" });
   }

   private static async Task<IResult> ResetPassword(
      ResetPasswordRequest request, UserManager<User> userManager)
   {
      if (string.IsNullOrEmpty(request.Email) ||
          string.IsNullOrEmpty(request.ResetCode) ||
          string.IsNullOrEmpty(request.NewPassword)
         )
      {
         return Results.BadRequest(new { Message = "missings fields" });
      }

      var user = await userManager.FindByEmailAsync(request.Email);

      if (user is null)
      {
         return Results.BadRequest("User not found.");
      }

      try
      {
         var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));

         var result = await userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
if (!result.Succeeded)
{
    return Results.BadRequest(new
    {
        Message = "Password reset failed",
        Errors = result.Errors.Select(e => e.Description)
    });
}

         if (result.Succeeded)
         {
            return Results.Ok(new { Message = "Password reset successful" });
         }
         else
         {

            return Results.BadRequest(new { Message = "error" });
         }

      }
      catch (FormatException)
      {
         return Results.BadRequest(new { Message = "Invalid token" });
      }

      catch (Exception ex)
      {
         return Results.BadRequest(new { Message = $"Error: {ex.Message}" });
      }

   }


   private static async Task<IResult> ForgotPassword(
      ForgotPasswordRequest request,
      UserManager<User> userManager,
      IEmailSender emailSender,
      IConfiguration config)
   {
      if (string.IsNullOrEmpty(request.Email))
      {
         return Results.BadRequest(new { Message = "Email is required" });
      }

      var user = await userManager.FindByEmailAsync(request.Email);

      if (user is null)
      {
         return Results.BadRequest(new { Message = "User not found." });
      }

      var token = await userManager.GeneratePasswordResetTokenAsync(user);
      var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

      var baseURL = config["BaseURL"] ?? "https://localhost:7034";

      await emailSender.SendEmailAsync(
         request.Email,
         "Password Reset",
         $"""
             You requested a password reset. Please reset your password by visiting: {baseURL}/Identity/Account/Manage

             {baseURL}/Setpassword.html?email={request.Email}&resetCode={encodedToken}
          """
      );

      return Results.Ok(new { Message = $"Password reset link sent to {request.Email}" });
   }
}