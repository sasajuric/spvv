using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace spvv {
    public partial class WebForm2:System.Web.UI.Page {
        protected void Page_Load(object sender,EventArgs e) {
            Session["kor_id"] = "";
            Session["korisnik"] = "";
            Session["nivo"] = "";
            txtKorisnik.Focus();
            //Session["korisnik"] = "Saša Jurić";            
            //Response.Redirect("vodici.aspx");
        }

        protected void btnPotvrdi_Click(object sender, EventArgs e)
        {
            string query = "select id, nivo from korisnici where korisnik = '" + txtKorisnik.Text + "' and lozinka = '" + txtLozinka.Text + "'";
            OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
            OleDbCommand cmd = new OleDbCommand();     
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            con.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            if (dr.Read()) {
                Session["kor_id"] = (dr["id"] as int?).ToString();
                Session["korisnik"] = txtKorisnik.Text;
                Session["nivo"] = (dr["nivo"] as int?).ToString();
                dr.Close();
                cmd.CommandText = "insert into log (kor_id, upit) values (" + Session["kor_id"] + ", 'LOGIN')";
                cmd.ExecuteNonQuery();
                Response.Redirect("default.aspx");
            }
            else
            {
                lblInfo.Text = "Neispravna lozinka.";
            }
            con.Close();
        }

        protected void btnOdustani_Click(object sender, EventArgs e)
        {

        }

    }
}