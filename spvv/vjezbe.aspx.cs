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
    public partial class wfVjezbe:System.Web.UI.Page {

        public string action = "";
        public string actionInstruktori = "";
        public string actionVodici = "";
        public int pageCurrent;
        public int pageSize = 10;
        public int pageCount;
        public int pageCurrentInstruktori;
        public int pageSizeInstruktori = 10;
        public int pageCountInstruktori;
        public int pageCurrentVodici;
        public int pageSizeVodici = 10;
        public int pageCountVodici;

        protected void Page_Load(object sender,EventArgs e) {        
            if (Session["korisnik"] == null || Session["korisnik"].ToString().Length == 0) {
                Response.Redirect("login.aspx");
            }

            if (!IsPostBack) {
                ViewState["pageCurrent"] = 0;
                refreshGrid();
            
                string query = "select id, prezime + ', ' + ime as instruktor from vodici union select null, null from dual order by instruktor";
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);                
                ddlInstruktor.DataSource = dt;
                ddlInstruktor.DataTextField = "instruktor";
                ddlInstruktor.DataValueField = "id";
                ddlInstruktor.DataBind();
                ddlVoditelj.DataSource = dt;
                ddlVoditelj.DataTextField = "instruktor";
                ddlVoditelj.DataValueField = "id";
                ddlVoditelj.DataBind();
                query = "select id, prezime + ', ' + ime as vodic from vodici union select null, null from dual order by vodic";
                cmd.CommandText = query;
                dr = cmd.ExecuteReader();
                dt = new DataTable();
                dt.Load(dr);
                ddlVodic.DataSource = dt;
                ddlVodic.DataTextField = "vodic";
                ddlVodic.DataValueField = "id";
                ddlVodic.DataBind();                
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
                actionVodici = (String)ViewState["actionVodici"];
            }
            pageCurrent = Convert.ToInt32(ViewState["pageCurrent"]);
            pageCount = Convert.ToInt32(ViewState["pageCount"]);
            pageCurrentInstruktori = Convert.ToInt32(ViewState["pageCurrentInstruktori"]);
            pageCountInstruktori = Convert.ToInt32(ViewState["pageCountInstruktori"]);
            pageCurrentVodici = Convert.ToInt32(ViewState["pageCurrentVodici"]);
            pageCountVodici = Convert.ToInt32(ViewState["pageCountVodici"]);
        }

        protected void refreshGrid()
        { 
            OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
            OleDbCommand cmd = new OleDbCommand();     
            string query = "select count(vjezbe.id) from (vjezbe inner join standardi on vjezbe.std_id = standardi.id) left join vodici on vjezbe.voditelj_vod_id = vodici.id where 1 = 1 " + ViewState["filter"];
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
            query = "select vjezbe.id, vjezbe.naziv, vjezbe.datum, standardi.naziv as standard, vodici.prezime + ', ' + vodici.ime as voditelj from (vjezbe inner join standardi on vjezbe.std_id = standardi.id) left join vodici on vjezbe.voditelj_vod_id = vodici.id where 1 = 1 " + ViewState["filter"] + " order by vjezbe.datum desc";
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
            query = "select * from (select top " + pageSize.ToString() + " * from (select top " + (pageSize * (pageCurrent + 1)).ToString() + " * from (" + query + ")) order by vjezbe.datum) order by vjezbe.datum desc";
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
            string query = "select count(vjezbeinstruktori.id) from vjezbeinstruktori left join vodici on vjezbeinstruktori.vod_id = vodici.id where vjezbeinstruktori.vje_id = " + hdnID.Value + " " + ViewState["filterinstruktori"];
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
            query = "select vjezbeinstruktori.id, vodici.prezime + ', ' + vodici.ime as instruktor from vjezbeinstruktori left join vodici on vjezbeinstruktori.vod_id = vodici.id where vjezbeinstruktori.vje_id = " + hdnID.Value + " " + ViewState["filterinstruktori"] + " order by vodici.prezime, vodici.ime";
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
        }

        protected void refreshGridVodici()
        { 
            OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
            OleDbCommand cmd = new OleDbCommand();     
            string query = "select vjezbevodici.id, vodici.prezime + ', ' + vodici.ime as vodic from vjezbevodici left join vodici on vjezbevodici.vod_id = vodici.id where vjezbevodici.vje_id = " + hdnID.Value + " " + ViewState["filtervodici"];
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            con.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            if (dr.Read()) {
                pageCountVodici = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dr[0] as int?) / Convert.ToDouble(pageSize)));            
            }
            else {
                pageCountVodici = 0;
            }
            ViewState["pageCountVodici"] = pageCountVodici;
            query = "select vjezbevodici.id, vodici.prezime + ', ' + vodici.ime as vodic from vjezbevodici left join vodici on vjezbevodici.vod_id = vodici.id where vjezbevodici.vje_id = " + hdnID.Value + " " + ViewState["filtervodici"] + " order by vodici.prezime, vodici.ime";
            if (actionVodici == "Traži") {                
                cmd.CommandText = query;
                dr.Close();
                dr = cmd.ExecuteReader();
                int position = -1;
                while (dr.Read()) {
                    position++;
                    if (dr["id"].ToString() == hdnVodiciID.Value) {
                        pageCurrentVodici = Convert.ToInt32(Math.Floor(Convert.ToDouble(position) / Convert.ToDouble(pageSize)));
                        ViewState["pageCurrentVodici"] = pageCurrentVodici;
                    }
                }
            }            
            query = "select * from (select top " + pageSizeVodici.ToString() + " * from (select top " + (pageSizeVodici * (pageCurrentVodici + 1)).ToString() + " * from (" + query + ")) order by vodic desc) order by vodic";
            cmd.CommandText = query;
            dr.Close();
            dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);            
            rptVodici.DataSource = dt;    
            rptVodici.DataBind();
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
                string query = "select naziv, datum, std_id, voditelj_vod_id from vjezbe where id = " + hdnID.Value;
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
                hdnVodiciID.Value = "";
                refreshGridInstruktori();
                actionVodici = "";
                ViewState["actionVodici"] = actionVodici;
            }
            if (action == "Vodiči" && hdnID.Value != "") {
                ViewState["filtervodici"] = "";
                hdnInstruktoriID.Value = "";
                hdnVodiciID.Value = "";
                refreshGridVodici();
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
                string query = "select vod_id from vjezbeinstruktori where id = " + hdnInstruktoriID.Value;
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

        protected void btnVodiciCommand_Click(object sender, EventArgs e)
        {
            HtmlButton hb = (HtmlButton)sender;
            actionVodici = hb.InnerText.Substring(hb.InnerText.IndexOf(">") + 1);
            if (Convert.ToInt32(Session["nivo"].ToString()) > 1 && (actionVodici == "Dodaj" || actionVodici == "Izmijeni" || actionVodici == "Obriši")) {
                actionVodici = "";
            }
            ViewState["actionVodici"] = actionVodici;
            if (actionVodici == "Izmijeni" && hdnID.Value != "") {
                string query = "select vod_id from vjezbevodici where id = " + hdnVodiciID.Value;
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                if (dr.Read()) {
                    ddlVodic.SelectedValue = (dr[0] as int?).ToString();
                }
                con.Close();
            }
            if (actionVodici == "Dodaj" || actionVodici == "Izmijeni") {
                ddlVodic.Focus();
            }
            if (actionVodici == "Traži" || actionVodici == "Filtriraj") {
                txtVodic.Focus();
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

            WordprocessingDocument package = WordprocessingDocument.Create(HttpRuntime.AppDomainAppPath + "\\vjezbe.docx", WordprocessingDocumentType.Document);
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

            string query = "select vjezbe.naziv, vjezbe.datum, standardi.naziv as naziv, vodici.prezime + ' ' + vodici.ime as voditelj from (vjezbe inner join standardi on vjezbe.std_id = standardi.id) left join vodici on vjezbe.voditelj_vod_id = vodici.id where 1 = 1 " + ViewState["filter"] + " order by vjezbe.datum desc";
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
            Response.AppendHeader("Content-Disposition", "attachment; filename=vjezbe.docx");
            Response.TransmitFile(HttpRuntime.AppDomainAppPath + "\\vjezbe.docx");
            Response.End();
        }

        protected void btnInstruktoriIspisi_Click(object sender, EventArgs e)
        {
        }

        protected void btnVodiciIspisi_Click(object sender, EventArgs e)
        {
        }

        protected void btnPotvrdi_Click(object sender, EventArgs e)
        {
            if (action == "Dodaj")
            {
                string query = "insert into vjezbe (naziv, datum, std_id, voditelj_vod_id) values ('" + txtNaziv.Text + "', '" + txtDatum.Text + "', " + ddlStandard.SelectedValue + ", " + ddlVoditelj.SelectedValue + ")";
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
                string query = "update vjezbe set naziv = '" + txtNaziv.Text + "', datum = '" + txtDatum.Text + "', std_id = " + ddlStandard.SelectedValue + ", voditelj_vod_id = " + ddlVoditelj.SelectedValue + " where id = " + hdnID.Value;
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
                string query = "delete from vjezbe where id = " + hdnID.Value;
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
            if (action == "Filtriraj" || action == "Traži") {
                string filter = "";
                if (txtNaziv.Text != "" && txtNazivDo.Text != "") {
                    filter = " and vjezbe.naziv >= '" + txtNaziv.Text + "' and (vjezbe.naziv <= '" + txtNazivDo.Text + "' or vjezbe.naziv <= (select max(naziv) from vjezbe where naziv like '" + txtNazivDo.Text + "%')";
                }
                else if (txtNaziv.Text != "") {
                    filter = " and vjezbe.naziv like '%" + txtNaziv.Text + "%'";
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
                    filter = filter + " and vjezbe.datum >= " + d1 + " and vjezbe.datum <= " + d2;
                }
                else if (d1 != "") {
                    filter = filter + " and vjezbe.datum = " + d1;
                }                
                if (txtStandard.Text != "" && txtStandardDo.Text != "") {
                    filter = filter + " and standardi.naziv >= '" + txtStandard.Text + "' and (standardi.naziv <= '" + txtStandardDo.Text + "' or standardi.naziv <= (select max(standardi.naziv) from vjezbe inner join standardi on vjezbe.std_id = standardi.id where standardi.naziv like '" + txtStandardDo.Text + "%'))";
                }
                else if (txtStandard.Text != "") {
                    filter = filter + " and standardi.naziv like '%" + txtStandard.Text + "%'";
                }
                if (txtVoditelj.Text != "" && txtVoditeljDo.Text != "") {
                    filter = filter + " and vodici.prezime + ', ' + vodici.ime >= '" + txtVoditelj.Text + "' and (vodici.prezime + ', ' + vodici.ime <= '" + txtVoditeljDo.Text + "' or vodici.prezime + ', ' + vodici.ime <= (select max(vodici.prezime + ', ' + vodici.ime) from vjezbe inner join vodici on vjezbe.voditelj_vod_id = vodici.id where vodici.prezime + ', ' + vodici.ime like '" + txtVoditeljDo.Text + "%'))";
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
                    string query = "select id from vjezbe where naziv = (select min(vjezbe.naziv) from vjezbe inner join vodici on vjezbe.voditelj_vod_id = vodici.id where 1 = 1 " + filter + " " + ViewState["filter"] + ")";
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
                        query = "select id from vjezbe where naziv = (select min(vjezbe.naziv) from vjezbe inner join vodici on vjezbe.voditelj_vod_id = vodici.id where 1 = 1 " + filter + ")";
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
                string query = "insert into vjezbeinstruktori (vje_id, vod_id) values (" + hdnID.Value + ", " + ddlInstruktor.SelectedValue + ")";
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
                string query = "update vjezbeinstruktori set vod_id = " + ddlInstruktor.SelectedValue + " where id = " + hdnInstruktoriID.Value;
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
            if (actionInstruktori == "Obriši")
            {
                string query = "delete from vjezbeinstruktori where id = " + hdnInstruktoriID.Value;
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
                    filter = filter + " and vodici.prezime + ', ' + vodici.ime >= '" + txtInstruktor.Text + "' and (vodici.prezime + ', ' + vodici.ime <= '" + txtInstruktorDo.Text + "' or vodici.prezime + ', ' + vodici.ime <= (select max(vodici.prezime + ', ' + vodici.ime) from vjezbeinstruktori inner join vodici on vjezbeinstruktori.vod_id = vodici.id where vje_id = " + hdnID.Value + " and vodici.prezime + ', ' + vodici.ime like '" + txtInstruktorDo.Text + "%'))";
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
                    string query = "select vjezbeinstruktori.id from vjezbeinstruktori inner join vodici on vjezbeinstruktori.vod_id = vodici.id where vjezbeinstruktori.vje_id = " + hdnID.Value + " and vodici.prezime + ', ' + vodici.ime = (select min(vodici.prezime + ', ' + vodici.ime) from vjezbeinstruktori inner join vodici on vjezbeinstruktori.vod_id = vodici.id where vjezbeinstruktori.vje_id = " + hdnID.Value + filter + " " + ViewState["filterinstruktori"] + ")";
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
                        query = "select vjezbeinstruktori.id from vjezbeinstruktori inner join vodici on vjezbeinstruktori.vod_id = vodici.id where vjezbeinstruktori.vje_id = " + hdnID.Value + " and vodici.prezime + ', ' + vodici.ime = (select min(vodici.prezime + ', ' + vodici.ime) from vjezbeinstruktori inner join vodici on vjezbeinstruktori.vod_id = vodici.id where vjezbeinstruktori.vje_id = " + hdnID.Value + " " + filter + ")";
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
            if (lblInstruktoriGreska.Text == "") {
                actionInstruktori = "";
            }
            ViewState["actionInstruktori"] = actionInstruktori;
        }

        protected void btnVodiciPotvrdi_Click(object sender, EventArgs e)
        {
            if (actionVodici == "Dodaj")
            {
                string query = "insert into vjezbevodici (vje_id, vod_id) values (" + hdnID.Value + ", " + ddlVodic.SelectedValue + ")";
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
                    txtVodic.Text = ddlVodic.SelectedItem.ToString();
                    actionVodici = "Traži";
                    lblVodiciGreska.Text = "";
                }
                catch {
                    lblVodiciGreska.Text = "Nije moguće unijeti.";
                }
                con.Close();
                refreshGridVodici();
            }
            if (actionVodici == "Izmijeni")
            {
                string query = "update vjezbevodici set vod_id = " + ddlVodic.SelectedValue + " where id = " + hdnVodiciID.Value;
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
                    txtVodic.Text = ddlVodic.SelectedItem.ToString();
                    actionVodici = "Traži";
                    lblVodiciGreska.Text = "";
                }
                catch {
                    lblVodiciGreska.Text = "Nije moguće unijeti.";
                }
                con.Close();
                refreshGridVodici();
            }
            if (actionVodici == "Obriši")
            {
                string query = "delete from vjezbevodici where id = " + hdnVodiciID.Value;
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
                    hdnVodiciID.Value = "";
                    lblVodiciGreska.Text = "";
                }
                catch {
                    lblVodiciGreska.Text = "Nije moguće obrisati.";
                }
                con.Close();
                refreshGridVodici();
            }
            if ((actionVodici == "Filtriraj") || (actionVodici == "Traži")) {
                string filter = "";
                if (txtVodic.Text != "" && txtVodicDo.Text != "") {
                    filter = filter + " and vodici.prezime + ', ' + vodici.ime >= '" + txtVodic.Text + "' and (vodici.prezime + ', ' + vodici.ime <= '" + txtVodicDo.Text + "' or vodici.prezime + ', ' + vodici.ime <= (select max(vodici.prezime + ', ' + vodici.ime) from vjezbevodici inner join vodici on vjezbevodici.vod_id = vodici.id where vje_id = " + hdnID.Value + " and vodici.prezime + ', ' + vodici.ime like '" + txtVodicDo.Text + "%'))";
                }
                else if (txtVodic.Text != "") {
                    filter = filter + " and vodici.prezime + ', ' + vodici.ime like '%" + txtVodic.Text + "%'";
                }
                if (actionVodici == "Filtriraj")
                {
                    ViewState["filtervodici"] = filter;
                    hdnVodiciID.Value = "";                    
                    pageCurrentVodici = 0;
                    ViewState["pageCurrentVodici"] = pageCurrentVodici;
                    refreshGridVodici();
                }
                if (actionVodici == "Traži")
                {
                    string query = "select vjezbevodici.id from vjezbevodici inner join vodici on vjezbevodici.vod_id = vodici.id where vjezbevodici.vje_id = " + hdnID.Value + " and vodici.prezime + ', ' + vodici.ime = (select min(vodici.prezime + ', ' + vodici.ime) from vjezbevodici inner join vodici on vjezbevodici.vod_id = vodici.id where vjezbevodici.vje_id = " + hdnID.Value + " " + filter + " " + ViewState["filterkandidati"] + ")";
                    OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                    OleDbCommand cmd = new OleDbCommand();     
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    OleDbDataReader dr = cmd.ExecuteReader();
                    if (dr.Read()) {
                        hdnVodiciID.Value = (dr[0] as int?).ToString();
                        refreshGridVodici();
                    }
                    else {
                        query = "select vjezbevodici.id from vjezbevodici inner join vodici on vjezbevodici.vod_id = vodici.id where vjezbevodici.vje_id = " + hdnID.Value + " and vodici.prezime + ', ' + vodici.ime = (select min(vodici.prezime + ', ' + vodici.ime) from vjezbevodici inner join vodici on vjezbevodici.vod_id = vodici.id where vjezbevodici.vje_id = " + hdnID.Value + " " + filter + ")";
                        cmd.CommandText = query;
                        dr.Close();
                        dr = cmd.ExecuteReader();
                        if (dr.Read()) {
                            hdnVodiciID.Value = (dr[0] as int?).ToString();
                            ViewState["filtervodici"] = "";
                            refreshGridVodici();
                        }
                        else {
                            hdnVodiciID.Value = "";
                        }
                    }
                    con.Close();                
                }
            }
            if (lblVodiciGreska.Text == "") {
                actionVodici = "";
            }
            ViewState["actionVodici"] = actionVodici;
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

        protected void btnVodiciZatvori_Click(object sender, EventArgs e)
        {
            lblVodiciGreska.Text = "";
            action = "";
            hdnVodiciID.Value = "";
            ViewState["action"] = action;
            actionVodici = "";
            ViewState["actionVodici"] = actionVodici;
        }

        protected void btnInstruktoriOdustani_Click(object sender, EventArgs e)
        {
            lblInstruktoriGreska.Text = "";
            actionInstruktori = "";
            ViewState["actionInstruktori"] = actionInstruktori;
        }

        protected void btnVodiciOdustani_Click(object sender, EventArgs e)
        {
            lblVodiciGreska.Text = "";
            actionVodici = "";
            ViewState["actionVodici"] = actionVodici;
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

        protected void btnVodiciIduca_Click(object sender, EventArgs e)
        {
            if (pageCurrentVodici < pageCountVodici - 1) {
                pageCurrentVodici = pageCurrentVodici + 1;
                ViewState["pageCurrentVodici"] = pageCurrentVodici;
                refreshGridVodici();
            }
        }

        protected void btnVodiciPrethodna_Click(object sender, EventArgs e)
        {
            if (pageCurrentVodici > 0) {
                pageCurrentVodici = pageCurrentVodici - 1;
                ViewState["pageCurrentVodici"] = pageCurrentVodici;
                refreshGridVodici();
            }
        }

        protected void btnVodiciPrva_Click(object sender, EventArgs e)
        {
            if (pageCurrentVodici != 0) {
                pageCurrentVodici = 0;
                ViewState["pageCurrentVodici"] = pageCurrentVodici;
                refreshGridVodici();
            }
        }

        protected void btnVodiciZadnja_Click(object sender, EventArgs e)
        {
            if (pageCurrentVodici != pageCountVodici - 1) {
                pageCurrentVodici = pageCountVodici - 1;
                ViewState["pageCurrentVodici"] = pageCurrentVodici;
                refreshGridVodici();
            }
        }

    }
}