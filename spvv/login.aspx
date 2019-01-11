<%@ Page Title="" Language="C#" MasterPageFile="~/main.master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="spvv.WebForm2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphGlavni" runat="server">
    <div style="margin: auto; padding: 20px; border: 1px solid #A0A0A0; border-radius: 10px; background: white; width: 400px">
        <span style="font-size: 2em">Prijava</span><br/>
        <br/>
        <table>
                <tr>
                    <td style="width: 100px">
                        <asp:Label Text="Korisnik:" AssociatedControlID="txtKorisnik" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtKorisnik" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="Lozinka:" AssociatedControlID="txtLozinka" runat="server" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtLozinka" TextMode="Password" runat="server"/>
                    </td>
                </tr>
        </table>
        <br/>
        <div style="text-align: right">        
            <button type="button" id="btnPotvrdi" runat="server" onserverclick="btnPotvrdi_Click" style="width: 100px"><img src="img/check.png" class="action"/>Potvrdi</button>
            <button type="button" id="btnOdustani" runat="server" onserverclick="btnOdustani_Click" style="width: 100px"><img src="img/cancel.png" class="action"/>Odustani</button>
        </div>
        <asp:Label Text="" id="lblInfo" runat="server" />
    </div>
</asp:Content>
