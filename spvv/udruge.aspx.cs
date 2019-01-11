using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Configuration;

namespace spvv {
    public partial class wfUdruge:System.Web.UI.Page {

        public string action = "";
        public int pageCurrent;
        public int pageSize = 10;
        public int pageCount;

        protected string nullstr(string ns) {
            string rez;
            if (ns == null || ns == "") {
                rez = "null";
            }
            else {
                rez = ns;
            }
            return rez;
        }

        protected void Page_Load(object sender,EventArgs e) {        
            if (Session["korisnik"] == null || Session["korisnik"].ToString().Length == 0) {
                Response.Redirect("login.aspx");
            }

            if (!IsPostBack) {
                ViewState["pageCurrent"] = 0;
                refreshGrid();

                string query = "select id, naziv from mjesta union select null, null from dual order by naziv";
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                ddlMjesto.DataSource = dt;
                ddlMjesto.DataTextField = "naziv";
                ddlMjesto.DataValueField = "id";
                ddlMjesto.DataBind();

                query = "select id, broj + ' ' + naziv as naziv from poste union select null, null from dual order by naziv";
                cmd.CommandText = query;
                dr = cmd.ExecuteReader();
                dt = new DataTable();
                dt.Load(dr);
                ddlPosta.DataSource = dt;
                ddlPosta.DataTextField = "naziv";
                ddlPosta.DataValueField = "id";
                ddlPosta.DataBind();

                con.Close();            
            }
            else {
                action = (String)ViewState["action"];
            }
            pageCurrent = Convert.ToInt32(ViewState["pageCurrent"]);
            pageCount = Convert.ToInt32(ViewState["pageCount"]);
        }

