<%@ Page Title="" Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeBehind="log.aspx.cs" Inherits="spvv.wfLog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphGlavni" runat="server">
    <h1>Log</h1>
    <asp:Repeater ID="rptMain" runat="server">
        <HeaderTemplate>
            <table style="border-collapse: collapse; width: 768px" class="grid">
                <tr class="header">
                    <th style="width: 150px">Vrijeme</th>
                    <th style="width: 75px">Korisnik</th>
                    <th style="width: 543px">Upit</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
                <tr class="row" id="trMain<%# Eval("id") %>">
                    <td style="width: 150px"><%# Eval("vrijeme") %></td>
                    <td style="width: 75px"><%# Eval("korisnik") %></td>
                    <td style="width: 543px"><%# Eval("upit") %></td>
                </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
                <tr class="altrow" id="trMain<%# Eval("id") %>">
                    <td style="width: 100px"><%# Eval("vrijeme") %></td>
                    <td style="width: 100px"><%# Eval("korisnik") %></td>
                    <td style="width: 543px"><%# Eval("upit") %></td>
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
                <button type="button" id="btnTrazi" runat="server" onserverclick="btnCommand_Click"><img src="img/find.png" class="action"/>Traži</button>
            </td>
            <td style="padding: 0px">
                <button type="button" id="btnFiltriraj" runat="server" onserverclick="btnCommand_Click"><img src="img/filter.png" class="action"/>Filtriraj</button>
            </td>
        </tr>
    </table>
    <% if ((action == "Dodaj") || ((action == "Izmijeni") && (hdnID.Value != "")) || (action == "Traži") || (action == "Filtriraj")) { %>    
    <div style="height: 1px; border-top: 1px solid black; margin-top: 5px; padding-top: 5px"></div>
    <h2><% = action %></h2>
    <table>
            <tr>
                <td>
                    <asp:Label Text="Vrijeme:" AssociatedControlID="txtVrijeme" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtVrijeme" runat="server" onFocus="this.select()" />
                </td>
                <% if (action == "Traži" || action == "Filtriraj") { %>
                <td>
                    <asp:TextBox ID="txtVrijemeDo" runat="server" onFocus="this.select()" />
                </td>
                <% } %>
            </tr>
            <tr>
                <td>
                    <asp:Label Text="Korisnik:" AssociatedControlID="txtKorisnik" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtKorisnik" runat="server" onFocus="this.select()" />
                </td>
                <% if (action == "Traži" || action == "Filtriraj") { %>
                <td>
                    <asp:TextBox ID="txtKorisnikDo" runat="server" onFocus="this.select()" />
                </td>
                <% } %>
            </tr>
            <tr>
                <td>
                    <asp:Label Text="Upit:" AssociatedControlID="txtUpit" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtUpit" runat="server" onFocus="this.select()" />
                </td>
                <% if (action == "Traži" || action == "Filtriraj") { %>
                <td>
                    <asp:TextBox ID="txtUpitDo" runat="server" onFocus="this.select()" />
                </td>
                <% } %>
            </tr>
    </table>    
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
</asp:Content>