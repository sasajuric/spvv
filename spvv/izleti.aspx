<%@ Page Title="" Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeBehind="izleti.aspx.cs" Inherits="spvv.wfIzleti" %>
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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphGlavni" runat="server">
    <h1>Izleti</h1>
    <asp:Repeater ID="rptMain" runat="server">
        <HeaderTemplate>
            <table style="border-collapse: collapse; width: 768px" class="grid">
                <tr class="header">
                    <th style="width: 80px">Datum</th>
                    <th style="width: 184px">Naziv</th>
                    <th style="width: 175px">Glavni vodič</th>
                    <th style="width: 175px">Pomoćni vodič</th>
                    <th style="width: 150px">Udruga</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
                <tr class="row" id="trMain<%# Eval("id") %>" onclick="grdMain_Click(<%# Eval("id") %>)">
                    <td style="width: 80px"><%# Convert.ToDateTime(Eval("datum")).ToString("dd.MM.yyyy.") %></td>
                    <td style="width: 184px"><%# Eval("naziv") %></td>
                    <td style="width: 175px"><%# Eval("glavnivodic") %></td>
                    <td style="width: 175px"><%# Eval("pomocnivodic") %></td>
                    <td style="width: 150px"><%# Eval("udruga") %></td>
                </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
                <tr class="altrow" id="trMain<%# Eval("id") %>" onclick="grdMain_Click(<%# Eval("id") %>)">
                    <td style="width: 80px"><%# Convert.ToDateTime(Eval("datum")).ToString("dd.MM.yyyy.") %></td>
                    <td style="width: 184px"><%# Eval("naziv") %></td>
                    <td style="width: 175px"><%# Eval("glavnivodic") %></td>
                    <td style="width: 175px"><%# Eval("pomocnivodic") %></td>
                    <td style="width: 150px"><%# Eval("udruga") %></td>
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
                    <asp:Label Text="Datum:" AssociatedControlID="txtDatum" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtDatum" runat="server" onFocus="this.select()" />
                </td>
                <% if (action == "Traži" || action == "Filtriraj") { %>
                <td>
                    <asp:TextBox ID="txtDatumDo" runat="server" onFocus="this.select()" />
                </td>
                <% } %>
            </tr>
            <tr>
                <td>
                    <asp:Label Text="Naziv:" AssociatedControlID="txtNaziv" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtNaziv" runat="server" onFocus="this.select()" />
                </td>
                <% if (action == "Traži" || action == "Filtriraj") { %>
                <td>
                    <asp:TextBox ID="txtNazivDo" runat="server" onFocus="this.select()" />
                </td>
                <% } %>
            </tr>
            <tr>
                <td>
                    <asp:Label Text="Glavni vodič:" AssociatedControlID="ddlGlavniVodic" runat="server" />
                </td>
                <% if (!(action == "Traži" || action == "Filtriraj")) { %>
                <td>
                    <asp:DropDownList ID="ddlGlavniVodic" runat="server" AppendDataBoundItems="true">
                    </asp:DropDownList><br/>
                </td>
                <% } %>
                <% if (action == "Traži" || action == "Filtriraj") { %>
                <td>
                    <asp:TextBox ID="txtGlavniVodic" runat="server" onFocus="this.select()" />
                </td>
                <td>
                    <asp:TextBox ID="txtGlavniVodicDo" runat="server" onFocus="this.select()" />
                </td>
                <% } %>
            </tr>
            <tr>
                <td>
                    <asp:Label Text="Pomoćni vodič:" AssociatedControlID="ddlPomocniVodic" runat="server" />
                </td>
                <% if (!(action == "Traži" || action == "Filtriraj")) { %>
                <td>
                    <asp:DropDownList ID="ddlPomocniVodic" runat="server" AppendDataBoundItems="true">
                    </asp:DropDownList><br/>
                </td>
                <% } %>
                <% if (action == "Traži" || action == "Filtriraj") { %>
                <td>
                    <asp:TextBox ID="txtPomocniVodic" runat="server" onFocus="this.select()" />
                </td>
                <td>
                    <asp:TextBox ID="txtPomocniVodicDo" runat="server" onFocus="this.select()" />
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
                    <asp:Label Text="Odredište:" AssociatedControlID="txtOdrediste" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtOdrediste" runat="server" onFocus="this.select()" />
                </td>
                <% if (action == "Traži" || action == "Filtriraj") { %>
                <td>
                    <asp:TextBox ID="txtOdredisteDo" runat="server" onFocus="this.select()" />
                </td>
                <% } %>
            </tr>
            <tr>
                <td>
                    <asp:Label Text="Trajanje:" AssociatedControlID="txtTrajanje" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtTrajanje" runat="server" onFocus="this.select()" />
                </td>
                <% if (action == "Traži" || action == "Filtriraj") { %>
                <td>
                    <asp:TextBox ID="txtTrajanjeDo" runat="server" onFocus="this.select()" />
                </td>
                <% } %>
            </tr>
            <tr>
                <td>
                    <asp:Label Text="Broj sudionika:" AssociatedControlID="txtBrojSudionika" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtBrojSudionika" runat="server" onFocus="this.select()" />
                </td>
                <% if (action == "Traži" || action == "Filtriraj") { %>
                <td>
                    <asp:TextBox ID="txtBrojSudionikaDo" runat="server" onFocus="this.select()" />
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
                <button type="button" id="btnPotvrdi" runat="server" onserverclick="btnPotvrdi_Click"><img src="img/check.png" class="action"/>Potvrdi</button>
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