        protected void refreshGrid()
        { 
            OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
            OleDbCommand cmd = new OleDbCommand();     
            string query = "select count(udruge.id) from (udruge left join poste on udruge.pos_id = poste.id) left join mjesta on udruge.mje_id = mjesta.id where 1 = 1 " + ViewState["filter"];
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            con.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            if (dr.Read()) {
                pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dr[0] as int?) / Convert.ToDouble(pageSize)));            
            }
            else {
                pageCount = 0;
            }
            ViewState["pageCount"] = pageCount;
            query = "select udruge.id, udruge.naziv, iif(isnull(udruge.ulica), '', udruge.ulica) + ' ' + iif(isnull(udruge.kucnibroj), '', udruge.kucnibroj) + '; ' + iif(isnull(poste.broj), '', poste.broj) + ' ' + iif(isnull(poste.naziv), '', poste.naziv) as adresa, mjesta.naziv as mjesto from (udruge left join poste on udruge.pos_id = poste.id) left join mjesta on udruge.mje_id = mjesta.id where 1 = 1 " + ViewState["filter"] + " order by udruge.naziv";
            if (action == "Traži") {                
                cmd.CommandText = query;
                dr.Close();
                dr = cmd.ExecuteReader();
                int position = -1;
                while (dr.Read()) {
                    position++;
                    if (dr["id"].ToString() == hdnID.Value) {
                        pageCurrent = Convert.ToInt32(Math.Floor(Convert.ToDouble(position) / Convert.ToDouble(pageSize)));
                        ViewState["pageCurrent"] = pageCurrent;
                    }
                }
            }            
            query = "select * from (select top " + pageSize.ToString() + " * from (select top " + (pageSize * (pageCurrent + 1)).ToString() + " * from (" + query + ")) order by udruge.naziv desc) order by udruge.naziv";
            cmd.CommandText = query;
            dr.Close();
            dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);            
            rptMain.DataSource = dt;    
            rptMain.DataBind();
            con.Close();
        }

        protected void btnCommand_Click(object sender, EventArgs e)
        {
            HtmlButton hb = (HtmlButton)sender;
            action = hb.InnerText.Substring(hb.InnerText.IndexOf(">") + 1);
            if (Convert.ToInt32(Session["nivo"].ToString()) > 1 && (action == "Dodaj" || action == "Izmijeni" || action == "Obriši")) {
                action = "";
            }
            ViewState["action"] = action;
            if (action == "Izmijeni" && hdnID.Value != "") {
                string query = "select naziv, ulica, kucnibroj, pos_id, mje_id from udruge where id = " + hdnID.Value;
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                if (dr.Read()) {
                    txtNaziv.Text = (dr[0] as string);
                    txtUlica.Text = (dr[1] as string);
                    txtKucniBroj.Text = (dr[2] as string);
                    ddlPosta.SelectedValue = (dr[3] as int?).ToString();
                    ddlMjesto.SelectedValue = (dr[4] as int?).ToString();
                }
                con.Close();
            }
            if ((action == "Dodaj") || (action == "Izmijeni") || (action == "Traži") || (action == "Filtriraj")) {
                txtNaziv.Focus();
            }
        }

        protected void btnIspisi_Click(object sender, EventArgs e)
        {
            Body body;
            Paragraph par;
            Run run;
            DocumentFormat.OpenXml.Wordprocessing.TableRow tr;
            DocumentFormat.OpenXml.Wordprocessing.TableCell tc;
            TableCellProperties tcp;
            RunProperties rp;

            WordprocessingDocument package = WordprocessingDocument.Create(HttpRuntime.AppDomainAppPath + "\\udruge.docx", WordprocessingDocumentType.Document);
            package.AddMainDocumentPart(); 

            Document document = new Document();
            package.MainDocumentPart.Document = document;
            body = document.AppendChild(new Body());
            SectionProperties sectionProps = body.AppendChild(new SectionProperties());
            sectionProps.Append(new PageMargin() { Top = 720, Right = 720, Bottom = 720, Left = 720, Header = 0, Footer = 0, Gutter = 0 }); // 1440 = 1", 720 = 0.5";

            par = body.AppendChild(new Paragraph());
            run = par.AppendChild(new Run());
            rp = run.AppendChild(new RunProperties());
            rp.AppendChild(new Bold() { Val = OnOffValue.FromBoolean(true) });
            rp.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "32" });
            run.AppendChild(new Text("Udruge"));

            DocumentFormat.OpenXml.Wordprocessing.Table table = new DocumentFormat.OpenXml.Wordprocessing.Table();
            TableProperties props = new TableProperties
                (
                    new TableBorders
                        (
                            new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }, // 100%
                            new TopBorder
                            {
                                Val = new EnumValue<BorderValues>(BorderValues.None),
                                Size = 6
                            },
                            new BottomBorder
                            {
                                Val = new EnumValue<BorderValues>(BorderValues.Single),
                                Size = 6
                            },
                            new LeftBorder
                            {
                                Val = new EnumValue<BorderValues>(BorderValues.None),
                                Size = 6
                            },
                            new RightBorder
                            {
                                Val = new EnumValue<BorderValues>(BorderValues.None),
                                Size = 6
                            },
                            new InsideHorizontalBorder
                            {
                               Val = new EnumValue<BorderValues>(BorderValues.Single),
                                Size = 6
                            },
                            new InsideVerticalBorder
                            {
                                Val = new EnumValue<BorderValues>(BorderValues.None),
                                Size = 6
                            }
                        )
                );
                
            table.AppendChild<TableProperties>(props);

            // red
            tr = table.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableRow());
            // ćelija
            tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
            tcp = tc.AppendChild(new TableCellProperties());
            tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
            par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
            run = par.AppendChild(new Run());
            rp = run.AppendChild(new RunProperties());
            rp.Append(new Color() { Val = "ffffff" });
            rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
            run.Append(new Text("Naziv"));
            // ćelija
            tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
            tcp = tc.AppendChild(new TableCellProperties());
            tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
            par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
            run = par.AppendChild(new Run());
            rp = run.AppendChild(new RunProperties());
            rp.Append(new Color() { Val = "ffffff" });
            rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
            run.Append(new Text("Adresa"));
            // ćelija
            tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
            tcp = tc.AppendChild(new TableCellProperties());
            tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
            par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
            run = par.AppendChild(new Run());
            rp = run.AppendChild(new RunProperties());
            rp.Append(new Color() { Val = "ffffff" });
            rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
            run.Append(new Text("Mjesto"));

            string query = "select udruge.naziv, iif(isnull(udruge.ulica), '', udruge.ulica) + ' ' + iif(isnull(udruge.kucnibroj), '', udruge.kucnibroj) + '; ' + iif(isnull(poste.broj), '', poste.broj) + ' ' + iif(isnull(poste.naziv), '', poste.naziv) as adresa, mjesta.naziv as mjesto from (udruge left join poste on udruge.pos_id = poste.id) left join mjesta on udruge.mje_id = mjesta.id where 1 = 1 " + ViewState["filter"] + " order by udruge.naziv";
            OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
            OleDbCommand cmd = new OleDbCommand();     
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            con.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            
            while (dr.Read())
            {
                tr = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                run.Append(new Text(dr[0] as string));

                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                run.Append(new Text(dr[1] as string));

                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                run.Append(new Text(dr[2] as string));

                table.Append(tr);
            }
            document.Body.Append(table);
            document.Save();
            package.Close();

            con.Close();

            Response.ContentType = "Application/docx";
            Response.AppendHeader("Content-Disposition", "attachment; filename=udruge.docx");
            Response.TransmitFile(HttpRuntime.AppDomainAppPath + "\\udruge.docx");
            Response.End();
        }

        protected void btnPotvrdi_Click(object sender, EventArgs e)
        {
            if (action == "Dodaj")
            {
                string query = "insert into udruge (naziv, ulica, kucnibroj, pos_id, mje_id) values ('" + txtNaziv.Text + "', '" + txtUlica.Text + "', '" + txtKucniBroj.Text + "', " + nullstr(ddlPosta.SelectedValue) + ", " + nullstr(ddlMjesto.SelectedValue) + ")";
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                try {
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "insert into log (kor_id, upit) values (" + Session["kor_id"] + ", '" + query.Replace('\'', '"') + "')";
                    cmd.ExecuteNonQuery();
                    action = "Traži";
                    lblGreska.Text = "";
                }
                catch {
                    lblGreska.Text = "Nije moguće unijeti.";
                }
                con.Close();
                refreshGrid();
            }
            if (action == "Izmijeni")
            {
                string query = "update udruge set naziv = '" + txtNaziv.Text + "', ulica = '" + txtUlica.Text + "', kucnibroj = '" + txtKucniBroj.Text + "', pos_id = " + nullstr(ddlPosta.SelectedValue) + ", mje_id = " + nullstr(ddlMjesto.SelectedValue) + " where id = " + hdnID.Value;
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                try {
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "insert into log (kor_id, upit) values (" + Session["kor_id"] + ", '" + query.Replace('\'', '"') + "')";
                    cmd.ExecuteNonQuery();
                    action = "Traži";
                    lblGreska.Text = "";
                }
                catch {
                    lblGreska.Text = "Nije moguće unijeti.";
                }
                con.Close();
                refreshGrid();
            }
            if (action == "Obriši")
            {
                string query = "delete from udruge where id = " + hdnID.Value;
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                try {
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "insert into log (kor_id, upit) values (" + Session["kor_id"] + ", '" + query.Replace('\'', '"') + "')";
                    cmd.ExecuteNonQuery();
                    hdnID.Value = "";
                    lblGreska.Text = "";
                }
                catch {
                    lblGreska.Text = "Nije moguće obrisati.";
                }
                con.Close();
                refreshGrid();
            }
            if ((action == "Filtriraj") || (action == "Traži")) {
                string filter = "";
                if (txtNaziv.Text != "" && txtNazivDo.Text != "") {
                    filter = " and udruge.naziv >= '" + txtNaziv.Text + "' and (udruge.naziv <= '" + txtNazivDo.Text + "' or udruge.naziv <= (select max(naziv) from udruge where naziv like '" + txtNazivDo.Text + "%'))";
                }
                else if (txtNaziv.Text != "") {
                    filter = " and udruge.naziv like '%" + txtNaziv.Text + "%'";
                }
                if (txtUlica.Text != "" && txtUlicaDo.Text != "") {
                    filter = filter + " and udruge.ulica >= '" + txtUlica.Text + "' and (udruge.ulica <= '" + txtUlicaDo.Text + "' or udruge.ulica <= (select max(ulica) from udruge where ulica like '" + txtUlicaDo.Text + "%'))";
                }
                else if (txtUlica.Text != "") {
                    filter = filter + " and udruge.ulica like '%" + txtUlica.Text + "%'";
                }
                if (txtKucniBroj.Text != "" && txtKucniBrojDo.Text != "") {
                    filter = filter + " and udruge.kucnibroj >= '" + txtKucniBroj.Text + "' and (udruge.kucnibroj <= '" + txtKucniBrojDo.Text + "' or udruge.kucnibroj <= (select max(kucnibroj) from udruge where kucnibroj like '" + txtKucniBrojDo.Text + "%'))";
                }
                else if (txtKucniBroj.Text != "") {
                    filter = filter + " and udruge.kucnibroj like '%" + txtKucniBroj.Text + "%'";
                }
                if (txtPosta.Text != "" && txtPostaDo.Text != "") {
                    filter = filter + " and ((poste.broj >= '" + txtPosta.Text + "' and (poste.broj <= '" + txtPostaDo.Text + "' or poste.broj <= (select max(poste.broj) from udruge inner join poste on udruge.pos_id = poste.id where poste.broj like '" + txtPostaDo.Text + "%'))) or " +
                                      "(poste.naziv >= '" + txtPosta.Text + "' and (poste.naziv <= '" + txtPostaDo.Text + "' or poste.naziv <= (select max(poste.naziv) from udruge inner join poste on udruge.pos_id = poste.id where poste.naziv like '" + txtPostaDo.Text + "%'))))";
                }
                else if (txtPosta.Text != "") {
                    filter = filter + " and (poste.broj like '%" + txtPosta.Text + "%') or (poste.naziv like '%" + txtPosta.Text + "%')";
                }
                if (txtMjesto.Text != "" && txtMjestoDo.Text != "") {
                    filter = filter + " and mjesta.naziv >= '" + txtMjesto.Text + "' and (mjesta.naziv <= '" + txtMjestoDo.Text + "' or mjesta.naziv <= (select max(mjesta.naziv) from udruge inner join mjesta on udruge.mje_id = mjesta.id where mjesta.naziv like '" + txtMjestoDo.Text + "%'))";
                }
                else if (txtMjesto.Text != "") {
                    filter = filter + " and mjesta.naziv like '%" + txtMjesto.Text + "%'";
                }
                if (action == "Filtriraj")
                {
                    ViewState["filter"] = filter;
                    hdnID.Value = "";
                    pageCurrent = 0;
                    ViewState["pageCurrent"] = pageCurrent;
                    refreshGrid();
                }
                if (action == "Traži")
                {
                    string query = "select id from udruge where naziv = (select min(udruge.naziv) from (udruge left join poste on udruge.pos_id = poste.id) left join mjesta on udruge.mje_id = mjesta.id where 1 = 1 " + filter + " " + ViewState["filter"] + ")";
                    OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                    OleDbCommand cmd = new OleDbCommand();     
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    OleDbDataReader dr = cmd.ExecuteReader();
                    if (dr.Read()) {
                        hdnID.Value = (dr[0] as int?).ToString();
                        refreshGrid();
                    }
                    else {
                        query = "select id from udruge where naziv = (select min(udruge.naziv) from (udruge left join poste on udruge.pos_id = poste.id) left join mjesta on udruge.mje_id = mjesta.id where 1 = 1 " + filter + ")";
                        cmd.CommandText = query;
                        dr.Close();
                        dr = cmd.ExecuteReader();
                        if (dr.Read()) {
                            hdnID.Value = (dr[0] as int?).ToString();
                            ViewState["filter"] = "";
                            refreshGrid();
                        }
                        else {
                            hdnID.Value = "";
                        }
                    }
                    con.Close();                
                }
            }
            if (lblGreska.Text == "") {
                action = "";
            }
            ViewState["action"] = action;
        }
    
        protected void btnOdustani_Click(object sender, EventArgs e)
        {
            lblGreska.Text = "";
            action = "";
            ViewState["action"] = action;
        }


        protected void btnIduca_Click(object sender, EventArgs e)
        {
            if (pageCurrent < pageCount - 1) {
                pageCurrent = pageCurrent + 1;
                ViewState["pageCurrent"] = pageCurrent;
                refreshGrid();
            }
        }

        protected void btnPrethodna_Click(object sender, EventArgs e)
        {
            if (pageCurrent > 0) {
                pageCurrent = pageCurrent - 1;
                ViewState["pageCurrent"] = pageCurrent;
                refreshGrid();
            }
        }

        protected void btnPrva_Click(object sender, EventArgs e)
        {
            if (pageCurrent != 0) {
                pageCurrent = 0;
                ViewState["pageCurrent"] = pageCurrent;
                refreshGrid();
            }
        }

        protected void btnZadnja_Click(object sender, EventArgs e)
        {
            if (pageCurrent != pageCount - 1) {
                pageCurrent = pageCount - 1;
                ViewState["pageCurrent"] = pageCurrent;
                refreshGrid();
            }
        }
     }
}