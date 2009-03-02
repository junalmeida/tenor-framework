<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Person.aspx.cs" Inherits="_Person" Title="Person - Sample Web App" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="Server">
    <p>
        This is a simple form to create or edit a person.
    </p>
    <p>
        <asp:Label ID="lblName" runat="server" Text="Name: " />
        <asp:TextBox ID="txtName" runat="server" />
    </p>
    <p>
        <asp:Label ID="lblEmail" runat="server" Text="Email: " />
        <asp:TextBox ID="txtEmail" runat="server" />
    </p>
    <asp:Button ID="btnSave" runat="server" Text="Save" onclick="btnSave_Click" />
</asp:Content>
