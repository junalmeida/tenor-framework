<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="SampleApp._Default"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" Runat="Server">
<div>
    <p>Captcha Test:</p>
    Please type in the following characters:
    <img src="blah.axd?captcha=captcha" alt="captcha" /><a href="blah.axd?captcha=captcha&audio=1">Audio</a>
    <asp:TextBox ID="txtCaptcha" runat="server" />
    <asp:Button ID="btnCheck" runat="server" Text="Test" OnClick="btnCheck_Click" />
</div>


</asp:Content>

