
/*
Copyright (C) 2023
Elias Stepanik: https://github.com/eliasstepanik
Olivia Streun: https://github.com/nnuuvv

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see https://www.gnu.org/licenses/.
*/

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