<%@ Page Title="" Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeBehind="ispiti.aspx.cs" Inherits="spvv.wfIspiti" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var mainID = '';
        function grdMain_Click(id) {
            if (mainID != '' && document.all['trMain' + mainID] != null) {
                document.all['trMain' + mainID].classList.remove('selectedrow');
            }
            if (mainID != id) {
                if (document.all['trMain' + id] != null) {
                    document.all['trMain' + id].classList.add('selectedrow');
                }
                mainID = id;
            }
            else {
                mainID = '';
            }
            document.all['cphGlavni_hdnID'].value = mainID;
        }
        var instruktoriID = '';
        function grdInstruktori_Click(id) {
            if (instruktoriID != '' && document.all['trInstruktori' + instruktoriID] != null) {
                document.all['trInstruktori' + instruktoriID].classList.remove('selectedrow');
            }
            if (instruktoriID != id) {
                if (document.all['trInstruktori' + id] != null) {
                    document.all['trInstruktori' + id].classList.add('selectedrow');
                }
                instruktoriID = id;
            }
            else {
                instruktoriID = '';
            }
            document.all['cphGlavni_hdnInstruktoriID'].value = instruktoriID;
        }
        var kandidatiID = '';
        function grdKandidati_Click(id) {
            if (kandidatiID != '' && document.all['trKandidati' + kandidatiID] != null) {
                document.all['trKandidati' + kandidatiID].classList.remove('selectedrow');
            }
            if (kandidatiID != id) {
                if (document.all['trKandidati' + id] != null) {
                    document.all['trKandidati' + id].classList.add('selectedrow');
                }
                kandidatiID = id;
            }
            else {
                kandidatiID = '';
            }
            document.all['cphGlavni_hdnKandidatiID'].value = kandidatiID;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphGlavni" runat="server">
    <div style="margin: auto; padding: 20px; border: 1px solid #A0A0A0; border-radius: 10px; background: white; width: 100%; height: 100%; box-sizing: border-box">
        <% if ((action == "") || (((action == "Izmijeni") || (action == "Obriši") || (action == "Kandidati") || (action == "Instruktori")) && (hdnID.Value == "")) && (actionInstruktori == "") && (actionKandidati == "")) { %>
        <table style="border-collapse: collapse; width: 100%"><tr><td style="font-size: 2em">Ispiti</td><td style="text-align: right"><button type="button" id="Button4" class="menu" style="margin-left: auto; font-weight: bold; padding: 10px; width: 44px; line-height: 20px" onclick="window.location='default.aspx'">X</button></td></tr></table>
        <br/>
        <asp:Repeater ID="rptMain" runat="server">
            <HeaderTemplate>
                <table style="border-collapse: collapse; width: 100%" class="grid" id="rptMain">
                    <tr class="header">
                        <th style="width: 10%">Datum</th>
                        <th style="width: 40%">Naziv</th>
                        <th style="width: 20%">Standard</th>
                        <th style="width: 30%">Voditelj</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                    <tr class="row" id="trMain<%# Eval("id") %>" <% if (!((action == "Instruktori" || action == "Kandidati") && hdnID.Value != "")) { %>onclick="grdMain_Click(<%# Eval("id") %>)"<% } %>>
                        <td style="width: 10%; text-align: right"><%# string.Format("{0:d.M.yyyy.}", Eval("datum")) %></td>
                        <td style="width: 40%"><%# Eval("naziv") %></td>
                        <td style="width: 20%; text-align: center"><%# Eval("standard") %></td>
                        <td style="width: 30%"><%# Eval("voditelj") %></td>
                    </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                    <tr class="altrow" id="trMain<%# Eval("id") %>" <% if (!((action == "Instruktori" || action == "Kandidati") && hdnID.Value != "")) { %>onclick="grdMain_Click(<%# Eval("id") %>)"<% } %>>
                        <td style="width: 10%; text-align: right"><%# string.Format("{0:d.M.yyyy.}", Eval("datum")) %></td>
                        <td style="width: 40%"><%# Eval("naziv") %></td>
                        <td style="width: 20%; text-align: center"><%# Eval("standard") %></td>
                        <td style="width: 30%"><%# Eval("voditelj") %></td>
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
                    <button type="button" id="Button6" runat="server" onserverclick="btnCommand_Click"><img src="img/instruktori.png" class="action"/>Instruktori</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="Button7" runat="server" onserverclick="btnCommand_Click"><img src="img/tecajci.png" class="action"/>Kandidati</button>
                </td>
                <td style="padding: 0px; width: 15px"/>
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
        <% } %>
        <% if ((action == "Dodaj") || ((action == "Izmijeni") && (hdnID.Value != "")) || (action == "Traži") || (action == "Filtriraj")) { %>    
        <span style="font-size: 2em"><% = action %></span><br/>
        <br/>
        <table>
                <tr>
                    <td>
                        <asp:Label Text="Naziv:" AssociatedControlID="txtNaziv" runat="server" />
                    </td>
                    <td style="padding-left: 20px">
                        <asp:TextBox ID="txtNaziv" runat="server" onFocus="this.select()" style="width: 300px"/>
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtNazivDo" runat="server" onFocus="this.select()" style="width: 300px"/>
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Datum:" AssociatedControlID="txtDatum" runat="server" />
                    </td>
                    <td style="padding-left: 20px">
                        <asp:TextBox ID="txtDatum" runat="server" onFocus="this.select()" style="width: 90px"/>
                    </td>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtDatumDo" runat="server" onFocus="this.select()" style="width: 90px"/>
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Standard:" AssociatedControlID="ddlStandard" runat="server" />
                    </td>
                    <% if (!(action == "Traži" || action == "Filtriraj")) { %>
                    <td style="padding-left: 20px">
                        <asp:DropDownList ID="ddlStandard" runat="server" AppendDataBoundItems="true">
                        </asp:DropDownList><br/>
                    </td>
                    <% } %>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td style="padding-left: 20px">
                        <asp:TextBox ID="txtStandard" runat="server" onFocus="this.select()" style="width: 50px"/>
                    </td>
                    <td>
                        <asp:TextBox ID="txtStandardDo" runat="server" onFocus="this.select()" style="width: 50px"/>
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Voditelj:" AssociatedControlID="ddlVoditelj" runat="server" />
                    </td>
                    <% if (!(action == "Traži" || action == "Filtriraj")) { %>
                    <td style="padding-left: 20px">
                        <asp:DropDownList ID="ddlVoditelj" runat="server" AppendDataBoundItems="true">
                        </asp:DropDownList><br/>
                    </td>
                    <% } %>
                    <% if (action == "Traži" || action == "Filtriraj") { %>
                    <td style="padding-left: 20px">
                        <asp:TextBox ID="txtVoditelj" runat="server" onFocus="this.select()" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtVoditeljDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
        </table>    
        <% } %>
        <% if ((action == "Obriši") && (hdnID.Value != "")) { %>
        <span style="font-size: 2em"><% = action %></span><br/>
        <br/>
        Jeste li sigurni da zaista želite obrisati zapis?<br/>
        <% } %>
        <% if ((action == "Dodaj") || ((action == "Izmijeni") && (hdnID.Value != "")) || ((action == "Obriši") && (hdnID.Value != "")) || (action == "Traži") || (action == "Filtriraj")) { %>
        <br/>
        <table style="border-collapse: collapse; margin-top: 5px" id="Table1">
            <tr>
                <td style="padding: 0px">
                    <button type="button" id="btnPotvrdi" runat="server" onserverclick="btnPotvrdi_Click" default="default" style="width: 100px"><img src="img/check.png" class="action"/>Potvrdi</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnOdustani" runat="server" onserverclick="btnOdustani_Click" style="width: 100px"><img src="img/cancel.png" class="action"/>Odustani</button>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label Text="" ID="lblGreska" runat="server" />
                </td>
            </tr>
        </table>
        <% } %>
        <% if (((action == "Instruktori") && (hdnID.Value != "")) || (((actionInstruktori == "Izmijeni") || (actionInstruktori == "Obriši")) && (hdnInstruktoriID.Value == ""))) { %>
        <table style="border-collapse: collapse; width: 100%"><tr><td style="font-size: 2em"><% = action %></td><td style="text-align: right"><button type="button" id="Button5" class="menu" style="margin-left: auto; font-weight: bold; padding: 10px; width: 44px; line-height: 20px" runat="server" onserverclick="btnKandidatiZatvori_Click">X</button></td></tr></table>
        <br/>
        <asp:Repeater ID="rptInstruktori" runat="server">
            <HeaderTemplate>
                <table style="border-collapse: collapse; width: 100%" class="grid">
                    <tr class="header">
                        <th style="width: 500px">Instruktor</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                    <tr class="row" id="trInstruktori<%# Eval("id") %>" onclick="grdInstruktori_Click(<%# Eval("id") %>)">
                        <td style="width: 100%"><%# Eval("instruktor") %></td>
                    </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                    <tr class="altrow" id="trInstruktori<%# Eval("id") %>" onclick="grdInstruktori_Click(<%# Eval("id") %>)">
                        <td style="width: 100%"><%# Eval("instruktor") %></td>
                    </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <asp:hiddenfield id="hdnInstruktoriID" runat="server"/>
        <table style="border-collapse: collapse; margin-top: 10px" id="toolbar">
            <tr>
                <td style="padding: 0px">
                    <button type="button" id="btnInstruktoriPrva" runat="server" onserverclick="btnInstruktoriPrva_Click"><img src="img/first.png" class="pager"/></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnInstruktoriPrethodna" runat="server" onserverclick="btnInstruktoriPrethodna_Click"><img src="img/back.png" class="pager"/></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnInstruktoriStanje" runat="server" style="width: 78px"><% = (pageCurrentInstruktori + 1).ToString() + " / " + pageCountInstruktori.ToString() %></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnInstruktoriIduca" runat="server" onserverclick="btnInstruktoriIduca_Click"><img src="img/next.png" class="pager"/></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnInstruktoriZadnja" runat="server" onserverclick="btnInstruktoriZadnja_Click"><img src="img/last.png" class="pager"/></button>
                </td>
                <td style="padding: 0px; width: 15px"/>
                <td style="padding: 0px">
                    <button type="button" id="btnInstruktoriDodaj" runat="server" onserverclick="btnInstruktoriCommand_Click"><img src="img/add.png" class="action"/>Dodaj</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnInstruktoriIzmijeni" runat="server" onserverclick="btnInstruktoriCommand_Click"><img src="img/edit2.png" class="action"/>Izmijeni</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnInstruktoriObrisi" runat="server" onserverclick="btnInstruktoriCommand_Click"><img src="img/delete.png" class="action"/>Obriši</button>
                </td>
                <td style="padding: 0px; width: 15px"/>
                <td style="padding: 0px">
                    <button type="button" id="btnInstruktoriTrazi" runat="server" onserverclick="btnInstruktoriCommand_Click"><img src="img/find.png" class="action"/>Traži</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnInstruktoriFiltriraj" runat="server" onserverclick="btnInstruktoriCommand_Click"><img src="img/filter.png" class="action"/>Filtriraj</button>
                </td>
                <td style="padding: 0px; width: 15px"/>
                <td style="padding: 0px">
                    <button type="button" id="btnInstruktoriIspisi" runat="server" onserverclick="btnInstruktoriIspisi_Click"><img src="img/print.png" class="action"/>Ispiši</button>
                </td>
            </tr>
        </table>
        <% } %>
        <% if ((actionInstruktori == "Dodaj") || ((actionInstruktori == "Izmijeni") && (hdnID.Value != "")) || (actionInstruktori == "Traži") || (actionInstruktori == "Filtriraj")) { %>    
        <span style="font-size: 2em"><% = actionInstruktori %></span><br/>
        <br/>
        <table>
                <tr>
                    <td>
                        <asp:Label Text="Instruktor:" AssociatedControlID="ddlInstruktor" runat="server" />
                    </td>
                    <% if (!(actionInstruktori == "Traži" || actionInstruktori == "Filtriraj")) { %>
                    <td style="padding-left: 20px">
                        <asp:DropDownList ID="ddlInstruktor" runat="server" AppendDataBoundItems="true">
                        </asp:DropDownList><br/>
                    </td>
                    <% } %>
                    <% if (actionInstruktori == "Traži" || actionInstruktori == "Filtriraj") { %>
                    <td style="padding-left: 20px">
                        <asp:TextBox ID="txtInstruktor" runat="server" onFocus="this.select()" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtInstruktorDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
        </table>    
        <% } %>
        <% if ((actionInstruktori == "Obriši") && (hdnInstruktoriID.Value != "")) { %>
        <span style="font-size: 2em"><% = actionInstruktori %></span><br/>
        <br/>
        Jeste li sigurni da zaista želite obrisati zapis?<br/>
        <% } %>
        <% if ((actionInstruktori == "Dodaj") || ((actionInstruktori == "Izmijeni") && (hdnInstruktoriID.Value != "")) || ((actionInstruktori == "Obriši") && (hdnInstruktoriID.Value != "")) || (actionInstruktori == "Traži") || (actionInstruktori == "Filtriraj")) { %>
        <br/>
        <table style="border-collapse: collapse; margin-top: 5px" id="Table1">
            <tr>
                <td style="padding: 0px">
                    <button type="button" id="Button1" runat="server" onserverclick="btnInstruktoriPotvrdi_Click" default="default"><img src="img/check.png" class="actionInstruktori"/>Potvrdi</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="Button2" runat="server" onserverclick="btnInstruktoriOdustani_Click"><img src="img/cancel.png" class="actionInstruktori"/>Odustani</button>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label Text="" ID="lblInstruktoriGreska" runat="server" />
                </td>
            </tr>
        </table>
        <% } %>

        <% if (((action == "Kandidati") && (hdnID.Value != "")) || (((actionKandidati == "Izmijeni") || (actionKandidati == "Obriši")) && (hdnKandidatiID.Value == ""))) { %>
        <table style="border-collapse: collapse; width: 100%"><tr><td style="font-size: 2em"><% = action %></td><td style="text-align: right"><button type="button" id="Button3" class="menu" style="margin-left: auto; font-weight: bold; padding: 10px; width: 44px; line-height: 20px" runat="server" onserverclick="btnKandidatiZatvori_Click">X</button></td></tr></table>
        <br/>
        <asp:Repeater ID="rptKandidati" runat="server">
            <HeaderTemplate>
                <table style="border-collapse: collapse; width: 100%" class="grid">
                    <tr class="header">
                        <th style="width: 90%">Kandidat</th>
                        <th style="width: 10%">Položio</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                    <tr class="row" id="trKandidati<%# Eval("id") %>" onclick="grdKandidati_Click(<%# Eval("id") %>)">
                        <td style="width: 90%"><%# Eval("kandidat") %></td>
                        <td style="width: 10%; text-align: center"><%# (Boolean.Parse(Eval("polozio").ToString())) ? "<img src=\"img/check.png\"/>" : "" %></td>
                    </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                    <tr class="altrow" id="trKandidati<%# Eval("id") %>" onclick="grdKandidati_Click(<%# Eval("id") %>)">
                        <td style="width: 90%"><%# Eval("kandidat") %></td>
                        <td style="width: 10%; text-align: center"><%# (Boolean.Parse(Eval("polozio").ToString())) ? "<img src=\"img/check.png\"/>" : "" %></td>
                    </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <asp:hiddenfield id="hdnKandidatiID" runat="server"/>
        <table style="border-collapse: collapse; margin-top: 10px" id="toolbar">
            <tr>
                <td style="padding: 0px">
                    <button type="button" id="btnKandidatiPrva" runat="server" onserverclick="btnKandidatiPrva_Click"><img src="img/first.png" class="pager"/></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnKandidatiPrethodna" runat="server" onserverclick="btnKandidatiPrethodna_Click"><img src="img/back.png" class="pager"/></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnKandidatiStanje" runat="server" style="width: 78px"><% = (pageCurrentKandidati + 1).ToString() + " / " + pageCountKandidati.ToString() %></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnKandidatiIduca" runat="server" onserverclick="btnKandidatiIduca_Click"><img src="img/next.png" class="pager"/></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnKandidatiZadnja" runat="server" onserverclick="btnKandidatiZadnja_Click"><img src="img/last.png" class="pager"/></button>
                </td>
                <td style="padding: 0px; width: 15px"/>
                <td style="padding: 0px">
                    <button type="button" id="btnKandidatiDodaj" runat="server" onserverclick="btnKandidatiCommand_Click"><img src="img/add.png" class="action"/>Dodaj</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnKandidatiIzmijeni" runat="server" onserverclick="btnKandidatiCommand_Click"><img src="img/edit2.png" class="action"/>Izmijeni</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnKandidatiObrisi" runat="server" onserverclick="btnKandidatiCommand_Click"><img src="img/delete.png" class="action"/>Obriši</button>
                </td>
                <td style="padding: 0px; width: 15px"/>
                <td style="padding: 0px">
                    <button type="button" id="btnKandidatiTrazi" runat="server" onserverclick="btnKandidatiCommand_Click"><img src="img/find.png" class="action"/>Traži</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnKandidatiFiltriraj" runat="server" onserverclick="btnKandidatiCommand_Click"><img src="img/filter.png" class="action"/>Filtriraj</button>
                </td>
                <td style="padding: 0px; width: 15px"/>
                <td style="padding: 0px">
                    <button type="button" id="btnKandidatiIspisi" runat="server" onserverclick="btnKandidatiIspisi_Click"><img src="img/print.png" class="action"/>Ispiši</button>
                </td>
            </tr>
        </table>
        <% } %>
        <% if ((actionKandidati == "Dodaj") || ((actionKandidati == "Izmijeni") && (hdnID.Value != "")) || (actionKandidati == "Traži") || (actionKandidati == "Filtriraj")) { %>    
        <span style="font-size: 2em"><% = actionKandidati %></span><br/>
        <br/>
        <table>
                <tr>
                    <td>
                        <asp:Label Text="Kandidat:" AssociatedControlID="ddlKandidat" runat="server" />
                    </td>
                    <% if (!(actionKandidati == "Traži" || actionKandidati == "Filtriraj")) { %>
                    <td style="padding-left: 20px">
                        <asp:DropDownList ID="ddlKandidat" runat="server" AppendDataBoundItems="true">
                        </asp:DropDownList><br/>
                    </td>
                    <% } %>
                    <% if (actionKandidati == "Traži" || actionKandidati == "Filtriraj") { %>
                    <td style="padding-left: 20px">
                        <asp:TextBox ID="txtKandidat" runat="server" onFocus="this.select()" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtKandidatDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Položio:" AssociatedControlID="cbPolozio" runat="server" />
                    </td>
                    <td style="padding-left: 20px">
                        <asp:checkbox id="cbPolozio" runat="server"/>
                    </td>
                </tr>
        </table>    
        <% } %>
        <% if ((actionKandidati == "Obriši") && (hdnKandidatiID.Value != "")) { %>
        <span style="font-size: 2em"><% = actionKandidati %></span><br/>
        <br/>
        Jeste li sigurni da zaista želite obrisati zapis?<br/>
        <% } %>
        <% if ((actionKandidati == "Dodaj") || ((actionKandidati == "Izmijeni") && (hdnKandidatiID.Value != "")) || ((actionKandidati == "Obriši") && (hdnKandidatiID.Value != "")) || (actionKandidati == "Traži") || (actionKandidati == "Filtriraj")) { %>
        <br/>
        <table style="border-collapse: collapse; margin-top: 5px" id="Table1">
            <tr>
                <td style="padding: 0px">
                    <button type="button" id="Button15" runat="server" onserverclick="btnKandidatiPotvrdi_Click" default="default"><img src="img/check.png" class="actionKandidati"/>Potvrdi</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="Button16" runat="server" onserverclick="btnKandidatiOdustani_Click"><img src="img/cancel.png" class="actionKandidati"/>Odustani</button>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label Text="" ID="lblKandidatiGreska" runat="server" />
                </td>
            </tr>
        </table>
        <% } %>

        <% if (hdnID.Value != "") { %>
        <script>grdMain_Click(<% = hdnID.Value %>);</script>
        <% } %>    
        <% if (hdnInstruktoriID.Value != "") { %>
        <script>grdInstruktori_Click(<% = hdnInstruktoriID.Value %>);</script>
        <% } %>    
        <% if (hdnKandidatiID.Value != "") { %>
        <script>grdKandidati_Click(<% = hdnKandidatiID.Value %>);</script>
        <% } %>
    </div>
</asp:Content>