<%@ Page Title="" Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeBehind="sintetika.aspx.cs" Inherits="spvv.wfSintetika" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphGlavni" runat="server">
    <h1>Sintetika</h1>
    <table>
            <tr>
                <td>
                    <asp:Label Text="Godina:" AssociatedControlID="txtGodina" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtGodina" runat="server" onFocus="this.select()" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <button type="button" id="btnIspisi" runat="server" onserverclick="btnIspisi_Click"><img src="img/print.png" class="action"/>Ispiši</button>
                </td>
            </tr>
    </table>    
</asp:Content>