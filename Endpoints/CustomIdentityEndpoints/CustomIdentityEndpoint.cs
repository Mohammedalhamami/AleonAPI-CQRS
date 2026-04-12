using System.Collections;
using System.Security.Claims;
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

      group.MapGet("/manage/profile", GetProfileInfo)
      .WithName("GetProfileInfo")
      .WithSummary("Get user profile information")
      .WithDescription("Get the profile information of the currently authenticated user")
      .RequireAuthorization()
      .Produces(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound)
      .Produces(StatusCodes.Status401Unauthorized);

      group.MapPut("/manage/profile", UpdateProfileInfo)
      .WithName("UpdateProfile")
      .WithSummary("Update user profile information")
      .WithDescription("Update the profile information of the currently authenticated user")
      .RequireAuthorization()
      .Produces(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound)
      .Produces(StatusCodes.Status401Unauthorized);

      group.MapGet("/manage/users", GetUsersList)
      .WithName("GetUsersList")
      .WithSummary("Get list of all users")
      .WithDescription("Get a list of all registered users (Admin only)")
      .RequireAuthorization()
      .Produces<IEnumerable<UserProfileResponse>>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status401Unauthorized)
      .Produces(StatusCodes.Status403Forbidden);


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


   private static async Task<IResult> GetProfileInfo(ClaimsPrincipal userClaims, UserManager<User> userManager)
   {

      var user = await userManager.GetUserAsync(userClaims);

      if (user is null)
      {
         return Results.Unauthorized();
      }

      var profileInfo = new UserProfileResponse
      {
         Id = user.Id,
         Email = user.Email,
         FirstName = user.FirstName,
         LastName = user.LastName,
         FullName = $"{user.FirstName} {user.LastName}",

      };

      return Results.Ok(profileInfo);
   }


   private static async Task<IResult> UpdateProfileInfo(
      ClaimsPrincipal userClaims,
      UserManager<User> userManager,
      UpdateProfileRequest request)
   {
      var user = await userManager.GetUserAsync(userClaims);

      if (user is null)
      {
         return Results.Unauthorized();
      }

      user.FirstName = request.FirstName;
      user.LastName = request.LastName;

      if (string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName))
      {
         return Results.BadRequest(new { Message = "First name and last name cannot be empty" });
      }

      var result = await userManager.UpdateAsync(user);

      if (result.Succeeded)
      {
         return Results.Ok(new { Message = "Profile updated successfully" });
      }
      else
      {
         return Results.BadRequest(new
         {
            Message = "Profile update failed",
            Errors = result.Errors.Select(e => e.Description)
         });
      }
   }

   private static async Task<IResult> GetUsersList(UserManager<User> userManager)
   {
      var users = userManager.Users.Select(u => new UserProfileResponse
      {
         Id = u.Id,
         Email = u.Email,
         FirstName = u.FirstName,
         LastName = u.LastName,
         FullName = $"{u.FirstName} {u.LastName}",
      }).ToList();

      return Results.Ok(users);
   }

}