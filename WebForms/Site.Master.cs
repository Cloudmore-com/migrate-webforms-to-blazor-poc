using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebForms
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Update the login status controls based on authentication status
            if (!IsPostBack)
            {
                UpdateLoginStatus();
            }
        }

        private void UpdateLoginStatus()
        {
            if (Page.User.Identity.IsAuthenticated)
            {
                LoginStatusText.Text = "Hello, " + Page.User.Identity.Name + "!";
                LoginLink.Visible = false;
                LogoutLink.Visible = true;
            }
            else
            {
                LoginStatusText.Text = "";
                LoginLink.Visible = true;
                LogoutLink.Visible = false;
            }
        }
    }
}