using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Novell.Directory.Ldap;

namespace Kassensystem.Controllers;

public class LoginController : Controller
{
    public static bool SimpleLdapAuth(string username, string password, out LdapEntry? resultEntity)
    {
        string Host, BindDN, BindPassword, BaseDC;
        int Port;
        
        #if (DEBUG)
        Host = "ldap.forumsys.com";
        BindDN = "cn=read-only-admin,dc=example,dc=com";
        BindPassword = "password";
        BaseDC = "dc=example,dc=com";
        Port = LdapConnection.DEFAULT_PORT;
        #else
        Host = Environment.GetEnvironmentVariable("LDAP_HOST") ?? "ldap.forumsys.com";
        BindDN = Environment.GetEnvironmentVariable("LDAP_BindDN") ?? "cn=read-only-admin,dc=example,dc=com";
        BindPassword = Environment.GetEnvironmentVariable("LDAP_BindPassword") ?? "password";
        BaseDC = Environment.GetEnvironmentVariable("LDAP_BaseDC") ?? "dc=example,dc=com";
        Port = int.Parse(Environment.GetEnvironmentVariable("LDAP_PORT") ?? LdapConnection.DEFAULT_PORT.ToString());
        #endif
        

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
    [HttpPost("/account/login")]
    public async Task<IActionResult> Login(UserCredentials credentials)
    {
        var User = new { username = credentials.Username, password = credentials.Password };

        var result = SimpleLdapAuth(User.username, User.password,out var resultEntity);

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

    [HttpGet("/account/logout")]
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