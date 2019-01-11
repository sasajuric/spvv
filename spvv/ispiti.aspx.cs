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
    public partial class wfIspiti:System.Web.UI.Page {

        public string action = "";
        public string actionInstruktori = "";
        public string actionKandidati = "";
        public int pageCurrent;
        public int pageSize = 10;
        public int pageCount;
        public int pageCurrentInstruktori;
        public int pageSizeInstruktori = 10;
        public int pageCountInstruktori;
        public int pageCurrentKandidati;
        public int pageSizeKandidati = 10;
        public int pageCountKandidati;

        protected void Page_Load(object sender,EventArgs e) {        
            if (Session["korisnik"] == null || Session["korisnik"].ToString().Length == 0) {
                Response.Redirect("login.aspx");
            }

            if (!IsPostBack) {
                ViewState["pageCurrent"] = 0;
                refreshGrid();

                string query = "select id, prezime + ', ' + ime as instruktor from vodici where instruktor <> '' or instruktorkandidat <> '' union select null, null from dual order by instruktor";
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);                
                ddlVoditelj.DataSource = dt;
                ddlVoditelj.DataTextField = "instruktor";
                ddlVoditelj.DataValueField = "id";
                ddlVoditelj.DataBind();
                query = "select id, naziv from standardi union select null, null from dual order by naziv";
                cmd.CommandText = query;
                dr = cmd.ExecuteReader();
                dt = new DataTable();
                dt.Load(dr);
                ddlStandard.DataSource = dt;
                ddlStandard.DataTextField = "naziv";
                ddlStandard.DataValueField = "id";
                ddlStandard.DataBind();                
                con.Close();
            }
            else {
                action = (String)ViewState["action"];
                actionInstruktori = (String)ViewState["actionInstruktori"];
                actionKandidati = (String)ViewState["actionKandidati"];
            }
            pageCurrent = Convert.ToInt32(ViewState["pageCurrent"]);
            pageCount = Convert.ToInt32(ViewState["pageCount"]);
            pageCurrentInstruktori = Convert.ToInt32(ViewState["pageCurrentInstruktori"]);
            pageCountInstruktori = Convert.ToInt32(ViewState["pageCountInstruktori"]);
            pageCurrentKandidati = Convert.ToInt32(ViewState["pageCurrentKandidati"]);
            pageCountKandidati = Convert.ToInt32(ViewState["pageCountKandidati"]);
        }

        protected void refreshGrid()
        { 
            OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
            OleDbCommand cmd = new OleDbCommand();     
            string query = "select count(ispiti.id) from (ispiti inner join standardi on ispiti.std_id = standardi.id) left join vodici on ispiti.voditelj_vod_id = vodici.id where 1 = 1 " + ViewState["filter"];
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
            query = "select ispiti.id, ispiti.naziv, ispiti.datum, standardi.naziv as standard, vodici.prezime + ', ' + vodici.ime as voditelj from (ispiti inner join standardi on ispiti.std_id = standardi.id) left join vodici on ispiti.voditelj_vod_id = vodici.id where 1 = 1 " + ViewState["filter"] + " order by ispiti.datum desc";
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
            query = "select * from (select top " + pageSize.ToString() + " * from (select top " + (pageSize * (pageCurrent + 1)).ToString() + " * from (" + query + ")) order by ispiti.datum) order by ispiti.datum desc";
            cmd.CommandText = query;
            dr.Close();
            dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);            
            rptMain.DataSource = dt;    
            rptMain.DataBind();
            con.Close();
        }

        protected void refreshGridInstruktori()
        { 
            OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
            OleDbCommand cmd = new OleDbCommand();     
            string query = "select count(ispitiinstruktori.id) from ispitiinstruktori left join vodici on ispitiinstruktori.vod_id = vodici.id where ispitiinstruktori.isp_id = " + hdnID.Value + " " + ViewState["filterinstruktori"];
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            con.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            if (dr.Read()) {
                pageCountInstruktori = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dr[0] as int?) / Convert.ToDouble(pageSize)));            
            }
            else {
                pageCountInstruktori = 0;
            }
            ViewState["pageCountInstruktori"] = pageCountInstruktori;
            query = "select ispitiinstruktori.id, vodici.prezime + ', ' + vodici.ime as instruktor from ispitiinstruktori left join vodici on ispitiinstruktori.vod_id = vodici.id where ispitiinstruktori.isp_id = " + hdnID.Value + " " + ViewState["filterinstruktori"] + " order by vodici.prezime, vodici.ime";
            if (actionInstruktori == "Traži") {                
                cmd.CommandText = query;
                dr.Close();
                dr = cmd.ExecuteReader();
                int position = -1;
                while (dr.Read()) {
                    position++;
                    if (dr["id"].ToString() == hdnInstruktoriID.Value) {
                        pageCurrentInstruktori = Convert.ToInt32(Math.Floor(Convert.ToDouble(position) / Convert.ToDouble(pageSize)));
                        ViewState["pageCurrentInstruktori"] = pageCurrentInstruktori;
                    }
                }
            }            
            query = "select * from (select top " + pageSizeInstruktori.ToString() + " * from (select top " + (pageSizeInstruktori * (pageCurrentInstruktori + 1)).ToString() + " * from (" + query + ")) order by instruktor desc) order by instruktor";
            cmd.CommandText = query;
            dr.Close();
            dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);            
            rptInstruktori.DataSource = dt;    
            rptInstruktori.DataBind();

            query = "select id, prezime + ', ' + ime as instruktor from vodici where instruktor like '%' + (select standardi.naziv from standardi where standardi.id = (select ispiti.std_id from ispiti where id = " + hdnID.Value + ")) + '%' or instruktorkandidat like '%' + (select standardi.naziv from standardi where standardi.id = (select ispiti.std_id from ispiti where id = " + hdnID.Value + ")) + '%' union select null, null from dual order by instruktor";
            cmd.CommandText = query;
            dr.Close();
            dr = cmd.ExecuteReader();
            dt.Clear();
            dt.Load(dr);
            ddlInstruktor.Items.Clear();       
            ddlInstruktor.DataSource = dt;
            ddlInstruktor.DataTextField = "instruktor";
            ddlInstruktor.DataValueField = "id";
            ddlInstruktor.DataBind();

            con.Close();
        }

        protected void refreshGridKandidati()
        { 
            OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
            OleDbCommand cmd = new OleDbCommand();     
            string query = "select count(ispitikandidati.id) from ispitikandidati left join vodici on ispitikandidati.vod_id = vodici.id where ispitikandidati.isp_id = " + hdnID.Value + " " + ViewState["filterkandidati"];
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            con.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            if (dr.Read()) {
                pageCountKandidati = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dr[0] as int?) / Convert.ToDouble(pageSize)));            
            }
            else {
                pageCountKandidati = 0;
            }
            ViewState["pageCountKandidati"] = pageCountKandidati;
            query = "select ispitikandidati.id, vodici.prezime + ', ' + vodici.ime as kandidat, ispitikandidati.polozio from ispitikandidati left join vodici on ispitikandidati.vod_id = vodici.id where ispitikandidati.isp_id = " + hdnID.Value + " " + ViewState["filterkandidati"] + " order by vodici.prezime, vodici.ime";
            if (actionKandidati == "Traži") {                
                cmd.CommandText = query;
                dr.Close();
                dr = cmd.ExecuteReader();
                int position = -1;
                while (dr.Read()) {
                    position++;
                    if (dr["id"].ToString() == hdnKandidatiID.Value) {
                        pageCurrentKandidati = Convert.ToInt32(Math.Floor(Convert.ToDouble(position) / Convert.ToDouble(pageSize)));
                        ViewState["pageCurrentKandidati"] = pageCurrentKandidati;
                    }
                }
            }            
            query = "select * from (select top " + pageSizeKandidati.ToString() + " * from (select top " + (pageSizeKandidati * (pageCurrentKandidati + 1)).ToString() + " * from (" + query + ")) order by kandidat desc) order by kandidat";
            cmd.CommandText = query;
            dr.Close();
            dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);            
            rptKandidati.DataSource = dt;    
            rptKandidati.DataBind();
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
                string query = "select naziv, datum, std_id, voditelj_vod_id from ispiti where id = " + hdnID.Value;
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                if (dr.Read()) {                    
                    txtNaziv.Text = (dr[0] as string);
                    if (dr["datum"].ToString() != "") {
                        txtDatum.Text = Convert.ToDateTime(dr["datum"]).ToString("dd.MM.yyyy.");
                    }
                    ddlStandard.SelectedValue = (dr[2] as int?).ToString();
                    ddlVoditelj.SelectedValue = (dr[3] as int?).ToString();
                }
                con.Close();
            }
            if ((action == "Dodaj") || (action == "Izmijeni") || (action == "Traži") || (action == "Filtriraj")) {
                txtNaziv.Focus();
            }
            if (action == "Instruktori" && hdnID.Value != "") {
                ViewState["filterinstruktori"] = "";
                hdnInstruktoriID.Value = "";
                hdnKandidatiID.Value = "";
                refreshGridInstruktori();
                actionKandidati = "";
                ViewState["actionKandidati"] = actionKandidati;
            }
            if (action == "Kandidati" && hdnID.Value != "") {
                ViewState["filterkandidati"] = "";
                hdnInstruktoriID.Value = "";
                hdnKandidatiID.Value = "";
                refreshGridKandidati();
                actionInstruktori = "";
                ViewState["actionInstruktori"] = actionInstruktori;
            }
        }

        protected void btnInstruktoriCommand_Click(object sender, EventArgs e)
        {
            HtmlButton hb = (HtmlButton)sender;
            actionInstruktori = hb.InnerText.Substring(hb.InnerText.IndexOf(">") + 1);
            if (Convert.ToInt32(Session["nivo"].ToString()) > 1 && (actionInstruktori == "Dodaj" || actionInstruktori == "Izmijeni" || actionInstruktori == "Obriši")) {
                actionInstruktori = "";
            }
            ViewState["actionInstruktori"] = actionInstruktori;
            if (actionInstruktori == "Izmijeni" && hdnID.Value != "") {
                string query = "select vod_id from ispitiinstruktori where id = " + hdnInstruktoriID.Value;
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                if (dr.Read()) {
                    ddlInstruktor.SelectedValue = (dr[0] as int?).ToString();
                }
                con.Close();
            }
            if (actionInstruktori == "Dodaj" || action == "Izmijeni")  {
                ddlInstruktor.Focus();
            }
            if (actionInstruktori == "Traži" || actionInstruktori == "Filtriraj") {
                txtInstruktor.Focus();
            }
        }

        protected void btnKandidatiCommand_Click(object sender, EventArgs e)
        {
            HtmlButton hb = (HtmlButton)sender;
            actionKandidati = hb.InnerText.Substring(hb.InnerText.IndexOf(">") + 1);
            if (Convert.ToInt32(Session["nivo"].ToString()) > 1 && (actionKandidati == "Dodaj" || actionKandidati == "Izmijeni" || actionKandidati == "Obriši")) {
                actionKandidati = "";
            }
            ViewState["actionKandidati"] = actionKandidati;
            if (actionKandidati == "Izmijeni" && hdnID.Value != "") {
                string query = "select vod_id, polozio from ispitikandidati where id = " + hdnKandidatiID.Value;
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                if (dr.Read()) {
                    ddlKandidat.SelectedValue = (dr[0] as int?).ToString();
                    if (Convert.ToBoolean(dr[1]) == true) {
                        cbPolozio.Checked = true;
                    }
                    else {
                        cbPolozio.Checked = false;
                    }
                }
                con.Close();
            }
            if (actionKandidati == "Dodaj" || actionKandidati == "Izmijeni") {
                string query = 
                    "select id, prezime + ', ' + ime as kandidat from vodici " +
                    "where id in " + // bio je na tečaju
                    "( " +
                    "select " +
                    "  tecajevitecajci.vod_id " +
                    "from " +
                    "  tecajevi " +
                    "  inner join tecajevitecajci on tecajevi.id = tecajevitecajci.tec_id " +
                    "where " +
                    "  tecajevi.std_id = (select std_id from ispiti where id = " + hdnID.Value + ") " +
                    ")" +
                    "and id not in " + // ako je i bio na ispitu, ali nije prošao
                    "( " +
                    "select " +
                    "  ispitikandidati.vod_id " +
                    "from " +
                    "  ispiti " +
                    "  inner join ispitikandidati on ispiti.id = ispitikandidati.isp_id " +
                    "where " +
                    "  ispiti.std_id = (select std_id from ispiti where id = " + hdnID.Value + ") " +
                    "  and ispitikandidati.polozio = true " +
                    ")  " +
                    "order by prezime, ime";
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();                
                OleDbDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                con.Close();
                ddlKandidat.Items.Clear();
                ddlKandidat.DataSource = dt;
                ddlKandidat.DataTextField = "kandidat";
                ddlKandidat.DataValueField = "id";
                ddlKandidat.DataBind();
                ddlKandidat.Focus();
            }
            if (actionKandidati == "Traži" || actionKandidati == "Filtriraj") {
                txtKandidat.Focus();
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

            WordprocessingDocument package = WordprocessingDocument.Create(HttpRuntime.AppDomainAppPath + "\\ispiti.docx", WordprocessingDocumentType.Document);
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
            run.AppendChild(new Text("Vodiči"));

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
            run.Append(new Text("Datum"));
            // ćelija
            tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
            tcp = tc.AppendChild(new TableCellProperties());
            tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
            par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
            run = par.AppendChild(new Run());
            rp = run.AppendChild(new RunProperties());
            rp.Append(new Color() { Val = "ffffff" });
            rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
            run.Append(new Text("Standard"));
            // ćelija
            tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
            tcp = tc.AppendChild(new TableCellProperties());
            tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
            par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
            run = par.AppendChild(new Run());
            rp = run.AppendChild(new RunProperties());
            rp.Append(new Color() { Val = "ffffff" });
            rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
            run.Append(new Text("Voditelj"));

            string query = "select ispiti.naziv, ispiti.datum, standardi.naziv as naziv, vodici.prezime + ', ' + vodici.ime as voditelj from (ispiti inner join standardi on ispiti.std_id = standardi.id) left join vodici on ispiti.voditelj_vod_id = vodici.id where 1 = 1 " + ViewState["filter"] + " order by ispiti.datum desc";
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
                run.Append(new Text(Convert.ToDateTime(dr[1]).ToString("dd.MM.yyyy.")));

                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                run.Append(new Text(dr[2] as string));

                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                run.Append(new Text(dr[3] as string));

                table.Append(tr);
            }
            document.Body.Append(table);
            document.Save();
            package.Close();

            con.Close();

            Response.ContentType = "Application/docx";
            Response.AppendHeader("Content-Disposition", "attachment; filename=ispiti.docx");
            Response.TransmitFile(HttpRuntime.AppDomainAppPath + "\\ispiti.docx");
            Response.End();
        }

        protected void btnInstruktoriIspisi_Click(object sender, EventArgs e)
        {
        }

        protected void btnKandidatiIspisi_Click(object sender, EventArgs e)
        {
        }

        protected void btnPotvrdi_Click(object sender, EventArgs e)
        {
            if (action == "Dodaj")
            {
                string query = "insert into ispiti (naziv, datum, std_id, voditelj_vod_id) values ('" + txtNaziv.Text + "', '" + txtDatum.Text + "', " + ddlStandard.SelectedValue + ", " + ddlVoditelj.SelectedValue + ")";
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
                string query = "update ispiti set naziv = '" + txtNaziv.Text + "', datum = '" + txtDatum.Text + "', std_id = " + ddlStandard.SelectedValue + ", voditelj_vod_id = " + ddlVoditelj.SelectedValue + " where id = " + hdnID.Value;
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
                string query = "delete from ispiti where id = " + hdnID.Value;
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
                    filter = " and ispiti.naziv >= '" + txtNaziv.Text + "' and (ispiti.naziv <= '" + txtNazivDo.Text + "' or ispiti.naziv <= (select max(naziv) from ispiti where naziv like '" + txtNazivDo.Text + "%'))";
                }
                else if (txtNaziv.Text != "") {
                    filter = " and ispiti.naziv like '%" + txtNaziv.Text + "%'";
                }
                DateTime dt;
                string d1 = "";
                string d2 = "";
                try {
                    dt = Convert.ToDateTime(txtDatum.Text);
                    d1 = "#" + dt.Month + "/" + dt.Day + "/" + dt.Year + "#";
                }
                catch {
                }
                try {
                    dt = Convert.ToDateTime(txtDatumDo.Text);
                    d2 = "#" + dt.Month + "/" + dt.Day + "/" + dt.Year + "#";
                }
                catch {
                }
                if (d1 != "" && d2 != "") {
                    filter = filter + " and ispiti.datum >= " + d1 + " and ispiti.datum <= " + d2;
                }
                else if (d1 != "") {
                    filter = filter + " and ispiti.datum = " + d1;
                }                
                if (txtStandard.Text != "" && txtStandardDo.Text != "") {
                    filter = filter + " and standardi.naziv >= '" + txtStandard.Text + "' and (standardi.naziv <= '" + txtStandardDo.Text + "' or standardi.naziv <= (select max(standardi.naziv) from tecajevi inner join standardi on tecajevi.std_id = standardi.id where standardi.naziv like '" + txtStandardDo.Text + "%'))";
                }
                else if (txtStandard.Text != "") {
                    filter = filter + " and ispiti.naziv like '%" + txtStandard.Text + "%'";
                }
                if (txtVoditelj.Text != "" && txtVoditeljDo.Text != "") {
                    filter = filter + " and vodici.prezime + ', ' + vodici.ime >= '" + txtVoditelj.Text + "' and (vodici.prezime + ', ' + vodici.ime <= '" + txtVoditeljDo.Text + "' or vodici.prezime + ', ' + vodici.ime <= (select max(vodici.prezime + ', ' + vodici.ime) from ispiti inner join vodici on ispiti.voditelj_vod_id = vodici.id where vodici.prezime + ', ' + vodici.ime like '" + txtVoditeljDo.Text + "%'))";
                }
                else if (txtVoditelj.Text != "") {
                    filter = filter + " and vodici.prezime + ', ' + vodici.ime like '%" + txtVoditelj.Text + "%'";
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
                    string query = "select min(ispiti.id) from ispiti inner join vodici on ispiti.voditelj_vod_id = vodici.id where 1 = 1 " + filter + " " + ViewState["filter"];
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
                        query = "select min(ispiti.id) from ispiti inner join vodici on ispiti.voditelj_vod_id = vodici.id where 1 = 1 " + filter;
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
    
        protected void btnInstruktoriPotvrdi_Click(object sender, EventArgs e)
        {
            if (actionInstruktori == "Dodaj")
            {
                string query = "insert into ispitiinstruktori (isp_id, vod_id) values (" + hdnID.Value + ", " + ddlInstruktor.SelectedValue + ")";
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
                    txtInstruktor.Text = ddlInstruktor.SelectedItem.ToString();
                    actionInstruktori = "Traži";
                    lblInstruktoriGreska.Text = "";
                }
                catch {
                    lblInstruktoriGreska.Text = "Nije moguće unijeti.";
                }
                con.Close();
                refreshGridInstruktori();
            }
            if (actionInstruktori == "Izmijeni")
            {
                string query = "update ispitiinstruktori set vod_id = " + ddlInstruktor.SelectedValue + " where id = " + hdnInstruktoriID.Value;
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
                    txtInstruktor.Text = ddlInstruktor.SelectedItem.ToString();
                    actionInstruktori = "Traži";
                    lblGreska.Text = "";
                }
                catch {
                    lblGreska.Text = "Nije moguće unijeti.";
                }
                con.Close();
                refreshGridInstruktori();
            }
            if (actionInstruktori == "Obriši")
            {
                string query = "delete from ispitiinstruktori where id = " + hdnInstruktoriID.Value;
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
                    hdnInstruktoriID.Value = "";
                    lblInstruktoriGreska.Text = "";
                }
                catch {
                    lblInstruktoriGreska.Text = "Nije moguće obrisati.";
                }
                con.Close();
                refreshGridInstruktori();
            }
            if ((actionInstruktori == "Filtriraj") || (actionInstruktori == "Traži")) {
                string filter = "";
                if (txtInstruktor.Text != "" && txtInstruktorDo.Text != "") {
                    filter = filter + " and vodici.prezime + ', ' + vodici.ime >= '" + txtInstruktor.Text + "' and (vodici.prezime + ', ' + vodici.ime <= '" + txtInstruktorDo.Text + "' or vodici.prezime + ', ' + vodici.ime <= (select max(vodici.prezime + ', ' + vodici.ime) from ispitiinstruktori inner join vodici on ispitiinstruktori.vod_id = vodici.id where ispitiinstruktori.isp_id = " + hdnID.Value + " and vodici.prezime + ', ' + vodici.ime like '" + txtInstruktorDo.Text + "%'))";
                }
                else if (txtInstruktor.Text != "") {
                    filter = filter + " and vodici.prezime + ', ' + vodici.ime like '%" + txtInstruktor.Text + "%'";
                }
                if (actionInstruktori == "Filtriraj")
                {
                    ViewState["filterinstruktori"] = filter;
                    hdnInstruktoriID.Value = "";                    
                    pageCurrentInstruktori = 0;
                    ViewState["pageCurrentInstruktori"] = pageCurrentInstruktori;
                    refreshGridInstruktori();
                }
                if (actionInstruktori == "Traži")
                {
                    string query = "select ispitiinstruktori.id from ispitiinstruktori inner join vodici on ispitiinstruktori.vod_id = vodici.id where ispitiinstruktori.isp_id = " + hdnID.Value + " and vodici.prezime + ', ' + vodici.ime = (select min(vodici.prezime + ', ' + vodici.ime) from ispitiinstruktori inner join vodici on ispitiinstruktori.vod_id = vodici.id where ispitiinstruktori.isp_id = " + hdnID.Value + " " + filter + " " + ViewState["filterinstruktori"] + ")";
                    OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                    OleDbCommand cmd = new OleDbCommand();     
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    OleDbDataReader dr = cmd.ExecuteReader();
                    if (dr.Read()) {
                        hdnInstruktoriID.Value = (dr[0] as int?).ToString();
                        refreshGridInstruktori();
                    }
                    else {
                        query = "select ispitiinstruktori.id from ispitiinstruktori inner join vodici on ispitiinstruktori.vod_id = vodici.id where ispitiinstruktori.isp_id = " + hdnID.Value + " and vodici.prezime + ', ' + vodici.ime = (select min(vodici.prezime + ', ' + vodici.ime) from ispitiinstruktori inner join vodici on ispitiinstruktori.vod_id = vodici.id where ispitiinstruktori.isp_id = " + hdnID.Value + " " + filter + ")";
                        cmd.CommandText = query;
                        dr.Close();
                        dr = cmd.ExecuteReader();
                        if (dr.Read()) {
                            hdnInstruktoriID.Value = (dr[0] as int?).ToString();
                            ViewState["filterinstruktori"] = "";
                            refreshGridInstruktori();
                        }
                        else {
                            hdnInstruktoriID.Value = "";
                        }
                    }
                    con.Close();                
                }
            }
            if (lblGreska.Text == "") {
                actionInstruktori = "";
            }
            ViewState["actionInstruktori"] = actionInstruktori;
        }

        protected void btnKandidatiPotvrdi_Click(object sender, EventArgs e)
        {
            if (actionKandidati == "Dodaj")
            {
                string query = "insert into ispitikandidati (isp_id, vod_id, polozio) values (" + hdnID.Value + ", " + ddlKandidat.SelectedValue + ", " + cbPolozio.Checked + ")";
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
                    txtKandidat.Text = ddlKandidat.SelectedItem.ToString();
                    actionKandidati = "Traži";
                    lblKandidatiGreska.Text = "";
                }
                catch {
                    lblKandidatiGreska.Text = "Nije moguće unijeti.";
                }
                con.Close();
                refreshGridKandidati();
            }
            if (actionKandidati == "Izmijeni")
            {
                string query = "update ispitikandidati set vod_id = " + ddlKandidat.SelectedValue + ", polozio = " + cbPolozio.Checked + " where id = " + hdnKandidatiID.Value;
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
                    txtKandidat.Text = ddlKandidat.SelectedItem.ToString();
                    actionKandidati = "Traži";
                    lblGreska.Text = "";
                }
                catch {
                    lblGreska.Text = "Nije moguće unijeti.";
                }
                con.Close();
                refreshGridKandidati();
            }
            if (actionKandidati == "Obriši")
            {
                string query = "delete from ispitikandidati where id = " + hdnKandidatiID.Value;
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
                    hdnKandidatiID.Value = "";
                    lblKandidatiGreska.Text = "";
                }
                catch {
                    lblKandidatiGreska.Text = "Nije moguće obrisati.";
                }
                con.Close();
                refreshGridKandidati();
            }
            if ((actionKandidati == "Filtriraj") || (actionKandidati == "Traži")) {
                string filter = "";
                if (txtKandidat.Text != "" && txtKandidatDo.Text != "") {
                    filter = filter + " and vodici.prezime + ', ' + vodici.ime >= '" + txtKandidat.Text + "' and (vodici.prezime + ', ' + vodici.ime <= '" + txtKandidatDo.Text + "' or vodici.prezime + ', ' + vodici.ime <= (select max(vodici.prezime + ', ' + vodici.ime) from ispitikandidati inner join vodici on ispitikandidati.vod_id = vodici.id where ispitikandidati. = " + hdnID.Value + " and vodici.prezime + ', ' + vodici.ime like '" + txtKandidatDo.Text + "%'))";
                }
                else if (txtKandidat.Text != "") {
                    filter = filter + " and vodici.prezime + ', ' + vodici.ime like '%" + txtKandidat.Text + "%'";
                }
                if (actionKandidati == "Filtriraj")
                {
                    ViewState["filterkandidati"] = filter;
                    hdnKandidatiID.Value = "";                    
                    pageCurrentKandidati = 0;
                    ViewState["pageCurrentKandidati"] = pageCurrentKandidati;
                    refreshGridKandidati();
                }
                if (actionKandidati == "Traži")
                {
                    string query = "select ispitikandidati.id from ispitikandidati inner join vodici on ispitikandidati.vod_id = vodici.id where ispitikandidati.isp_id = " + hdnID.Value + " and vodici.prezime + ', ' + vodici.ime = (select min(vodici.prezime + ', ' + vodici.ime) from ispitikandidati inner join vodici on ispitikandidati.vod_id = vodici.id where ispitikandidati.isp_id = " + hdnID.Value + " " + filter + " " + ViewState["filterkandidati"] + ")";
                    OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                    OleDbCommand cmd = new OleDbCommand();     
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    OleDbDataReader dr = cmd.ExecuteReader();
                    if (dr.Read()) {
                        hdnKandidatiID.Value = (dr[0] as int?).ToString();
                        refreshGridKandidati();
                    }
                    else {
                        query = "select ispitikandidati.id from ispitikandidati inner join vodici on ispitikandidati.vod_id = vodici.id where ispitikandidati.isp_id = " + hdnID.Value + " and vodici.prezime + ', ' + vodici.ime = (select min(vodici.prezime + ', ' + vodici.ime) from ispitikandidati inner join vodici on ispitikandidati.vod_id = vodici.id where ispitikandidati.isp_id = " + hdnID.Value + " " + filter + ")";
                        cmd.CommandText = query;
                        dr.Close();
                        dr = cmd.ExecuteReader();
                        if (dr.Read()) {
                            hdnKandidatiID.Value = (dr[0] as int?).ToString();
                            ViewState["filterkandidati"] = "";
                            refreshGridKandidati();
                        }
                        else {
                            hdnKandidatiID.Value = "";
                        }
                    }
                    con.Close();                
                }
            }
            if (lblGreska.Text == "") {
                actionKandidati = "";
            }
            ViewState["actionKandidati"] = actionKandidati;
        }

        protected void btnOdustani_Click(object sender, EventArgs e)
        {
            lblGreska.Text = "";
            action = "";
            ViewState["action"] = action;
        }

        protected void btnInstruktoriZatvori_Click(object sender, EventArgs e)
        {
            lblInstruktoriGreska.Text = "";
            action = "";
            hdnInstruktoriID.Value = "";
            ViewState["action"] = action;
            actionInstruktori = "";
            ViewState["actionInstruktori"] = actionInstruktori;
        }

        protected void btnKandidatiZatvori_Click(object sender, EventArgs e)
        {
            lblKandidatiGreska.Text = "";
            action = "";
            hdnKandidatiID.Value = "";
            ViewState["action"] = action;
            actionKandidati = "";
            ViewState["actionKandidati"] = actionKandidati;
        }
        
        protected void btnInstruktoriOdustani_Click(object sender, EventArgs e)
        {
            lblInstruktoriGreska.Text = "";
            actionInstruktori = "";
            ViewState["actionInstruktori"] = actionInstruktori;
        }

        protected void btnKandidatiOdustani_Click(object sender, EventArgs e)
        {
            lblKandidatiGreska.Text = "";
            actionKandidati = "";
            ViewState["actionKandidati"] = actionKandidati;
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

        protected void btnInstruktoriIduca_Click(object sender, EventArgs e)
        {
            if (pageCurrentInstruktori < pageCountInstruktori - 1) {
                pageCurrentInstruktori = pageCurrentInstruktori + 1;
                ViewState["pageCurrentInstruktori"] = pageCurrentInstruktori;
                refreshGridInstruktori();
            }
        }

        protected void btnInstruktoriPrethodna_Click(object sender, EventArgs e)
        {
            if (pageCurrentInstruktori > 0) {
                pageCurrentInstruktori = pageCurrentInstruktori - 1;
                ViewState["pageCurrentInstruktori"] = pageCurrentInstruktori;
                refreshGridInstruktori();
            }
        }

        protected void btnInstruktoriPrva_Click(object sender, EventArgs e)
        {
            if (pageCurrentInstruktori != 0) {
                pageCurrentInstruktori = 0;
                ViewState["pageCurrentInstruktori"] = pageCurrentInstruktori;
                refreshGridInstruktori();
            }
        }

        protected void btnInstruktoriZadnja_Click(object sender, EventArgs e)
        {
            if (pageCurrentInstruktori != pageCountInstruktori - 1) {
                pageCurrentInstruktori = pageCountInstruktori - 1;
                ViewState["pageCurrentInstruktori"] = pageCurrentInstruktori;
                refreshGridInstruktori();
            }
        }

        protected void btnKandidatiIduca_Click(object sender, EventArgs e)
        {
            if (pageCurrentKandidati < pageCountKandidati - 1) {
                pageCurrentKandidati = pageCurrentKandidati + 1;
                ViewState["pageCurrentKandidati"] = pageCurrentKandidati;
                refreshGridKandidati();
            }
        }

        protected void btnKandidatiPrethodna_Click(object sender, EventArgs e)
        {
            if (pageCurrentKandidati > 0) {
                pageCurrentKandidati = pageCurrentKandidati - 1;
                ViewState["pageCurrentKandidati"] = pageCurrentKandidati;
                refreshGridKandidati();
            }
        }

        protected void btnKandidatiPrva_Click(object sender, EventArgs e)
        {
            if (pageCurrentKandidati != 0) {
                pageCurrentKandidati = 0;
                ViewState["pageCurrentKandidati"] = pageCurrentKandidati;
                refreshGridKandidati();
            }
        }

        protected void btnKandidatiZadnja_Click(object sender, EventArgs e)
        {
            if (pageCurrentKandidati != pageCountKandidati - 1) {
                pageCurrentKandidati = pageCountKandidati - 1;
                ViewState["pageCurrentKandidati"] = pageCurrentKandidati;
                refreshGridKandidati();
            }
        }

    }
}