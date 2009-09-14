<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Person.aspx.cs" Inherits="SampleApp._Person" Title="Person - Sample Web App" %>

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
        <p>
        Choose linked departments:
        <asp:CheckBoxList ID="cblDepartments" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow">
        </asp:CheckBoxList>
        </p>
    <p>
        Choose the items for this person:
        <asp:DropDownList ID="cmbCategory" runat="server" AutoPostBack="true" 
            onselectedindexchanged="cmbCategory_SelectedIndexChanged" /><br />
        <asp:CheckBoxList ID="cblItems" runat="server" RepeatLayout="Flow" RepeatDirection="Vertical" />
    </p>
    <p>
        <asp:CheckBox ID="chkActive" runat="server" Text="Active?" TextAlign="Left" />
    </p>
    <p>
        Choose the marital status:
        <asp:DropDownList ID="cmbMaritalStatus" runat="server" />
    </p>
    <p>
        <asp:Label ID="lblPhoto" runat="server" Text="Photo:" />
        <asp:FileUpload ID="fupPhoto" runat="server" />
    </p>
    
    <asp:Button ID="btnSave" runat="server" Text="Save" onclick="btnSave_Click" />
</asp:Content>
