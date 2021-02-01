using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConsumerWebClient
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_Click(object sender, EventArgs e)
        {
            ServiceReference2.Service1Client client = new ServiceReference2.Service1Client();
            var response = client.GetMessage(txt.Text);
            lbl.Text = response;
            //client.Close();
        }
    }
}