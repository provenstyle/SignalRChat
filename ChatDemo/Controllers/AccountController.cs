using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ChatDemo.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public void LogIn(string username, string password)
        {
            if (username == "lvs" && password == "Pass@word1")
            {
                FormsAuthentication.Initialize();
                FormsAuthentication.SetAuthCookie(username, false);
                var ticket = new FormsAuthenticationTicket(
                   1, // Ticket version
                   username, // Username associated with ticket
                   DateTime.Now, // Date/time issued
                   DateTime.Now.AddMinutes(30), // Date/time to expire
                   true, // "true" for a persistent user cookie
                   "", // User-data, in this case the roles
                   FormsAuthentication.FormsCookiePath);// Path cookie valid for

                // Encrypt the cookie using the machine key for secure transport
                string hash = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie(
                   FormsAuthentication.FormsCookieName, // Name of auth cookie
                   hash); // Hashed ticket

                // Set the cookie's expiration time to the tickets expiration time
                if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;

                // Add the cookie to the list for outgoing response
                Response.Cookies.Add(cookie);

            }
        }
    }
}
