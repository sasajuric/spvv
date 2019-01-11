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
    public partial class wfIzleti:System.Web.UI.Page {

        public string action = "";
        public int pageCurrent;
        public int pageSize = 10;
        public int pageCount;

        protected string nulld(string ns) {
            string rez;
            if (ns == null || ns == "") {
                rez = "null";
            }
            else {
                rez = ns;
            }
            return rez;
        }

        protected string nulls(string ns) {
            string rez;
            if (ns == null || ns == "") {
                rez = "null";
            }
            else {
                rez = "'" + ns + "'";
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

                string query = "select id, prezime + ', ' + ime as glavnivodic from vodici union select null, null from dual order by glavnivodic";
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);                
                ddlGlavniVodic.DataSource = dt;
                ddlGlavniVodic.DataTextField = "glavnivodic";
                ddlGlavniVodic.DataValueField = "id";
                ddlGlavniVodic.DataBind();
                query = "select id, prezime + ', ' + ime as pomocnivodic from vodici union select null, null from dual order by pomocnivodic";
                cmd.CommandText = query;
                dr = cmd.ExecuteReader();
                dt = new DataTable();
                dt.Load(dr);
                ddlPomocniVodic.DataSource = dt;
                ddlPomocniVodic.DataTextField = "pomocnivodic";
                ddlPomocniVodic.DataValueField = "id";
                ddlPomocniVodic.DataBind();                
                query = "select id, naziv from udruge union select null, null from dual order by naziv";
                cmd.CommandText = query;
                dr = cmd.ExecuteReader();
                dt = new DataTable();
                dt.Load(dr);
                ddlUdruga.DataSource = dt;
                ddlUdruga.DataTextField = "naziv";
                ddlUdruga.DataValueField = "id";
                ddlUdruga.DataBind();
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
            string query = query = "select count(izleti.id) from ((izleti left join vodici as vodicig on izleti.glavni_vod_id = vodicig.id) left join vodici as vodicip on izleti.pomocni_vod_id = vodicip.id) left join udruge on izleti.udr_id = udruge.id where 1 = 1 " + ViewState["filter"];
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
            query = "select izleti.id, izleti.datum, izleti.naziv, vodicig.prezime + ', ' + vodicig.ime as glavnivodic, vodicip.prezime + ', ' + vodicip.ime as pomocnivodic, udruge.naziv as udruga, izleti.odrediste, izleti.trajanje, izleti.brojsudionika from ((izleti left join vodici as vodicig on izleti.glavni_vod_id = vodicig.id) left join vodici as vodicip on izleti.pomocni_vod_id = vodicip.id) left join udruge on izleti.udr_id = udruge.id where 1 = 1 " + ViewState["filter"] + " order by izleti.datum desc";
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
            query = "select * from (select top " + pageSize.ToString() + " * from (select top " + (pageSize * (pageCurrent + 1)).ToString() + " * from (" + query + ")) order by izleti.datum) order by izleti.datum desc";
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
                string query = "select datum, naziv, glavni_vod_id, pomocni_vod_id, udr_id, odrediste, trajanje, brojsudionika from izleti where id = " + hdnID.Value;
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                if (dr.Read()) {
                    if (dr["datum"].ToString() != "") {
                        txtDatum.Text = Convert.ToDateTime(dr["datum"]).ToString("dd.MM.yyyy.");
                    }
                    txtNaziv.Text = dr["naziv"].ToString();
                    ddlGlavniVodic.SelectedValue = (dr["glavni_vod_id"] as int?).ToString();
                    ddlPomocniVodic.SelectedValue = (dr["pomocni_vod_id"] as int?).ToString();
                    ddlUdruga.SelectedValue = (dr["udr_id"] as int?).ToString();
                    txtOdrediste.Text = dr["odrediste"].ToString();
                    txtTrajanje.Text = dr["trajanje"].ToString();
                    txtBrojSudionika.Text = dr["brojsudionika"].ToString();
                }
                con.Close();
            }
            if ((action == "Dodaj") || (action == "Izmijeni") || (action == "Traži") || (action == "Filtriraj")) {
                txtDatum.Focus();
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

            WordprocessingDocument package = WordprocessingDocument.Create(HttpRuntime.AppDomainAppPath + "\\izleti.docx", WordprocessingDocumentType.Document);
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
            run.AppendChild(new Text("Izleti"));

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
            run.Append(new Text("Glavni vodič"));
            // ćelija
            tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
            tcp = tc.AppendChild(new TableCellProperties());
            tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
            par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
            run = par.AppendChild(new Run());
            rp = run.AppendChild(new RunProperties());
            rp.Append(new Color() { Val = "ffffff" });
            rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
            run.Append(new Text("Pomoćni vodič"));
            // ćelija
            tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
            tcp = tc.AppendChild(new TableCellProperties());
            tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
            par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
            run = par.AppendChild(new Run());
            rp = run.AppendChild(new RunProperties());
            rp.Append(new Color() { Val = "ffffff" });
            rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
            run.Append(new Text("Udruga"));
            // ćelija
            tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
            tcp = tc.AppendChild(new TableCellProperties());
            tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
            par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
            run = par.AppendChild(new Run());
            rp = run.AppendChild(new RunProperties());
            rp.Append(new Color() { Val = "ffffff" });
            rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
            run.Append(new Text("Odredište"));
            // ćelija
            tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
            tcp = tc.AppendChild(new TableCellProperties());
            tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
            par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
            run = par.AppendChild(new Run());
            rp = run.AppendChild(new RunProperties());
            rp.Append(new Color() { Val = "ffffff" });
            rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
            run.Append(new Text("Trajanje"));
            // ćelija
            tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
            tcp = tc.AppendChild(new TableCellProperties());
            tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
            par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
            run = par.AppendChild(new Run());
            rp = run.AppendChild(new RunProperties());
            rp.Append(new Color() { Val = "ffffff" });
            rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
            run.Append(new Text("Broj sudionika"));

            string query = "select izleti.datum, izleti.naziv, vodicig.prezime + ', ' + vodicig.ime as glavnivodic, vodicip.prezime + ', ' + vodicip.ime as pomocnivodic, udruge.naziv as udruga, izleti.odrediste, izleti.trajanje, izleti.brojsudionika from ((izleti left join vodici as vodicig on izleti.glavni_vod_id = vodicig.id) left join vodici as vodicip on izleti.pomocni_vod_id = vodicip.id) left join udruge on izleti.udr_id = udruge.id where 1 = 1 " + ViewState["filter"] + " order by datum desc";
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
                run.Append(new Text(Convert.ToDateTime(dr["datum"]).ToString("dd.MM.yyyy.")));

                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                run.Append(new Text(dr["naziv"] as string));                

                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                run.Append(new Text(dr["glavnivodic"] as string));                

                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                run.Append(new Text(dr["pomocnivodic"] as string));                

                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                run.Append(new Text(dr["udruga"] as string));                

                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                run.Append(new Text(dr["odrediste"] as string));                

                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                run.Append(new Text(dr["trajanje"].ToString()));

                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                run.Append(new Text(dr["brojsudionika"].ToString()));

                table.Append(tr);
            }
            document.Body.Append(table);
            document.Save();
            package.Close();

            con.Close();

            Response.ContentType = "Application/docx";
            Response.AppendHeader("Content-Disposition", "attachment; filename=izleti.docx");
            Response.TransmitFile(HttpRuntime.AppDomainAppPath + "\\izleti.docx");
            Response.End();
        }

        protected void btnPotvrdi_Click(object sender, EventArgs e)
        {
            if (action == "Dodaj")
            {
                string query = "insert into izleti (datum, naziv, glavni_vod_id, pomocni_vod_id, udr_id, odrediste, trajanje, brojsudionika) values (" +
                    nulls(txtDatum.Text) + ", " +
                    nulls(txtNaziv.Text) + ", " +
                    nulld(ddlGlavniVodic.SelectedValue) + ", " +
                    nulld(ddlPomocniVodic.SelectedValue) + ", " +
                    nulld(ddlUdruga.SelectedValue) + ", " +
                    nulls(txtOdrediste.Text) + ", " +
                    nulls(txtTrajanje.Text) + ", " +
                    nulls(txtBrojSudionika.Text) +
                    ")";
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
                string query = "update izleti set " + 
                    "datum = " + nulls(txtDatum.Text) + ", " +
                    "naziv = " + nulls(txtNaziv.Text) + ", " +
                    "glavni_vod_id = " + nulld(ddlGlavniVodic.SelectedValue) + ", " +
                    "pomocni_vod_id = " + nulld(ddlPomocniVodic.SelectedValue) + ", " +
                    "udr_id = " + nulld(ddlUdruga.SelectedValue) + ", " +
                    "odrediste = " + nulls(txtOdrediste.Text) + ", " +
                    "trajanje = " + nulld(txtTrajanje.Text) + ", " +
                    "brojsudionika = " + nulld(txtBrojSudionika.Text) + " " +
                    "where id = " + hdnID.Value;
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
                string query = "delete from izleti where id = " + hdnID.Value;
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
                    filter = " and izleti.datum >= " + d1 + " and izleti.datum <= " + d2;
                }
                else if (d1 != "") {
                    filter = " and izleti.datum = " + d1;
                }
                if (txtNaziv.Text != "" && txtNazivDo.Text != "") {
                    filter = filter + " and naziv >= '" + txtNaziv.Text + "' and (naziv <= '" + txtNazivDo.Text + "' or naziv <= (select max(naziv) from izleti where naziv like '" + txtNazivDo.Text + "%'))";
                }
                else if (txtNaziv.Text != "") {
                    filter = filter + " and naziv like '%" + txtNaziv.Text + "%'";
                }
                if (txtGlavniVodic.Text != "" && txtGlavniVodicDo.Text != "") {
                    filter = filter + " and vodicig.prezime + ', ' + vodicig.ime >= '" + txtGlavniVodic.Text + "' and (vodicig.prezime + ', ' + vodicig.ime <= '" + txtGlavniVodicDo.Text + "' or vodicig.prezime + ', ' + vodicig.ime <= (select max(vodicig.prezime + ', ' + vodicig.ime) from izleti inner join vodici on izleti.glavni_vod_id = vodicig.id where vodici.prezime + ', ' + vodici.ime like '" + txtGlavniVodicDo.Text + "%'))";
                }
                else if (txtGlavniVodic.Text != "") {
                    filter = filter + " and vodicig.prezime + ', ' + vodicig.ime like '%" + txtGlavniVodic.Text + "%'";
                }
                if (txtPomocniVodic.Text != "" && txtPomocniVodicDo.Text != "") {
                    filter = filter + " and vodicip.prezime + ', ' + vodicip.ime >= '" + txtPomocniVodic.Text + "' and (vodicip.prezime + ', ' + vodicip.ime <= '" + txtPomocniVodicDo.Text + "' or vodicip.prezime + ', ' + vodicip.ime <= (select max(vodicip.prezime + ', ' + vodicip.ime) from izleti inner join vodici on izleti.pomocni_vod_id = vodicip.id where vodici.prezime + ', ' + vodici.ime like '" + txtPomocniVodicDo.Text + "%'))";
                }
                else if (txtPomocniVodic.Text != "") {
                    filter = filter + " and vodicip.prezime + ', ' + vodicip.ime like '%" + txtPomocniVodic.Text + "%'";
                }
                if (txtUdruga.Text != "" && txtUdrugaDo.Text != "") {
                    filter = filter + " and udruge.naziv >= '" + txtUdruga.Text + "' and (udruge.naziv <= '" + txtUdrugaDo.Text + "' or udruge.naziv <= (select max(udruge.naziv) from izleti inner join udruge on izleti.udr_id = udruge.id where udruge.naziv like '" + txtUdrugaDo.Text + "%'))";
                }
                else if (txtGlavniVodic.Text != "") {
                    filter = filter + " and udruge.naziv like '%" + txtUdruga.Text + "%'";
                }
                if (txtOdrediste.Text != "" && txtOdredisteDo.Text != "") {
                    filter = filter + " and odrediste >= '" + txtOdrediste.Text + "' and (odrediste <= '" + txtOdredisteDo.Text + "' or odrediste <= (select max(odrediste) from izleti where odrediste like '" + txtOdredisteDo.Text + "%'))";
                }
                else if (txtOdrediste.Text != "") {
                    filter = filter + " and odrediste like '%" + txtOdrediste.Text + "%'";
                }
                if (txtTrajanje.Text != "" && txtTrajanjeDo.Text != "") {
                    filter = filter + " and trajanje >= " + txtTrajanje.Text + " and trajanje <= " + txtTrajanjeDo.Text; 
                }
                else if (txtOdrediste.Text != "") {
                    filter = filter + " and trajanje = " + txtTrajanje.Text;
                }
                if (txtBrojSudionika.Text != "" && txtBrojSudionikaDo.Text != "") {
                    filter = filter + " and brojsudionika >= " + txtBrojSudionika.Text + " and brojsudionika <= " + txtBrojSudionikaDo.Text;
                }
                else if (txtBrojSudionika.Text != "") {
                    filter = filter + " and brojsudionika = " + txtBrojSudionika.Text;
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
                    string query = "select id from izleti where datum & naziv = (select min(datum & naziv) from izleti where 1 = 1 " + filter + " " + ViewState["filter"] + ")";
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
                        query = "select id from izleti where datum & naziv = (select min(datum & naziv) from izleti where 1 = 1 " + filter + ")";
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