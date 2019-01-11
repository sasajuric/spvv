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
using System.IO;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using SD = System.Drawing;
using System.Drawing.Drawing2D;

namespace spvv {
    public partial class wfVodici:System.Web.UI.Page {

        public string action = "";
        public int pageCurrent;
        public int pageSize = 19;
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

                query = "select udruge.id, udruge.naziv + ', ' + mjesta.naziv as udruga from udruge left join mjesta on udruge.mje_id = mjesta.id union select null, null from dual order by udruga";
                cmd.CommandText = query;
                dr = cmd.ExecuteReader();
                dt = new DataTable();
                dt.Load(dr);
                ddlUdruga.DataSource = dt;
                ddlUdruga.DataTextField = "udruga";
                ddlUdruga.DataValueField = "id";
                ddlUdruga.DataBind();

                con.Close();
            }
            else {
                action = (String)ViewState["action"];
                if (action != "") {
                    txtIme.Focus();
                }
            } 
            pageCurrent = Convert.ToInt32(ViewState["pageCurrent"]);
            pageCount = Convert.ToInt32(ViewState["pageCount"]);
        }

        protected void refreshGrid()
        {            
            OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
            OleDbCommand cmd = new OleDbCommand();
            string query = "select count(vodici.id) " +
                    "from (((((vodici left join udruge on vodici.udr_id = udruge.id) " + 
                    "left join qvodicitecajevi on vodici.id = qvodicitecajevi.vod_id) " +
                    "left join qvodiciispiti on vodici.id = qvodiciispiti.vod_id) " +
                    "left join poste on vodici.pos_id = poste.id) " + 
                    "left join mjesta on vodici.mje_id = mjesta.id) " +
                    "left join mjesta as udruge_mjesta on udruge.mje_id = udruge_mjesta.id " +
                    "where 1 = 1 " + ViewState["filter"];
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
            query = "select vodici.id, vodici.prezime + ', ' + vodici.ime as prezimeime, udruge.naziv + ', ' + udruge_mjesta.naziv as udruga, vodici.licencado, qvodicitecajevi.standard as pripravnik, qvodiciispiti.standard as vodic, vodici.instruktorkandidat, vodici.instruktor, vodici.oib, vodici.spol, right(vodici.clanarina, 4) as clanarina " +
                    "from (((((vodici left join udruge on vodici.udr_id = udruge.id) " + 
                    "left join qvodicitecajevi on vodici.id = qvodicitecajevi.vod_id) " +
                    "left join qvodiciispiti on vodici.id = qvodiciispiti.vod_id) " +
                    "left join poste on vodici.pos_id = poste.id) " + 
                    "left join mjesta on vodici.mje_id = mjesta.id) " +
                    "left join mjesta as udruge_mjesta on udruge.mje_id = udruge_mjesta.id " +
                    "where 1 = 1 " + ViewState["filter"] + " order by vodici.prezime, vodici.ime";
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
            query = "select * from (select top " + pageSize.ToString() + " * from (select top " + (pageSize * (pageCurrent + 1)).ToString() + " * from (" + query + ")) order by prezimeime desc) order by prezimeime";
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
            if (action == "Dodaj") {
                imgFotografija.ImageUrl = "foto/prazna.jpg";
            }
            if (action == "Izmijeni" && hdnID.Value != "") {
                string query = "select ime, prezime, brojznacke, udr_id, ulica, kucnibroj, pos_id, mje_id, email, telefon, datumrodjenja, datumsmrti, velicinamajice, licencado, izvor, napomena, primjedba, instruktorkandidat, instruktor, oib, spol, fotografija, clanarina from vodici where id = " + hdnID.Value;
                OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
                OleDbCommand cmd = new OleDbCommand();     
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                OleDbDataReader dr = cmd.ExecuteReader();
                if (dr.Read()) {
                    txtIme.Text = (dr[0] as string);
                    txtPrezime.Text = (dr[1] as string);
                    txtBrojZnacke.Text = (dr[2] as string);
                    ddlUdruga.SelectedValue = (dr[3] as int?).ToString();
                    txtUlica.Text = (dr[4] as string);
                    txtKucniBroj.Text = (dr[5] as string);
                    ddlPosta.SelectedValue = (dr[6] as int?).ToString();
                    ddlMjesto.SelectedValue = (dr[7] as int?).ToString();
                    txtEmail.Text = (dr[8] as string);
                    txtTelefon.Text = (dr[9] as string);
                    if (dr["datumrodjenja"].ToString() != "") {
                        txtDatumRodjenja.Text = Convert.ToDateTime(dr["datumrodjenja"]).ToString("dd.MM.yyyy.");
                    }
                    if (dr["datumsmrti"].ToString() != "") {
                        txtDatumSmrti.Text = Convert.ToDateTime(dr["datumsmrti"]).ToString("dd.MM.yyyy.");
                    }
                    txtVelicinaMajice.Text = (dr[12] as string);
                    txtLicencaVaziDo.Text = (dr[13] as int?).ToString();
                    txtIzvor.Text = (dr[14] as string);
                    txtNapomena.Text = (dr[15] as string);
                    txtPrimjedba.Text = (dr[16] as string);
                    txtInstruktorKandidat.Text = dr["instruktorkandidat"].ToString();
                    txtInstruktor.Text = dr["instruktor"].ToString();                    
                    txtOIB.Text = dr["oib"].ToString();
                    txtSpol.Text = dr["spol"].ToString();
                    if (dr["fotografija"] != DBNull.Value) {
                        imgFotografija.ImageUrl = dr["fotografija"].ToString();
                    }
                    else {
                        imgFotografija.ImageUrl = "foto/prazna.jpg";
                    }
                    txtClanarina.Text = dr["clanarina"].ToString();
                }
                con.Close();
            }
            if (action == "Dodaj" || action == "Izmijeni" || action == "Traži" || action == "Filtriraj") {
                txtIme.Focus();
            }
        }

        private static void AddImageToBody(string relationshipId, Paragraph p)
        {
            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         //new DW.Extent() { Cx = 990000L, Cy = 792000L },
                         new DW.Extent() { Cx = 1897200L, Cy = 2538000L },
                         new DW.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, 
                             RightEdge = 0L, BottomEdge = 0L },
                         new DW.DocProperties() { Id = (UInt32Value)1U, 
                             Name = "Picture 1" },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties() 
                                            { Id = (UInt32Value)0U, 
                                                Name = "New Bitmap Image.jpg" },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension() 
                                                    { Uri = 
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}" })
                                         ) 
                                         { Embed = relationshipId, 
                                             CompressionState = 
                                             A.BlipCompressionValues.Print },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             //new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                             new A.Extents() { Cx = 1897200L, Cy = 2538000L }),                                             
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         ) { Preset = A.ShapeTypeValues.Rectangle }))
                             ) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     ) { DistanceFromTop = (UInt32Value)0U, 
                         DistanceFromBottom = (UInt32Value)0U, 
                         DistanceFromLeft = (UInt32Value)0U, 
                         DistanceFromRight = (UInt32Value)0U, EditId = "50D07946" });

            p.AppendChild(new Run(element));
        }

        protected void btnIspisi_Click(object sender, EventArgs e)
        {
            Body body;
            Paragraph par;
            Run run;
            DocumentFormat.OpenXml.Wordprocessing.TableRow tr;
            DocumentFormat.OpenXml.Wordprocessing.TableRow trm;
            DocumentFormat.OpenXml.Wordprocessing.TableCell tc;
            DocumentFormat.OpenXml.Wordprocessing.TableCell tcm;
            TableCellProperties tcp;
            RunProperties rp;

            WordprocessingDocument package = WordprocessingDocument.Create(HttpRuntime.AppDomainAppPath + "\\vodici.docx", WordprocessingDocumentType.Document);
            package.AddMainDocumentPart(); 

            Document document = new Document();
            package.MainDocumentPart.Document = document;
            body = document.AppendChild(new Body());
            SectionProperties sectionProps = body.AppendChild(new SectionProperties());
            sectionProps.Append(new PageMargin() { Top = 720, Right = 720, Bottom = 720, Left = 720, Header = 0, Footer = 0, Gutter = 0 }); // 1440 = 1", 720 = 0.5";

            OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstr"].ConnectionString);
            OleDbCommand cmd = new OleDbCommand();     
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            con.Open();

            if (hdnID.Value == "") {
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
                run.Append(new Text("Ime"));
                // ćelija
                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                rp.Append(new Color() { Val = "ffffff" });
                rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                run.Append(new Text("Prezime"));
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
                run.Append(new Text("Lic. do"));
                // ćelija
                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                rp.Append(new Color() { Val = "ffffff" });
                rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                run.Append(new Text("Pripr."));
                // ćelija
                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                rp.Append(new Color() { Val = "ffffff" });
                rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                run.Append(new Text("Vodič"));
                // ćelija
                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                rp.Append(new Color() { Val = "ffffff" });
                rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                run.Append(new Text("Inst. k."));
                // ćelija
                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                rp.Append(new Color() { Val = "ffffff" });
                rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });
                run.Append(new Text("Inst."));
                // članarina
                tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                tcp = tc.AppendChild(new TableCellProperties());
                tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "auto", Fill = "black" });
                par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                run = par.AppendChild(new Run());
                rp = run.AppendChild(new RunProperties());
                rp.Append(new Color() { Val = "ffffff" });
                rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });
                run.Append(new Text("Član"));

                string query = "select vodici.ime, vodici.prezime, udruge.naziv + ', ' + udruge_mjesta.naziv as udruga, vodici.licencado, qvodicitecajevi.standard as pripravnik, qvodiciispiti.standard as vodic, vodici.instruktorkandidat, vodici.instruktor, vodici.oib, vodici.spol, right(vodici.clanarina, 4) as clanarina " +
                    "from (((((vodici left join udruge on vodici.udr_id = udruge.id) " + 
                    "left join qvodicitecajevi on vodici.id = qvodicitecajevi.vod_id) " +
                    "left join qvodiciispiti on vodici.id = qvodiciispiti.vod_id) " +
                    "left join poste on vodici.pos_id = poste.id) " + 
                    "left join mjesta on vodici.mje_id = mjesta.id) " +
                    "left join mjesta as udruge_mjesta on udruge.mje_id = udruge_mjesta.id " +
                    "where 1 = 1 " + ViewState["filter"] + " order by vodici.prezime, vodici.ime";
                cmd.CommandText = query;
                OleDbDataReader dr = cmd.ExecuteReader();
            
                while (dr.Read()) {
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

                    tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                    tcp = tc.AppendChild(new TableCellProperties());
                    par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    run.Append(new Text(dr["licencado"].ToString()));

                    tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                    tcp = tc.AppendChild(new TableCellProperties());
                    par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    run.Append(new Text(dr[4] as string));

                    tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                    tcp = tc.AppendChild(new TableCellProperties());
                    par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    run.Append(new Text(dr[5] as string));

                    tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                    tcp = tc.AppendChild(new TableCellProperties());
                    par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    run.Append(new Text(dr[6] as string));

                    tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                    tcp = tc.AppendChild(new TableCellProperties());
                    par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    run.Append(new Text(dr[7] as string));

                    tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                    tcp = tc.AppendChild(new TableCellProperties());
                    par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    run.Append(new Text(dr["clanarina"] as string));

                    table.Append(tr);
                }
                document.Body.Append(table);
            }
            else {
                string query = "select " +
                    "vodici.ime, " +
                    "vodici.prezime, " +
                    "vodici.brojznacke, " +
                    "udruge.naziv + ', ' + udruge_mjesta.naziv as udruga, " +
                    "vodici.ulica + ' ' + vodici.kucnibroj as adresa, " +
                    "poste.broj + ' ' + poste.naziv as posta, " +
                    "mjesta.naziv as mjesto, " +
                    "vodici.email, " +
                    "vodici.telefon, " +
                    "vodici.datumrodjenja, " +
                    "vodici.datumsmrti, " +
                    "vodici.velicinamajice, " +
                    "vodici.licencado, " +
                    "vodici.izvor, " +
                    "vodici.napomena, " +
                    "vodici.primjedba, " +
                    "qvodicitecajevi.standard as pripravnik, " +
                    "qvodiciispiti.standard as vodic, " +
                    "vodici.instruktorkandidat, " +
                    "vodici.instruktor, " +
                    "vodici.fotografija, " +
                    "vodici.oib, " + 
                    "vodici.spol, " +
                    "vodici.clanarina " +
                    "from (((((vodici left join udruge on vodici.udr_id = udruge.id) " + 
                    "left join qvodicitecajevi on vodici.id = qvodicitecajevi.vod_id) " +
                    "left join qvodiciispiti on vodici.id = qvodiciispiti.vod_id) " +
                    "left join poste on vodici.pos_id = poste.id) " + 
                    "left join mjesta on vodici.mje_id = mjesta.id) " +
                    "left join mjesta as udruge_mjesta on udruge.mje_id = udruge_mjesta.id " +
                    "where vodici.id = " + hdnID.Value;
                cmd.CommandText = query;
                OleDbDataReader dr = cmd.ExecuteReader();
            
                if (dr.Read()) {
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
                                        Val = new EnumValue<BorderValues>(BorderValues.None),
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
                                       Val = new EnumValue<BorderValues>(BorderValues.None),
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
                    tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "black", Fill = "silver" });
                    tcp.Append(new GridSpan() { Val = 2 });
                    tcp.Append(new TableCellMargin(new TopMargin() { Width = "100" }, new BottomMargin() { Width = "100" }, new LeftMargin() { Width = "100" }, new RightMargin() { Width = "100" }));
                    tcp.Append(new BottomBorder {Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 48, Color = "gray"});
                    par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "48" });
                    run.Append(new Text(dr["ime"] + " " + dr["prezime"] + " [" + dr["brojznacke"] + "]"));
                    // red
                    tr = table.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableRow());
                    // ćelija
                    tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                    tcp = tc.AppendChild(new TableCellProperties());
                    tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "black", Fill = "silver" });
                    tcp.Append(new TableCellMargin(new TopMargin() { Width = "200" }, new BottomMargin() { Width = "100" }, new LeftMargin() { Width = "100" }, new RightMargin() { Width = "100" }));
                    tcp.Append(new TableCellWidth() { Width = "0" });
                    par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                    // slika
                    if (dr["fotografija"] != DBNull.Value) {
                        MainDocumentPart mainPart = package.MainDocumentPart;
                        ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
                        try {
                            using (FileStream stream = new FileStream(Server.MapPath(dr["fotografija"].ToString()), FileMode.Open)) { imagePart.FeedData(stream); }
                            AddImageToBody(mainPart.GetIdOfPart(imagePart), par);
                        }
                        catch {
                        }
                    }
                    else {
                        MainDocumentPart mainPart = package.MainDocumentPart;
                        ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
                        try {
                            using (FileStream stream = new FileStream(Server.MapPath("foto\\prazna.jpg"), FileMode.Open)) { imagePart.FeedData(stream); }
                            AddImageToBody(mainPart.GetIdOfPart(imagePart), par);
                        }
                        catch {
                        }
                    }
                    // mala tablica ispod slike
                    DocumentFormat.OpenXml.Wordprocessing.Table tablemala = new DocumentFormat.OpenXml.Wordprocessing.Table();
                    TableProperties propsm = new TableProperties
                        (
                            new TableBorders
                                (
                                    new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }, // 100%
                                    new TopBorder
                                    {
                                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                                        Size = 6
                                    },
                                    new BottomBorder
                                    {
                                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                                        Size = 6
                                    },
                                    new LeftBorder
                                    {
                                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                                        Size = 6
                                    },
                                    new RightBorder
                                    {
                                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                                        Size = 6
                                    },
                                    new InsideHorizontalBorder
                                    {
                                       Val = new EnumValue<BorderValues>(BorderValues.Single),
                                        Size = 6
                                    },
                                    new InsideVerticalBorder
                                    {
                                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                                        Size = 6
                                    }
                                )
                        );
                    tablemala.AppendChild<TableProperties>(propsm);
                    // red 1
                    trm = tablemala.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableRow());                    
                    // ćelija 1
                    tcm = trm.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                    tcp = tcm.AppendChild(new TableCellProperties());
                    tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "black", Fill = "white" });
                    tcp.Append(new TableCellWidth() { Width = "50%" });
                    par = tcm.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" }, new Justification() { Val = JustificationValues.Center } )));
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "32" });
                    run.Append(new Text(dr["pripravnik"].ToString()));
                    run.Append(new Break());
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "20" });
                    run.Append(new Text("pripravnik"));
                    // ćelija 2
                    tcm = trm.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                    tcp = tcm.AppendChild(new TableCellProperties());
                    tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "black", Fill = "white" });
                    par = tcm.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" }, new Justification() { Val = JustificationValues.Center } )));
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "32" });
                    run.Append(new Text(dr["vodic"].ToString()));
                    run.Append(new Break());
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "20" });
                    run.Append(new Text("vodič"));
                    // red 2
                    trm = tablemala.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableRow());                    
                    // ćelija 3
                    tcm = trm.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                    tcp = tcm.AppendChild(new TableCellProperties());
                    tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "black", Fill = "white" });
                    par = tcm.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" }, new Justification() { Val = JustificationValues.Center } )));
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "32" });
                    run.Append(new Text(dr["instruktorkandidat"].ToString()));
                    run.Append(new Break());
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "20" });
                    run.Append(new Text("instr. kandidat"));
                    // ćelija 4
                    tcm = trm.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                    tcp = tcm.AppendChild(new TableCellProperties());
                    tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "black", Fill = "white" });
                    par = tcm.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" }, new Justification() { Val = JustificationValues.Center } )));
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "32" });
                    run.Append(new Text(dr["instruktor"].ToString()));
                    run.Append(new Break());
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "20" });
                    run.Append(new Text("instruktor"));

                    // u ćeliju sa slikom dodaj još jedan paragraph i na nju dodaj malu tablicu
                    par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "16" });
                    par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));
                    run = par.AppendChild(new Run());                    
                    run.Append(tablemala);
                    for (int i = 0; i < 26; i++) {
                        run.Append(new Break());
                    }

                    // glavna ćelija desno
                    tc = tr.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.TableCell());
                    tcp = tc.AppendChild(new TableCellProperties());
                    tcp.Append(new TableCellMargin(new LeftMargin() { Width = "100" }, new RightMargin() { Width = "100" }));
                    tcp.Append(new Shading() { Val = ShadingPatternValues.Clear, Color = "black", Fill = "white" });
                    par = tc.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" })));

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text("Adresa:"));
                    run.Append(new Break());
                    run = par.AppendChild(new Run());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text(dr["adresa"].ToString()));
                    run.Append(new Break());
                    run.Append(new Text(dr["mjesto"].ToString()));
                    run.Append(new Break());
                    run.Append(new Text(dr["posta"].ToString()));
                    run.Append(new Break());
                    run.Append(new Break());

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text() { Text = "Telefon: ", Space = SpaceProcessingModeValues.Preserve });
                    run = par.AppendChild(new Run());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text(dr["telefon"].ToString()));
                    run.Append(new Break()); 

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text() { Text = "E-mail: ", Space = SpaceProcessingModeValues.Preserve });
                    run = par.AppendChild(new Run());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text(dr["email"].ToString()));
                    run.Append(new Break()); 
                    run.Append(new Break()); 

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text() { Text = "OIB: ", Space = SpaceProcessingModeValues.Preserve });
                    run = par.AppendChild(new Run());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text(dr["OIB"].ToString()));
                    run.Append(new Break()); 
                    run.Append(new Break()); 

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text() { Text = "Datum rođenja: ", Space = SpaceProcessingModeValues.Preserve });
                    run = par.AppendChild(new Run());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    if (dr["datumrodjenja"].ToString() != "") {
                        run.Append(new Text(Convert.ToDateTime(dr["datumrodjenja"]).ToString("dd.MM.yyyy.")));
                    }
                    run.Append(new Break());
                    run.Append(new Break());

                    if (dr["datumsmrti"].ToString() != "") {
                        run = par.AppendChild(new Run());
                        rp = run.AppendChild(new RunProperties());
                        rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                        rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                        run.Append(new Text() { Text = "Datum smrti: ", Space = SpaceProcessingModeValues.Preserve });
                        run = par.AppendChild(new Run());
                        rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                        run.Append(new Text(Convert.ToDateTime(dr["datumsmrti"]).ToString("dd.MM.yyyy.")));
                        run.Append(new Break());
                        run.Append(new Break());
                    }

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text() { Text = "Udruga: ", Space = SpaceProcessingModeValues.Preserve });
                    run = par.AppendChild(new Run());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text(dr["udruga"].ToString()));
                    run.Append(new Break());
                    run.Append(new Break());

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text() { Text = "Spol: ", Space = SpaceProcessingModeValues.Preserve });
                    run = par.AppendChild(new Run());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text(dr["spol"].ToString()));
                    run.Append(new Break()); 
                    run.Append(new Break()); 

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text() { Text = "Veličina majice: ", Space = SpaceProcessingModeValues.Preserve });
                    run = par.AppendChild(new Run());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text(dr["velicinamajice"].ToString()));
                    run.Append(new Break());
                    run.Append(new Break());

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text() { Text = "Licenca važi do: ", Space = SpaceProcessingModeValues.Preserve });
                    run = par.AppendChild(new Run());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text(dr["licencado"].ToString()));
                    run.Append(new Break());
                    run.Append(new Break());

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text() { Text = "Licenca važi do: ", Space = SpaceProcessingModeValues.Preserve });
                    run = par.AppendChild(new Run());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text(dr["clanarina"].ToString()));
                    run.Append(new Break());
                    run.Append(new Break());

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text("Tečajevi:"));
                    run.Append(new Break());

                    OleDbCommand cmd1 = new OleDbCommand();     
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Connection = con;
                    query = "select tecajevi.naziv, tecajevi.datum, vodici.prezime + ', ' + vodici.ime as voditelj, standardi.naziv as standard from ((tecajevi left join vodici on tecajevi.voditelj_vod_id = vodici.id) left join standardi on tecajevi.std_id = standardi.id) left join tecajevitecajci on tecajevi.id = tecajevitecajci.tec_id where tecajevitecajci.vod_id = " + hdnID.Value + " order by standardi.naziv";
                    cmd1.CommandText = query;
                    OleDbDataReader dr1 = cmd1.ExecuteReader();                    
                    while (dr1.Read())
                    {
                        run = par.AppendChild(new Run());
                        rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                        run.Append(new Text() { Text = dr1["standard"].ToString() + " - " + Convert.ToDateTime(dr1["datum"]).ToString("dd.MM.yyyy.") + ": " + dr1["naziv"].ToString() + " (" + dr1["voditelj"].ToString() + ")", Space = SpaceProcessingModeValues.Preserve });
                        run.Append(new Break());
                    }
                    run.Append(new Break());

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text("Ispiti:"));
                    run.Append(new Break());

                    query = "select ispiti.naziv, ispiti.datum, vodici.prezime + ', ' + vodici.ime as voditelj, standardi.naziv as standard, ispitikandidati.polozio from ((ispiti left join vodici on ispiti.voditelj_vod_id = vodici.id) left join standardi on ispiti.std_id = standardi.id) left join ispitikandidati on ispiti.id = ispitikandidati.isp_id where ispitikandidati.vod_id = " + hdnID.Value + " order by standardi.naziv";
                    cmd1.CommandText = query;
                    dr1.Close();
                    dr1 = cmd1.ExecuteReader();                    
                    string polozio = "";
                    while (dr1.Read())
                    {
                        if (Convert.ToBoolean(dr1["polozio"]) == true) {
                            polozio = "položio";
                        }
                        else {
                            polozio = "nije položio";
                        }
                        run = par.AppendChild(new Run());
                        rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                        run.Append(new Text() { Text = dr1["standard"].ToString() + " - " + Convert.ToDateTime(dr1["datum"]).ToString("dd.MM.yyyy.") + ": " + dr1["naziv"].ToString() + " (" + dr1["voditelj"].ToString() + ") - " + polozio, Space = SpaceProcessingModeValues.Preserve });
                        run.Append(new Break());
                    }
                    run.Append(new Break());

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text("Vježbe:"));
                    run.Append(new Break());

                    query = "select vjezbe.naziv, vjezbe.datum, vodici.prezime + ', ' + vodici.ime as voditelj, standardi.naziv as standard from ((vjezbe left join vodici on vjezbe.voditelj_vod_id = vodici.id) left join standardi on vjezbe.std_id = standardi.id) left join vjezbevodici on vjezbe.id = vjezbevodici.vje_id where vjezbevodici.vod_id = " + hdnID.Value + " order by vjezbe.datum";
                    cmd1.CommandText = query;
                    dr1.Close();
                    dr1 = cmd1.ExecuteReader();                    
                    while (dr1.Read())
                    {
                        run = par.AppendChild(new Run());
                        rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                        run.Append(new Text() { Text = dr1["standard"].ToString() + " - " + Convert.ToDateTime(dr1["datum"]).ToString("dd.MM.yyyy.") + ": " + dr1["naziv"].ToString() + " (" + dr1["voditelj"].ToString() + ")", Space = SpaceProcessingModeValues.Preserve });
                        run.Append(new Break());
                    }
                    run.Append(new Break());

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text("Izleti:"));
                    run.Append(new Break());

                    query = "select izleti.datum, izleti.naziv, udruge.naziv + ', ' + udruge.naziv as udruga, izleti.odrediste from (izleti left join udruge on izleti.udr_id = udruge.id) left join mjesta on udruge.mje_id = mjesta.id where glavni_vod_id = " + hdnID.Value + " or pomocni_vod_id = " + hdnID.Value + " order by izleti.datum";
                    cmd1.CommandText = query;
                    dr1.Close();
                    dr1 = cmd1.ExecuteReader();                    
                    while (dr1.Read())
                    {
                        run = par.AppendChild(new Run());
                        rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                        run.Append(new Text() { Text = Convert.ToDateTime(dr1["datum"]).ToString("dd.MM.yyyy.") + ": " + dr1["naziv"].ToString() + " (" + dr1["odrediste"].ToString() + ") " + dr1["udruga"].ToString(), Space = SpaceProcessingModeValues.Preserve });
                        run.Append(new Break());
                    }
                    run.Append(new Break());

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text("Primjedba:"));
                    run.Append(new Break());
                    run = par.AppendChild(new Run());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text(dr["primjedba"].ToString()));
                    run.Append(new Break());
                    run.Append(new Break());
                

                    run = par.AppendChild(new Run());
                    rp = run.AppendChild(new RunProperties());
                    rp.Append(new Bold() { Val = OnOffValue.FromBoolean(true) });            
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text("Napomena:"));
                    run.Append(new Break());
                    run = par.AppendChild(new Run());
                    rp.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "24" });
                    run.Append(new Text(dr["napomena"].ToString()));
                    run.Append(new Break());
                    run.Append(new Break());

                    document.Body.Append(table);
                }
               
            }
            document.Save();
            package.Close();

            con.Close();

            Response.ContentType = "Application/docx";
            Response.AppendHeader("Content-Disposition", "attachment; filename=vodici.docx");
            Response.TransmitFile(HttpRuntime.AppDomainAppPath + "\\vodici.docx");
            Response.End();
        }

        protected void btnPotvrdi_Click(object sender, EventArgs e)
        {
            if (action == "Dodaj")
            {
                string query = "insert into vodici (ime, prezime, brojznacke, udr_id, ulica, kucnibroj, pos_id, mje_id, email, telefon, datumrodjenja, datumsmrti, velicinamajice, licencado, izvor, napomena, primjedba, instruktorkandidat, instruktor, oib, spol, fotografija, clanarina) values (" +
                    nulls(txtIme.Text) + ", " +
                    nulls(txtPrezime.Text) + ", " +
                    nulls(txtBrojZnacke.Text) + ", " +
                    nulld(ddlUdruga.SelectedValue) + ", " +
                    nulls(txtUlica.Text) + ", " +
                    nulls(txtKucniBroj.Text) + ", " +
                    nulld(ddlPosta.SelectedValue) + ", " +
                    nulld(ddlMjesto.SelectedValue) + ", " +
                    nulls(txtEmail.Text) + ", " +
                    nulls(txtTelefon.Text) + ", " +
                    nulls(txtDatumRodjenja.Text) + ", " +
                    nulls(txtDatumSmrti.Text) + ", " +
                    nulls(txtVelicinaMajice.Text) + ", " +
                    nulls(txtLicencaVaziDo.Text) + ", " +
                    nulls(txtIzvor.Text) + ", " +
                    nulls(txtNapomena.Text) + ", " +
                    nulls(txtPrimjedba.Text) + ", " +
                    nulls(txtInstruktorKandidat.Text) + ", " +
                    nulls(txtInstruktor.Text) + ", " +
                    nulls(txtOIB.Text) + ", " +
                    nulls(txtSpol.Text) + ", " +
                    nulls(imgFotografija.ImageUrl) + ", " +
                    nulls(txtClanarina.Text) +
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
                string query = "update vodici set " + 
                        "ime = " + nulls(txtIme.Text) + ", " +
                        "prezime = " + nulls(txtPrezime.Text) + ", " +
                        "brojznacke = " + nulls(txtBrojZnacke.Text) + ", " +
                        "udr_id = " + nulld(ddlUdruga.SelectedValue) + ", " +
                        "ulica = " + nulls(txtUlica.Text) + ", " +
                        "kucnibroj = " + nulls(txtKucniBroj.Text) + ", " +
                        "pos_id = " + nulld(ddlPosta.SelectedValue) + ", " +
                        "mje_id = " + nulld(ddlMjesto.SelectedValue) + ", " +
                        "email = " + nulls(txtEmail.Text) + ", " +
                        "telefon = " + nulls(txtTelefon.Text) + ", " +
                        "datumrodjenja = " + nulls(txtDatumRodjenja.Text) + ", " +
                        "datumsmrti = " + nulls(txtDatumSmrti.Text) + ", " +
                        "velicinamajice = " + nulls(txtVelicinaMajice.Text) + ", " +
                        "licencado = " + nulls(txtLicencaVaziDo.Text) + ", " +
                        "izvor = " + nulls(txtIzvor.Text) + ", " +
                        "napomena = " + nulls(txtNapomena.Text) + ", " +
                        "primjedba = " + nulls(txtPrimjedba.Text) + ", " +
                        "instruktorkandidat = " + nulls(txtInstruktorKandidat.Text) + ", " +
                        "instruktor = " + nulls(txtInstruktor.Text) + ", " +
                        "oib = " + nulls(txtOIB.Text) + ", " +
                        "spol = " + nulls(txtSpol.Text) + ", " +
                        "fotografija = " + nulls(imgFotografija.ImageUrl) + ", " +
                        "clanarina = " + nulls(txtClanarina.Text) + " " +
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
                string query = "delete from vodici where id = " + hdnID.Value;
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
                if (txtIme.Text != "" && txtImeDo.Text != "") {
                    filter = " and vodici.ime >= '" + txtIme.Text + "' and (vodici.ime <= '" + txtImeDo.Text + "' or vodici.ime <= (select max(ime) from vodici where ime like '" + txtImeDo.Text + "%'))";
                }
                else if (txtIme.Text != "") {
                    filter = " and vodici.ime like '%" + txtIme.Text + "%'";
                }
                if (txtPrezime.Text != "" && txtPrezimeDo.Text != "") {
                    filter = filter + " and vodici.prezime >= '" + txtPrezime.Text + "' and (vodici.prezime <= '" + txtPrezimeDo.Text + "' or vodici.prezime <= (select max(prezime) from vodici where prezime like '" + txtPrezimeDo.Text + "%'))";
                }
                else if (txtPrezime.Text != "") {
                    filter = filter + " and vodici.prezime like '%" + txtPrezime.Text + "%'";
                }
                if (txtSpol.Text != "" && txtSpolDo.Text != "") {
                    filter = filter + " and vodici.spol >= '" + txtSpol.Text + "' and (vodici.spol <= '" + txtSpolDo.Text + "' or vodici.spol <= (select max(spol) from vodici where spol like '" + txtSpolDo.Text + "%'))";
                }
                else if (txtSpol.Text != "") {
                    filter = filter + " and vodici.spol like '%" + txtSpol.Text + "%'";
                }
                if (txtOIB.Text != "" && txtOIBDo.Text != "") {
                    filter = filter + " and vodici.oib >= '" + txtOIB.Text + "' and (vodici.oib <= '" + txtOIBDo.Text + "' or vodici.oib <= (select max(oib) from vodici where oib like '" + txtOIBDo.Text + "%'))";
                }
                else if (txtOIB.Text != "") {
                    filter = filter + " and vodici.oib like '%" + txtOIB.Text + "%'";
                }
                if (txtBrojZnacke.Text != "" && txtBrojZnackeDo.Text != "") {
                    filter = filter + " and vodici.brojznacke >= '" + txtBrojZnacke.Text + "' and (vodici.brojznacke <= '" + txtBrojZnackeDo.Text + "' or vodici.brojznacke <= (select max(brojznacke) from vodici where brojznacke like '" + txtBrojZnackeDo.Text + "%'))";
                }
                else if (txtBrojZnacke.Text != "") {
                    filter = filter + " and vodici.brojznacke like '%" + txtBrojZnacke.Text + "%'";
                }
                if (txtUdruga.Text != "" && txtUdrugaDo.Text != "") {
                    filter = filter + " and udruge.naziv + ', ' + mjesta.naziv >= '" + txtUdruga.Text + "' and (udruge.naziv + ', ' + udruge_mjesta.naziv as udruga <= '" + txtUdrugaDo.Text + "' or udruge.naziv + ', ' + udruge_mjesta.naziv as udruga <= (select max(udruge.naziv + ', ' + mjesta.naziv) from (vodici inner join udruge on vodici.udr_id = udruge.id) left join mjesta on udruge.mje_id = mjesta.id where udruge.naziv + ', ' + mjesta.naziv like '" + txtUdrugaDo.Text + "%'))";
                }
                else if (txtUdruga.Text != "") {
                    filter = filter + " and udruge.naziv + ', ' + mjesta.naziv like '%" + txtUdruga.Text + "%'";
                }
                if (txtUlica.Text != "" && txtUlicaDo.Text != "") {
                    filter = filter + " and vodici.ulica >= '" + txtUlica.Text + "' and (vodici.ulica <= '" + txtUlicaDo.Text + "' or vodici.ulica <= (select max(ulica) from vodici where ulica like '" + txtUlicaDo.Text + "%'))";
                }
                else if (txtUlica.Text != "") {
                    filter = filter + " and vodici.ulica like '%" + txtUlica.Text + "%'";
                }
                if (txtKucniBroj.Text != "" && txtKucniBrojDo.Text != "") {
                    filter = filter + " and vodici.kucnibroj >= '" + txtKucniBroj.Text + "' and (vodici.kucnibroj <= '" + txtKucniBrojDo.Text + "' or vodici.kucnibroj <= (select max(kucnibroj) from vodici where kucnibroj like '" + txtKucniBrojDo.Text + "%'))";
                }
                else if (txtKucniBroj.Text != "") {
                    filter = filter + " and vodici.kucnibroj like '%" + txtKucniBroj.Text + "%'";
                }
                if (txtPosta.Text != "" && txtPostaDo.Text != "") {
                    filter = filter + " and ((poste.broj >= '" + txtPosta.Text + "' and (poste.broj <= '" + txtPostaDo.Text + "' or poste.broj <= (select max(poste.broj) from vodici inner join poste on vodici.pos_id = poste.id where poste.broj like '" + txtPostaDo.Text + "%'))) or " +
                                      "(poste.naziv >= '" + txtPosta.Text + "' and (poste.naziv <= '" + txtPostaDo.Text + "' or poste.naziv <= (select max(poste.naziv) from vodici inner join poste on vodici.pos_id = poste.id where poste.naziv like '" + txtPostaDo.Text + "%'))))";
                }
                else if (txtPosta.Text != "") {
                    filter = filter + " and (poste.broj like '%" + txtPosta.Text + "%') or (poste.naziv like '%" + txtPosta.Text + "%')";
                }
                if (txtMjesto.Text != "" && txtMjestoDo.Text != "") {
                    filter = filter + " and mjesta.naziv >= '" + txtMjesto.Text + "' and (mjesta.naziv <= '" + txtMjestoDo.Text + "' or mjesta.naziv <= (select max(mjesta.naziv) from vodici inner join mjesta on vodici.mje_id = mjesta.id where mjesta.naziv like '" + txtMjestoDo.Text + "%'))";
                }
                else if (txtMjesto.Text != "") {
                    filter = filter + " and mjesta.naziv like '%" + txtMjesto.Text + "%'";
                }
                if (txtEmail.Text != "" && txtEmailDo.Text != "") {
                    filter = filter + " and vodici.email >= '" + txtEmail.Text + "' and (vodici.email <= '" + txtEmailDo.Text + "' or vodici.email <= (select max(email) from vodici where email like '" + txtEmailDo.Text + "%'))";
                }
                else if (txtEmail.Text != "") {
                    filter = filter + " and vodici.email like '%" + txtEmail.Text + "%'";
                }
                if (txtTelefon.Text != "" && txtTelefonDo.Text != "") {
                    filter = filter + " and vodici.telefon >= '" + txtTelefon.Text + "' and (vodici.telefon <= '" + txtTelefonDo.Text + "' or vodici.telefon <= (select max(telefon) from vodici where telefon like '" + txtTelefonDo.Text + "%'))";
                }
                else if (txtTelefon.Text != "") {
                    filter = filter + " and vodici.telefon like '%" + txtTelefon.Text + "%'";
                }
                DateTime dt;
                string d1 = "";
                string d2 = "";
                try {
                    dt = Convert.ToDateTime(txtDatumRodjenja.Text);
                    d1 = "#" + dt.Month + "/" + dt.Day + "/" + dt.Year + "#";
                }
                catch {
                }
                try {
                    dt = Convert.ToDateTime(txtDatumRodjenjaDo.Text);
                    d2 = "#" + dt.Month + "/" + dt.Day + "/" + dt.Year + "#";
                }
                catch {
                }
                if (d1 != "" && d2 != "") {
                    filter = filter + " and vodici.datumrodjenja >= " + d1 + " and vodici.datumrodjenja <= " + d2;
                }
                else if (d1 != "") {
                    filter = filter + " and vodici.datumrodjenja = " + d1;
                }
                d1 = "";
                d2 = "";
                try {
                    dt = Convert.ToDateTime(txtDatumSmrti.Text);
                    d1 = "#" + dt.Month + "/" + dt.Day + "/" + dt.Year + "#";
                }
                catch {
                }
                try {
                    dt = Convert.ToDateTime(txtDatumSmrtiDo.Text);
                    d2 = "#" + dt.Month + "/" + dt.Day + "/" + dt.Year + "#";
                }
                catch {
                }
                if (d1 != "" && d2 != "") {
                    filter = filter + " and vodici.datumsmrti >= " + d1 + " and vodici.datumsmrti <= " + d2;
                }
                else if (d1 != "") {
                    filter = filter + " and vodici.datumsmrti = " + d1;
                }
                if (txtVelicinaMajice.Text != "" && txtVelicinaMajiceDo.Text != "") {
                    filter = filter + " and vodici.velicinamajice >= '" + txtVelicinaMajice.Text + "' and (vodici.velicinamajice <= '" + txtVelicinaMajiceDo.Text + "' or vodici.velicinamajice <= (select max(velicinamajice) from vodici where velicinamajice like '" + txtVelicinaMajiceDo.Text + "%'))";
                }
                else if (txtVelicinaMajice.Text != "") {
                    filter = filter + " and vodici.velicinamajice like '%" + txtVelicinaMajice.Text + "%'";
                }
                if (txtLicencaVaziDo.Text != "" && txtLicencaVaziDoDo.Text != "") {
                    filter = filter + " and vodici.licencado >= " + txtLicencaVaziDo.Text + " and vodici.licencado <= " + txtLicencaVaziDoDo.Text;
                }
                else if (txtLicencaVaziDoDo.Text != "") {
                    filter = filter + " and vodici.licencado = " + txtLicencaVaziDo.Text;
                }
                if (txtIzvor.Text != "" && txtIzvorDo.Text != "") {
                    filter = filter + " and vodici.izvor >= '" + txtIzvor.Text + "' and (vodici.izvor <= '" + txtIzvorDo.Text + "' or vodici.izvor <= (select max(izvor) from vodici where izvor like '" + txtIzvorDo.Text + "%'))";
                }
                else if (txtIzvor.Text != "") {
                    filter = filter + " and vodici.izvor like '%" + txtIzvor.Text + "%'";
                }
                if (txtNapomena.Text != "" && txtNapomenaDo.Text != "") {
                    filter = filter + " and vodici.napomena >= '" + txtNapomena.Text + "' and (vodici.napomena <= '" + txtNapomenaDo.Text + "' or vodici.napomena <= (select max(napomena) from vodici where napomena like '" + txtNapomenaDo.Text + "%'))";
                }
                else if (txtNapomena.Text != "") {
                    filter = filter + " and vodici.napomena like '%" + txtNapomena.Text + "%'";
                }
                if (txtPrimjedba.Text != "" && txtPrimjedbaDo.Text != "") {
                    filter = filter + " and vodici.primjedba >= '" + txtPrimjedba.Text + "' and (vodici.primjedba <= '" + txtPrimjedbaDo.Text + "' or vodici.primjedba <= (select max(primjedba) from vodici where primjedba like '" + txtPrimjedbaDo.Text + "%'))";
                }
                else if (txtPrimjedba.Text != "") {
                    filter = filter + " and vodici.primjedba like '%" + txtPrimjedba.Text + "%'";
                }
                if (txtPripravnik.Text != "" && txtPripravnikDo.Text != "") {
                    filter = filter + " and qvodicitecajevi.standard >= '" + txtPripravnik.Text + "' and (qvodicitecajevi.standard <= '" + txtPripravnikDo.Text + "' or qvodicitecajevi.standard <= (select max(qvodicitecajevi.standard) from vodici inner join qvodicitecajevi on vodici.id = qvodicitecajevi.vod_id where qvodicitecajevi.standard like '" + txtPripravnikDo.Text + "%'))";
                }
                else if (txtPripravnik.Text != "") {
                    filter = filter + " and qvodicitecajevi.standard like '%" + txtPripravnik.Text + "%'";
                }
                if (txtVodic.Text != "" && txtVodicDo.Text != "") {
                    filter = filter + " and qvodiciispiti.standard >= '" + txtVodic.Text + "' and (qvodiciispiti.standard <= '" + txtVodicDo.Text + "' or qvodiciispiti.standard <= (select max(qvodiciispiti.standard) from vodici inner join qvodiciispiti on vodici.id = qvodiciispiti.vod_id where qvodiciispiti.standard like '" + txtVodicDo.Text + "%'))";
                }
                else if (txtVodic.Text != "") {
                    filter = filter + " and qvodiciispiti.standard like '%" + txtVodic.Text + "%'";
                }
                if (txtInstruktorKandidat.Text != "" && txtInstruktorKandidatDo.Text != "") {
                    filter = filter + " and vodici.instruktorkandidat >= '" + txtInstruktorKandidat.Text + "' and (vodici.instruktorkandidat <= '" + txtInstruktorKandidatDo.Text + "' or vodici.instruktorkandidat <= (select max(instruktorkandidat) from vodici where instruktorkandidat like '" + txtInstruktorKandidatDo.Text + "%'))";
                }
                else if (txtInstruktorKandidat.Text != "") {
                    filter = filter + " and vodici.instruktorkandidat like '%" + txtInstruktorKandidat.Text + "%'";
                }
                if (txtInstruktor.Text != "" && txtInstruktorDo.Text != "") {
                    filter = filter + " and vodici.instruktor >= '" + txtInstruktor.Text + "' and (vodici.instruktor <= '" + txtInstruktorDo.Text + "' or vodici.instruktor <= (select max(instruktor) from vodici where instruktor like '" + txtInstruktorDo.Text + "%'))";
                }
                else if (txtInstruktor.Text != "") {
                    filter = filter + " and vodici.instruktor like '%" + txtInstruktor.Text + "%'";
                }
                if (txtClanarina.Text != "" && txtClanarinaDo.Text != "") {
                    filter = filter + " and vodici.clanarina >= '" + txtClanarina.Text + "' and (vodici.clanarina <= '" + txtClanarinaDo.Text + "' or vodici.clanarina <= (select max(clanarina) from vodici where clanarina like '" + txtClanarinaDo.Text + "%'))";
                }
                else if (txtClanarina.Text != "") {
                    filter = filter + " and right(vodici.clanarina, 4) like '%" + txtClanarina.Text + "%'";
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
                    string query = "select id from vodici where prezime + ', ' + ime = (select min(vodici.prezime + ', ' + vodici.ime) " +
                        "from (((((vodici left join udruge on vodici.udr_id = udruge.id) " + 
                        "left join qvodicitecajevi on vodici.id = qvodicitecajevi.vod_id) " +
                        "left join qvodiciispiti on vodici.id = qvodiciispiti.vod_id) " +
                        "left join poste on vodici.pos_id = poste.id) " + 
                        "left join mjesta on vodici.mje_id = mjesta.id) " +
                        "left join mjesta as udruge_mjesta on udruge.mje_id = udruge_mjesta.id " +
                        "where 1 = 1 " + filter + " " + ViewState["filter"] + ")";
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
                        query = "select id from vodici where prezime + ', ' + ime = (select min(vodici.prezime + ', ' + vodici.ime) " +
                            "from (((((vodici left join udruge on vodici.udr_id = udruge.id) " + 
                            "left join qvodicitecajevi on vodici.id = qvodicitecajevi.vod_id) " +
                            "left join qvodiciispiti on vodici.id = qvodiciispiti.vod_id) " +
                            "left join poste on vodici.pos_id = poste.id) " + 
                            "left join mjesta on vodici.mje_id = mjesta.id) " +
                            "left join mjesta as udruge_mjesta on udruge.mje_id = udruge_mjesta.id " +
                            "where 1 = 1 " + filter + ")";
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

        protected void btnUcitajFotografiju_Click(object sender, EventArgs e)
        {
            if (fuFotografija.HasFile) {
                /*string fileName = Path.GetFileName(fuFotografija.PostedFile.FileName);
                fuFotografija.PostedFile.SaveAs(Server.MapPath("~/foto/") + fileName);
                imgFotografija.ImageUrl = Server.MapPath("~/foto/") + fileName;*/

                // Find the fileUpload control
                string filename = fuFotografija.FileName;

                // Specify the upload directory
                string directory = Server.MapPath(@"foto\");

                // Create a bitmap of the content of the fileUpload control in memory
                SD.Bitmap originalBMP = new SD.Bitmap(fuFotografija.FileContent);

                // Calculate the new image dimensions
                decimal origWidth = originalBMP.Width;
                decimal origHeight = originalBMP.Height;
                decimal sngRatio = origWidth / origHeight;
                int newWidth = 240;
                int newHeight = Convert.ToInt32(Math.Round(newWidth / sngRatio));

                // Create a new bitmap which will hold the previous resized bitmap
                SD.Bitmap newBMP = new SD.Bitmap(originalBMP, newWidth, newHeight);
                // Create a graphic based on the new bitmap
                SD.Graphics oGraphics = SD.Graphics.FromImage(newBMP);

                // Set the properties for the new graphic file
                oGraphics.SmoothingMode = SmoothingMode.AntiAlias;oGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                // Draw the new graphic based on the resized bitmap
                oGraphics.DrawImage(originalBMP, 0, 0, newWidth, newHeight);

                // Save the new graphic file to the server
                filename = txtPrezime.Text + "_" + txtIme.Text + "_" + txtDatumRodjenja.Text + ".png";
                newBMP.Save(directory + filename, System.Drawing.Imaging.ImageFormat.Png);

                // Once finished with the bitmap objects, we deallocate them.
                originalBMP.Dispose();
                newBMP.Dispose();
                oGraphics.Dispose();

                imgFotografija.ImageUrl = "foto/" + filename;
            }            
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