<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="PersonList.aspx.cs" Inherits="PersonList" Title="Person List - Tenor Sample Web App" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" Runat="Server">
<asp:Panel ID="pnlSearch" runat="server" DefaultButton="btnSearch">
    <p>Please, type in some information to search.</p>
    <p>Person's name: <asp:TextBox ID="txtName" runat="server" /></p>
    <p>Person's item's name: <asp:TextBox ID="txtItemName" runat="server" /></p>
    <p>Item's category: <asp:TextBox ID="txtCategory" runat="server" /></p>
    
    <asp:Button ID="btnSearch" runat="server" Text="Search" 
        onclick="btnSearch_Click" />
</asp:Panel>
<asp:Panel ID="pnlResults" runat="server" Visible="false">
    <asp:GridView ID="grdResults" runat="server" AutoGenerateColumns="False" Width="100%">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" />
            <asp:BoundField DataField="Email" HeaderText="Email" />
            <asp:HyperLinkField DataNavigateUrlFields="PersonId" 
                DataNavigateUrlFormatString="~/Person.aspx?id={0}" HeaderText="Edit" 
                Text="Edit" />
        
        </Columns>
        
    </asp:GridView>

</asp:Panel>
</asp:Content>

