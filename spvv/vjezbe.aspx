<%@ Page Title="" Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeBehind="vjezbe.aspx.cs" Inherits="spvv.wfVjezbe" %>
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
        var vodiciID = '';
        function grdVodici_Click(id) {
            if (vodiciID != '' && document.all['trVodici' + vodiciID] != null) {
                document.all['trVodici' + vodiciID].classList.remove('selectedrow');
            }
            if (vodiciID != id) {
                if (document.all['trVodici' + id] != null) {
                    document.all['trVodici' + id].classList.add('selectedrow');
                }
                vodiciID = id;
            }
            else {
                vodiciID = '';
            }
            document.all['cphGlavni_hdnVodiciID'].value = vodiciID;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphGlavni" runat="server">
    <div style="margin: auto; padding: 20px; border: 1px solid #A0A0A0; border-radius: 10px; background: white; width: 100%; height: 100%; box-sizing: border-box">
        <table style="border-collapse: collapse; width: 100%"><tr><td style="font-size: 2em">Županije</td><td style="text-align: right"><button type="button" id="Button4" class="menu" style="margin-left: auto; font-weight: bold; padding: 10px; width: 44px; line-height: 20px" onclick="window.location='default.aspx'">X</button></td></tr></table>
        <br/>
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
                    <tr class="row" id="trMain<%# Eval("id") %>" <% if (!((action == "Instruktori" || action == "Vodiči") && hdnID.Value != "")) { %>onclick="grdMain_Click(<%# Eval("id") %>)"<% } %>>
                        <td style="width: 80px; text-align: right"><%# string.Format("{0:d.M.yyyy.}", Eval("datum")) %></td>
                        <td style="width: 412px"><%# Eval("naziv") %></td>
                        <td style="width: 70px; text-align: center"><%# Eval("standard") %></td>
                        <td style="width: 221px"><%# Eval("voditelj") %></td>
                    </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                    <tr class="altrow" id="trMain<%# Eval("id") %>" <% if (!((action == "Instruktori" || action == "Vodiči") && hdnID.Value != "")) { %>onclick="grdMain_Click(<%# Eval("id") %>)"<% } %>>
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
                    <button type="button" id="btnInstruktori" runat="server" onserverclick="btnCommand_Click"><img src="img/instruktori.png" class="action"/>Instruktori</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnVodiči" runat="server" onserverclick="btnCommand_Click"><img src="img/tecajci.png" class="action"/>Vodiči</button>
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

        <% if ((action == "Vodiči") && (hdnID.Value != "")) { %>    
        <div style="height: 1px; border-top: 1px solid black; margin-top: 5px; padding-top: 5px"></div>
        <h2><% = action %></h2>
        <asp:Repeater ID="rptVodici" runat="server">
            <HeaderTemplate>
                <table style="border-collapse: collapse; width: 768px" class="grid">
                    <tr class="header">
                        <th style="width: 500px">Vodič</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                    <tr class="row" id="trVodici<%# Eval("id") %>" onclick="grdVodici_Click(<%# Eval("id") %>)">
                        <td style="width: 500px"><%# Eval("vodic") %></td>
                    </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                    <tr class="altrow" id="trVodici<%# Eval("id") %>" onclick="grdVodici_Click(<%# Eval("id") %>)">
                        <td style="width: 500px"><%# Eval("vodic") %></td>
                    </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <asp:hiddenfield id="hdnVodiciID" runat="server"/>
        <table style="border-collapse: collapse; margin-top: 10px" id="toolbar">
            <tr>
                <td style="padding: 0px">
                    <button type="button" id="btnVodiciPrva" runat="server" onserverclick="btnVodiciPrva_Click"><img src="img/first.png" class="pager"/></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnVodiciPrethodna" runat="server" onserverclick="btnVodiciPrethodna_Click"><img src="img/back.png" class="pager"/></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnVodiciStanje" runat="server" style="width: 78px"><% = (pageCurrentVodici + 1).ToString() + " / " + pageCountVodici.ToString() %></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnVodiciIduca" runat="server" onserverclick="btnVodiciIduca_Click"><img src="img/next.png" class="pager"/></button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnVodiciZadnja" runat="server" onserverclick="btnVodiciZadnja_Click"><img src="img/last.png" class="pager"/></button>
                </td>
                <td style="padding: 0px; width: 15px"/>
                <td style="padding: 0px">
                    <button type="button" id="btnVodiciDodaj" runat="server" onserverclick="btnVodiciCommand_Click"><img src="img/add.png" class="action"/>Dodaj</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnVodiciIzmijeni" runat="server" onserverclick="btnVodiciCommand_Click"><img src="img/edit2.png" class="action"/>Izmijeni</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnVodiciObrisi" runat="server" onserverclick="btnVodiciCommand_Click"><img src="img/delete.png" class="action"/>Obriši</button>
                </td>
                <td style="padding: 0px; width: 15px"/>
                <td style="padding: 0px">
                    <button type="button" id="btnVodiciTrazi" runat="server" onserverclick="btnVodiciCommand_Click"><img src="img/find.png" class="action"/>Traži</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="btnVodiciFiltriraj" runat="server" onserverclick="btnVodiciCommand_Click"><img src="img/filter.png" class="action"/>Filtriraj</button>
                </td>
                <td style="padding: 0px; width: 15px"/>
                <td style="padding: 0px">
                    <button type="button" id="btnVodiciIspisi" runat="server" onserverclick="btnVodiciIspisi_Click"><img src="img/print.png" class="action"/>Ispiši</button>
                </td>
            </tr>
        </table>
        <table style="border-collapse: collapse; margin-top: 10px" id="toolbar2">
            <tr>
                <td style="padding: 0px">
                    <button type="button" id="Button14" runat="server" onserverclick="btnVodiciZatvori_Click"><img src="img/cancel.png" class="action"/>Zatvori</button>
                </td>
            </tr>
        </table>
        <% } %>
        <% if ((actionVodici == "Dodaj") || ((actionVodici == "Izmijeni") && (hdnID.Value != "")) || (actionVodici == "Traži") || (actionVodici == "Filtriraj")) { %>    
        <div style="height: 1px; border-top: 1px solid black; margin-top: 5px; padding-top: 5px"></div>
        <h2><% = actionVodici %></h2>
        <table>
                <tr>
                    <td>
                        <asp:Label Text="Vodič:" AssociatedControlID="ddlVodic" runat="server" />
                    </td>
                    <% if (!(actionVodici == "Traži" || actionVodici == "Filtriraj")) { %>
                    <td>
                        <asp:DropDownList ID="ddlVodic" runat="server" AppendDataBoundItems="true">
                        </asp:DropDownList><br/>
                    </td>
                    <% } %>
                    <% if (actionVodici == "Traži" || actionVodici == "Filtriraj") { %>
                    <td>
                        <asp:TextBox ID="txtVodic" runat="server" onFocus="this.select()" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtVodicDo" runat="server" onFocus="this.select()" />
                    </td>
                    <% } %>
                </tr>
        </table>    
        <% } %>
        <% if ((actionVodici == "Obriši") && (hdnVodiciID.Value != "")) { %>
        <div style="height: 1px; border-top: 1px solid black; margin-top: 5px; padding-top: 5px"></div>
        <h2><% = actionVodici %></h2>
        Jeste li sigurni da zaista želite obrisati zapis?
        <% } %>
        <% if ((actionVodici == "Dodaj") || ((actionVodici == "Izmijeni") && (hdnID.Value != "")) || ((actionVodici == "Obriši") && (hdnID.Value != "")) || (actionVodici == "Traži") || (actionVodici == "Filtriraj")) { %>
        <table style="border-collapse: collapse; margin-top: 5px" id="Table1">
            <tr>
                <td style="padding: 0px">
                    <button type="button" id="Button15" runat="server" onserverclick="btnVodiciPotvrdi_Click" default="default"><img src="img/check.png" class="actionVodici"/>Potvrdi</button>
                </td>
                <td style="padding: 0px">
                    <button type="button" id="Button16" runat="server" onserverclick="btnVodiciOdustani_Click"><img src="img/cancel.png" class="actionVodici"/>Odustani</button>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label Text="" ID="lblVodiciGreska" runat="server" />
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
        <% if (hdnVodiciID.Value != "") { %>
        <script>grdVodici_Click(<% = hdnVodiciID.Value %>);</script>
        <% } %>    
    </div>
</asp:Content>