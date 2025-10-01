using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebForms
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // If user is already authenticated, redirect to home page
            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/");
            }
        }

        protected void LogIn_Click(object sender, EventArgs e)
        {
            // For simplicity, we're hardcoding credentials
            // In a real application, use a secure user store like ASP.NET Identity
            if (ValidateUser(Username.Text, Password.Text))
            {
                // Create authentication ticket
                FormsAuthentication.SetAuthCookie(Username.Text, RememberMe.Checked);
                
                // Redirect to originally requested URL or default page
                string returnUrl = Request.QueryString["ReturnUrl"];
                if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    Response.Redirect(returnUrl);
                }
                else
                {
                    Response.Redirect("~/");
                }
            }
            else
            {
                FailureText.Text = "Invalid username or password.";
            }
        }

        private bool ValidateUser(string username, string password)
        {
            // TODO: Replace with proper user authentication system
            // This is a simple example - DO NOT use in production!
            
            // For demo purposes only - hardcoded credentials:
            return (username == "admin" && password == "password123");
        }
    }
}