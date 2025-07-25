using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebForms
{
    public partial class Logout : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Log out the user
            FormsAuthentication.SignOut();

            // Clear all session variables
            Session.Clear();
            Session.Abandon();

            // Clear any cookies
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            // Redirect to the login page after 2 seconds
            Response.AddHeader("REFRESH", "2;URL=Login.aspx");
        }
    }
}