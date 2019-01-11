<%@ Page Title="" Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeBehind="tecajevi.aspx.cs" Inherits="spvv.wfTecajevi" %>
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
        var tecajciID = '';
        function grdTecajci_Click(id) {
            if (tecajciID != '' && document.all['trTecajci' + tecajciID] != null) {
                document.all['trTecajci' + tecajciID].classList.remove('selectedrow');
            }
            if (tecajciID != id) {
                if (document.all['trTecajci' + id] != null) {
                    document.all['trTecajci' + id].classList.add('selectedrow');
                }
                tecajciID = id;
            }
            else {
                tecajciID = '';
            }
            document.all['cphGlavni_hdnTecajciID'].value = tecajciID;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphGlavni" runat="server">
    <h1>Tečajevi</h1>
    <asp:Repeater ID="rptMain" runat="server">
        <HeaderTemplate>
            <table style="border-collapse: collapse; width: 768px" class="grid">
                <tr class="header">
                    <th style="width: 80px">Datum</th>
                    <th style="width: 412px">Naziv</th>
                    <th style="width: 70px">Standard</th>
                    <th style="width: 221px">Voditelj</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
                <tr class="row" id="trMain<%# Eval("id") %>" <% if (!((action == "Instruktori" || action == "Tečajci") && hdnID.Value != "")) { %>onclick="grdMain_Click(<%# Eval("id") %>)"<% } %>>
                    <td style="width: 80px; text-align: right"><%# string.Format("{0:d.M.yyyy.}", Eval("datum")) %></td>
                    <td style="width: 412px"><%# Eval("naziv") %></td>
                    <td style="width: 70px; text-align: center"><%# Eval("standard") %></td>
                    <td style="width: 221px"><%# Eval("voditelj") %></td>
                </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
                <tr class="altrow" id="trMain<%# Eval("id") %>" <% if (!((action == "Instruktori" || action == "Tečajci") && hdnID.Value != "")) { %>onclick="grdMain_Click(<%# Eval("id") %>)"<% } %>>
                    <td style="width: 80px; text-align: right"><%# string.Format("{0:d.M.yyyy.}", Eval("datum")) %></td>
                    <td style="width: 412px"><%# Eval("naziv") %></td>
                    <td style="width: 70px; text-align: center"><%# Eval("standard") %></td>
                    <td style="width: 221px"><%# Eval("voditelj") %></td>
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
    <table style="border-collapse: collapse; margin-top: 10px" id="toolbar2">
        <tr>
            <td style="padding: 0px">
                <button type="button" id="btnInstruktori" runat="server" onserverclick="btnCommand_Click"><img src="img/instruktori.png" class="action"/>Instruktori</button>
            </td>
            <td style="padding: 0px">
                <button type="button" id="btnTečajci" runat="server" onserverclick="btnCommand_Click"><img src="img/tecajci.png" class="action"/>Tečajci</button>
            </td>
        </tr>
    </table>
    <% if ((action == "Dodaj") || ((action == "Izmijeni") && (hdnID.Value != "")) || (action == "Traži") || (action == "Filtriraj")) { %>    
    <div style="height: 1px; border-top: 1px solid black; margin-top: 5px; padding-top: 5px"></div>
    <h2><% = action %></h2>
    <table>
            <tr>
                <td>
                    <asp:Label Text="Naziv:" AssociatedControlID="txtNaziv" runat="server" />
                </td>
                <td>
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
                <td>
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
                <td>
                    <asp:DropDownList ID="ddlStandard" runat="server" AppendDataBoundItems="true">
                    </asp:DropDownList><br/>
                </td>
                <% } %>
                <% if (action == "Traži" || action == "Filtriraj") { %>
                <td>
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
                <td>
                    <asp:DropDownList ID="ddlVoditelj" runat="server" AppendDataBoundItems="true">
                    </asp:DropDownList><br/>
                </td>
                <% } %>
                <% if (action == "Traži" || action == "Filtriraj") { %>
                <td>
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
    <% if ((action == "Instruktori") && (hdnID.Value != "")) { %>    
    <div style="height: 1px; border-top: 1px solid black; margin-top: 5px; padding-top: 5px"></div>
    <h2><% = action %></h2>
    <asp:Repeater ID="rptInstruktori" runat="server">
        <HeaderTemplate>
            <table style="border-collapse: collapse; width: 768px" class="grid">
                <tr class="header">
                    <th style="width: 500px">Instruktor</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
                <tr class="row" id="trInstruktori<%# Eval("id") %>" onclick="grdInstruktori_Click(<%# Eval("id") %>)">
                    <td style="width: 500px"><%# Eval("instruktor") %></td>
                </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
                <tr class="altrow" id="trInstruktori<%# Eval("id") %>" onclick="grdInstruktori_Click(<%# Eval("id") %>)">
                    <td style="width: 500px"><%# Eval("instruktor") %></td>
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
    <table style="border-collapse: collapse; margin-top: 10px" id="toolbar2">
        <tr>
            <td style="padding: 0px">
                <button type="button" id="btnInstruktoriZatvori" runat="server" onserverclick="btnInstruktoriZatvori_Click"><img src="img/cancel.png" class="action"/>Zatvori</button>
            </td>
        </tr>
    </table>
    <% } %>
    <% if ((actionInstruktori == "Dodaj") || ((actionInstruktori == "Izmijeni") && (hdnID.Value != "")) || (actionInstruktori == "Traži") || (actionInstruktori == "Filtriraj")) { %>    
    <div style="height: 1px; border-top: 1px solid black; margin-top: 5px; padding-top: 5px"></div>
    <h2><% = actionInstruktori %></h2>
    <table>
            <tr>
                <td>
                    <asp:Label Text="Instruktor:" AssociatedControlID="ddlInstruktor" runat="server" />
                </td>
                <% if (!(actionInstruktori == "Traži" || actionInstruktori == "Filtriraj")) { %>
                <td>
                    <asp:DropDownList ID="ddlInstruktor" runat="server" AppendDataBoundItems="true">
                    </asp:DropDownList><br/>
                </td>
                <% } %>
                <% if (actionInstruktori == "Traži" || actionInstruktori == "Filtriraj") { %>
                <td>
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
    <div style="height: 1px; border-top: 1px solid black; margin-top: 5px; padding-top: 5px"></div>
    <h2><% = actionInstruktori %></h2>
    Jeste li sigurni da zaista želite obrisati zapis?
    <% } %>
    <% if ((actionInstruktori == "Dodaj") || ((actionInstruktori == "Izmijeni") && (hdnID.Value != "")) || ((actionInstruktori == "Obriši") && (hdnID.Value != "")) || (actionInstruktori == "Traži") || (actionInstruktori == "Filtriraj")) { %>
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

    <% if ((action == "Tečajci") && (hdnID.Value != "")) { %>    
    <div style="height: 1px; border-top: 1px solid black; margin-top: 5px; padding-top: 5px"></div>
    <h2><% = action %></h2>
    <asp:Repeater ID="rptTecajci" runat="server">
        <HeaderTemplate>
            <table style="border-collapse: collapse; width: 768px" class="grid">
                <tr class="header">
                    <th style="width: 500px">Tečajac</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
                <tr class="row" id="trTecajci<%# Eval("id") %>" onclick="grdTecajci_Click(<%# Eval("id") %>)">
                    <td style="width: 500px"><%# Eval("tecajac") %></td>
                </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
                <tr class="altrow" id="trTecajci<%# Eval("id") %>" onclick="grdTecajci_Click(<%# Eval("id") %>)">
                    <td style="width: 500px"><%# Eval("tecajac") %></td>
                </tr>
        </AlternatingItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <asp:hiddenfield id="hdnTecajciID" runat="server"/>
    <table style="border-collapse: collapse; margin-top: 10px" id="toolbar">
        <tr>
            <td style="padding: 0px">
                <button type="button" id="btnTecajciPrva" runat="server" onserverclick="btnTecajciPrva_Click"><img src="img/first.png" class="pager"/></button>
            </td>
            <td style="padding: 0px">
                <button type="button" id="btnTecajciPrethodna" runat="server" onserverclick="btnTecajciPrethodna_Click"><img src="img/back.png" class="pager"/></button>
            </td>
            <td style="padding: 0px">
                <button type="button" id="btnTecajciStanje" runat="server" style="width: 78px"><% = (pageCurrentTecajci + 1).ToString() + " / " + pageCountTecajci.ToString() %></button>
            </td>
            <td style="padding: 0px">
                <button type="button" id="btnTecajciIduca" runat="server" onserverclick="btnTecajciIduca_Click"><img src="img/next.png" class="pager"/></button>
            </td>
            <td style="padding: 0px">
                <button type="button" id="btnTecajciZadnja" runat="server" onserverclick="btnTecajciZadnja_Click"><img src="img/last.png" class="pager"/></button>
            </td>
            <td style="padding: 0px; width: 15px"/>
            <td style="padding: 0px">
                <button type="button" id="btnTecajciDodaj" runat="server" onserverclick="btnTecajciCommand_Click"><img src="img/add.png" class="action"/>Dodaj</button>
            </td>
            <td style="padding: 0px">
                <button type="button" id="btnTecajciIzmijeni" runat="server" onserverclick="btnTecajciCommand_Click"><img src="img/edit2.png" class="action"/>Izmijeni</button>
            </td>
            <td style="padding: 0px">
                <button type="button" id="btnTecajciObrisi" runat="server" onserverclick="btnTecajciCommand_Click"><img src="img/delete.png" class="action"/>Obriši</button>
            </td>
            <td style="padding: 0px; width: 15px"/>
            <td style="padding: 0px">
                <button type="button" id="btnTecajciTrazi" runat="server" onserverclick="btnTecajciCommand_Click"><img src="img/find.png" class="action"/>Traži</button>
            </td>
            <td style="padding: 0px">
                <button type="button" id="btnTecajciFiltriraj" runat="server" onserverclick="btnTecajciCommand_Click"><img src="img/filter.png" class="action"/>Filtriraj</button>
            </td>
            <td style="padding: 0px; width: 15px"/>
            <td style="padding: 0px">
                <button type="button" id="btnTecajciIspisi" runat="server" onserverclick="btnTecajciIspisi_Click"><img src="img/print.png" class="action"/>Ispiši</button>
            </td>
        </tr>
    </table>
    <table style="border-collapse: collapse; margin-top: 10px" id="toolbar2">
        <tr>
            <td style="padding: 0px">
                <button type="button" id="Button14" runat="server" onserverclick="btnTecajciZatvori_Click"><img src="img/cancel.png" class="action"/>Zatvori</button>
            </td>
        </tr>
    </table>
    <% } %>
    <% if ((actionTecajci == "Dodaj") || ((actionTecajci == "Izmijeni") && (hdnID.Value != "")) || (actionTecajci == "Traži") || (actionTecajci == "Filtriraj")) { %>    
    <div style="height: 1px; border-top: 1px solid black; margin-top: 5px; padding-top: 5px"></div>
    <h2><% = actionTecajci %></h2>
    <table>
            <tr>
                <td>
                    <asp:Label Text="Tečajac:" AssociatedControlID="ddlTecajac" runat="server" />
                </td>
                <% if (!(actionTecajci == "Traži" || actionTecajci == "Filtriraj")) { %>
                <td>
                    <asp:DropDownList ID="ddlTecajac" runat="server" AppendDataBoundItems="true">
                    </asp:DropDownList><br/>
                </td>
                <% } %>
                <% if (actionTecajci == "Traži" || actionTecajci == "Filtriraj") { %>
                <td>
                    <asp:TextBox ID="txtTecajac" runat="server" onFocus="this.select()" />
                </td>
                <td>
                    <asp:TextBox ID="txtTecajacDo" runat="server" onFocus="this.select()" />
                </td>
                <% } %>
            </tr>
    </table>    
    <% } %>
    <% if ((actionTecajci == "Obriši") && (hdnTecajciID.Value != "")) { %>
    <div style="height: 1px; border-top: 1px solid black; margin-top: 5px; padding-top: 5px"></div>
    <h2><% = actionTecajci %></h2>
    Jeste li sigurni da zaista želite obrisati zapis?
    <% } %>
    <% if ((actionTecajci == "Dodaj") || ((actionTecajci == "Izmijeni") && (hdnID.Value != "")) || ((actionTecajci == "Obriši") && (hdnID.Value != "")) || (actionTecajci == "Traži") || (actionTecajci == "Filtriraj")) { %>
    <table style="border-collapse: collapse; margin-top: 5px" id="Table1">
        <tr>
            <td style="padding: 0px">
                <button type="button" id="Button15" runat="server" onserverclick="btnTecajciPotvrdi_Click" default="default"><img src="img/check.png" class="actionTecajci"/>Potvrdi</button>
            </td>
            <td style="padding: 0px">
                <button type="button" id="Button16" runat="server" onserverclick="btnTecajciOdustani_Click"><img src="img/cancel.png" class="actionTecajci"/>Odustani</button>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label Text="" ID="lblTecajciGreska" runat="server" />
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
    <% if (hdnTecajciID.Value != "") { %>
    <script>grdTecajci_Click(<% = hdnTecajciID.Value %>);</script>
    <% } %>    
</asp:Content>