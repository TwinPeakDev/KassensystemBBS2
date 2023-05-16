using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Novell.Directory.Ldap;

namespace Kassensystem.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    public static bool LdapAuth(string username, string password, out LdapEntry? resultEntity)
    {
        string Host, BindDN, BindPassword, BaseDC;
        int Port;
        
        
        //TODO: Add null Checks
        Host = Environment.GetEnvironmentVariable("LDAP_HOST") ?? "ldap.forumsys.com";
        BindDN = Environment.GetEnvironmentVariable("LDAP_BindDN") ?? "cn=read-only-admin,dc=example,dc=com";
        BindPassword = Environment.GetEnvironmentVariable("LDAP_BindPassword") ?? "password";
        BaseDC = Environment.GetEnvironmentVariable("LDAP_BaseDC") ?? "dc=example,dc=com";
        Port = int.Parse(Environment.GetEnvironmentVariable("LDAP_PORT") ?? LdapConnection.DEFAULT_PORT.ToString());


        resultEntity = null;

        try
        {
            var connection = new LdapConnection();
            connection.Connect(Host, Port);
            connection.Bind(BindDN, BindPassword);
            var searchFilter = $"(uid={username})";
            var entities = connection.Search(BaseDC, LdapConnection.SCOPE_SUB, searchFilter, null, false);
            string userDn = null;
            while (entities.hasMore())
            {
                var entity = entities.next();
                var account = entity.getAttribute("uid");
                if (account != null && account.StringValue == username)
                {
                    userDn = entity.DN;
                    resultEntity = entity;
                    break;
                }
            }
            if (string.IsNullOrWhiteSpace(userDn)) return false;
            connection.Bind(userDn, password);
            return connection.Bound;
        }
        catch (LdapException e)
        {
            throw e;
        }
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserCredentials credentials)
    {
        var User = new { username = credentials.Username, password = credentials.Password };

        var result = LdapAuth(User.username, User.password,out var resultEntity);

        if (result == true)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, User.username),
                new Claim(ClaimTypes.Role, $"{resultEntity.getAttribute("gidNumber").StringValue}")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            return LocalRedirect("/");
        }

        return LocalRedirect("/login/Invalid credentials");
    }
    
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return LocalRedirect("/");
    }
    
}

public class UserCredentials
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}