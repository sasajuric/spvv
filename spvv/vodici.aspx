<%@ Page Title="" Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeBehind="vodici.aspx.cs" Inherits="spvv.wfVodici" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var mainID = '';
        function grdMain_Click(id)
        {
            if (mainID != '' && document.all['trMain' + mainID] != null)
            {
                document.all['trMain' + mainID].classList.remove('selectedrow');
            }
            if (mainID != id)
            {
                if (document.all['trMain' + id] != null) {
                    document.all['trMain' + id].classList.add('selectedrow');
                }
                mainID = id;
            }
            else
            {
                mainID = '';
            }
            document.all['cphGlavni_hdnID'].value = mainID;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphGlavni" runat="server">
    <div style="margin: auto; padding: 20px; border: 1px solid #A0A0A0; border-radius: 10px; background: white; width: 100%; height: 100%; box-sizing: border-box">
        <table style="border-collapse: collapse; width: 100%"><tr><td style="font-size: 2em">Županije</td><td style="text-align: right"><button type="button" id="Button4" class="menu" style="margin-left: auto; font-weight: bold; padding: 10px; width: 44px; line-height: 20px" onclick="window.location='default.aspx'">X</button></td></tr></table>
        <br/>
        <asp:Repeater ID="rptMain" runat="server">
            <HeaderTemplate>
                <table style="border-collapse: collapse; width: 100%" class="grid">
                    <tr class="header"> 
                        <th style="width: 20%">Prezime, ime</th>
                        <th style="width: 20%">Udruga</th>
                        <th style="width: 10%">Lic. do</th>
                        <th style="width: 10%">Pripr.</th>
                        <th style="width: 10%">Vodič</th>
                        <th style="width: 10%">Inst. k.</th>
                        <th style="width: 10%">Inst.</th>
                        <th style="width: 10%">Član</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                    <tr class="row" id="trMain<%# Eval("id") %>" onclick="grdMain_Click(<%# Eval("id") %>)">
                        <td style="width: 20%"><%# Eval("prezimeime") %></td>
                        <td style="width: 20%"><%# Eval("udruga") %></td>
                        <td style="width: 10%; text-align: right"><%# Eval("licencado") %></td>
                        <td style="width: 10%; text-align: center"><%# Eval("pripravnik") %></td>
                        <td style="width: 10%; text-align: center"><%# Eval("vodic") %></td>
                        <td style="width: 10%; text-align: center"><%# Eval("instruktorkandidat") %></td>
                        <td style="width: 10%; text-align: center"><%# Eval("instruktor") %></td>
                        <td style="width: 10%; text-align: center"><%# Eval("clanarina") %></td>
                    </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                    <tr class="altrow" id="trMain<%# Eval("id") %>" onclick="grdMain_Click(<%# Eval("id") %>)">
                        <td style="width: 20%"><%# Eval("prezimeime") %></td>
                        <td style="width: 20%"><%# Eval("udruga") %></td>
                        <td style="width: 10%; text-align: right"><%# Eval("licencado") %></td>
                        <td style="width: 10%; text-align: center"><%# Eval("pripravnik") %></td>
                        <td style="width: 10%; text-align: center"><%# Eval("vodic") %></td>
                        <td style="width: 10%; text-align: center"><%# Eval("instruktorkandidat") %></td>
                        <td style="width: 10%; text-align: center"><%# Eval("instruktor") %></td>
                        <td style="width: 10%; text-align: center"><%# Eval("clanarina") %></td>
                    </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <asp:hiddenfield id="hdnID" runat="server"/>
        <table style="border-collapse: collapse; margin-top: 10px" id="toolbar">
            <tr>
                <td style="padding: 0px">
                    <button type="button" id="btnPrva" runat="server" onserverclick="btnPrva_Click"><img src="img/first.png" class="pager"/></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnPrethodna" runat="server" onserverclick="btnPrethodna_Click"><img src="img/back.png" class="pager"/></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnStanje" runat="server" style="width: 78px"><% = (pageCurrent + 1).ToString() + " / " + pageCount.ToString() %></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnIduca" runat="server" onserverclick="btnIduca_Click"><img src="img/next.png" class="pager"/></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnZadnja" runat="server" onserverclick="btnZadnja_Click"><img src="img/last.png" class="pager"/></button>
                </td>
                <td style="padding: 0px; width: 15px"/>
                <td style="padding: 0px">
                    <button type="button" id="btnDodaj" runat="server" onserverclick="btnCommand_Click"><img src="img/add.png" class="action"/>Dodaj</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnIzmijeni" runat="server" onserverclick="btnCommand_Click"><img src="img/edit2.png" class="action"/>Izmijeni</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnObrisi" runat="server" onserverclick="btnCommand_Click"><img src="img/delete.png" class="action"/>Obriši</button>
                </td>
                <td style="padding: 0px; width: 15px"/>
                <td style="padding: 0px">
                    <button type="button" id="btnTrazi" runat="server" onserverclick="btnCommand_Click"><img src="img/find.png" class="action"/>Traži</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnFiltriraj" runat="server" onserverclick="btnCommand_Click"><img src="img/filter.png" class="action"/>Filtriraj</button>
                </td>
                <td style="padding: 0px; width: 15px"/>
                <td style="padding: 0px">
                    <button type="button" id="btnIspisi" runat="server" onserverclick="btnIspisi_Click"><img src="img/print.png" class="action"/>Ispiši</button>
                </td>
            </tr>
        </table>
        <% if ((action == "Dodaj") || ((action == "Izmijeni") && (hdnID.Value != "")) || (action == "Traži") || (action == "Filtriraj")) { %>    
        <div style="height: 1px; border-top: 1px solid black; margin-top: 5px; padding-top: 5px"></div>
        <h2><% = action %></h2>
        <table>
                <tr>
                    <td>
                        <asp:Label Text="Ime:" AssociatedControlID="txtIme" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtIme" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtImeDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                    <% if (!(action == "Traži" || action == "Filtriraj")) { %>
                    <td rowspan="18" style="padding-left: 32px">
                        <asp:Label Text="Fotografija:" AssociatedControlID="" runat="server" /><br/>
                        <asp:Image ImageUrl="foto/prazna.jpg" ID="imgFotografija" runat="server"/><br/>
                        <asp:FileUpload ID="fuFotografija" runat="server" /><br/>
                        <br/>
                        <button type="button" id="btnbtnUcitajFotografiju" runat="server" onserverclick="btnUcitajFotografiju_Click" default="default">Učitaj</button>
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Prezime:" AssociatedControlID="txtPrezime" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtPrezime" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtPrezimeDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Spol:" AssociatedControlID="txtSpol" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtSpol" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtSpolDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="OIB:" AssociatedControlID="txtOIB" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtOIB" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtOIBDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Broj značke:" AssociatedControlID="txtBrojZnacke" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtBrojZnacke" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtBrojZnackeDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Udruga:" AssociatedControlID="ddlUdruga" runat="server" />
                    </td>
                    <% if (!(action == "Traži" || action == "Filtriraj")) { %>
                    <td>
                        <asp:DropDownList ID="ddlUdruga" runat="server" AppendDataBoundItems="true">
                        </asp:DropDownList>
                    </td>
                    <% } %>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtUdruga" runat="server" onFocus="this.select()" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtUdrugaDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Ulica:" AssociatedControlID="txtUlica" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtUlica" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtUlicaDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Kućni broj:" AssociatedControlID="txtKucniBroj" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtKucniBroj" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtKucniBrojDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Pošta:" AssociatedControlID="ddlPosta" runat="server" />
                    </td>
                    <% if (!(action == "Traži" || action == "Filtriraj")) { %>
                    <td>
                        <asp:DropDownList ID="ddlPosta" runat="server" AppendDataBoundItems="true">
                        </asp:DropDownList>
                    </td>
                    <% } %>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtPosta" runat="server" onFocus="this.select()" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtPostaDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Mjesto:" AssociatedControlID="ddlMjesto" runat="server" />
                    </td>
                    <% if (!(action == "Traži" || action == "Filtriraj")) { %>
                    <td>
                        <asp:DropDownList ID="ddlMjesto" runat="server" AppendDataBoundItems="true">
                        </asp:DropDownList>
                    </td>
                    <% } %>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtMjesto" runat="server" onFocus="this.select()" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtMjestoDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="E-mail:" AssociatedControlID="txtEmail" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtEmail" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtEmailDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Telefon:" AssociatedControlID="txtTelefon" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtTelefon" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtTelefonDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Datum rođenja:" AssociatedControlID="txtDatumRodjenja" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtDatumRodjenja" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtDatumRodjenjaDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Datum smrti:" AssociatedControlID="txtDatumSmrti" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtDatumSmrti" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtDatumSmrtiDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Veličina majice:" AssociatedControlID="txtVelicinaMajice" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtVelicinaMajice" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtVelicinaMajiceDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Licenca važi do:" AssociatedControlID="txtLicencaVaziDo" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtLicencaVaziDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtLicencaVaziDoDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Izvor:" AssociatedControlID="txtIzvor" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtIzvor" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtIzvorDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Napomena:" AssociatedControlID="txtNapomena" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtNapomena" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtNapomenaDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Primjedba:" AssociatedControlID="txtPrimjedba" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtPrimjedba" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtPrimjedbaDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <% if (action == "Traži" || action == "Filtriraj") { %>
                <tr>
                    <td>
                        <asp:Label Text="Pripravnik:" AssociatedControlID="txtPripravnik" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtPripravnik" runat="server" onFocus="this.select()" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtPripravnikDo" runat="server" onFocus="this.select()" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Vodič:" AssociatedControlID="txtVodic" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtVodic" runat="server" onFocus="this.select()" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtVodicDo" runat="server" onFocus="this.select()" />
                    </td>
                </tr>
                <% } %>
                <tr>
                    <td>
                        <asp:Label Text="Instruktor kandidat:" AssociatedControlID="txtInstruktorKandidat" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtInstruktorKandidat" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtInstruktorKandidatDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Instruktor:" AssociatedControlID="txtInstruktor" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtInstruktor" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtInstruktorDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>            
                <tr>
                    <td>
                        <asp:Label Text="Članarina:" AssociatedControlID="txtClanarina" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtClanarina" runat="server" onFocus="this.select()" />
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtClanarinaDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>            
        </table>    
        <% } %>
        <% if ((action == "Obriši") && (hdnID.Value != "")) { %>
        <div style="height: 1px; border-top: 1px solid black; margin-top: 5px; padding-top: 5px"></div>
        <h2><% = action %></h2>
        Jeste li sigurni da zaista želite obrisati zapis?
        <% } %>
        <% if ((action == "Dodaj") || ((action == "Izmijeni") && (hdnID.Value != "")) || ((action == "Obriši") && (hdnID.Value != "")) || (action == "Traži") || (action == "Filtriraj")) { %>
        <table style="border-collapse: collapse; margin-top: 5px" id="Table1">
            <tr>
                <td style="padding: 0px">
                    <button type="button" id="btnPotvrdi" runat="server" onserverclick="btnPotvrdi_Click" default="default"><img src="img/check.png" class="action"/>Potvrdi</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnOdustani" runat="server" onserverclick="btnOdustani_Click"><img src="img/cancel.png" class="action"/>Odustani</button>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label Text="" ID="lblGreska" runat="server" />
                </td>
            </tr>
        </table>
        <% } %>
        <% if (hdnID.Value != "") { %>
        <script>grdMain_Click(<% = hdnID.Value %>);</script>
        <% } %>    
    </div>
</asp:Content>