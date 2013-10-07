using System;
using System.Web.UI;

namespace BDOPayrollReporter
{
    public partial class Header : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Page.DataBind();
        }
    }
}