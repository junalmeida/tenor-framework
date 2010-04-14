<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" Inherits="SampleApp._Default"  Codebehind="Default.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" Runat="Server">
<div>
    <p>Captcha Test:</p>
    Please type in the following characters:
    <img src="blah.axd?captcha=captcha" alt="captcha" /><a href="blah.axd?captcha=captcha&audio=1">Audio</a>
    <asp:TextBox ID="txtCaptcha" runat="server" />
    <asp:Button ID="btnCheck" runat="server" Text="Test"  />
    <p>
    
    Only Numbers: <tenor:TextBox runat="server" TextBoxMode="Float" ID="floatText" />
    Percent: <tenor:TextBox runat="server" TextBoxMode="Percent" ID="percentText" />
    Money: <tenor:TextBox runat="server" TextBoxMode="Currency" ID="currencyText" />    
    Cpf: <tenor:TextBox runat="server" TextBoxMode="CPF" ID="CPF" />
    
    <asp:GridView ID="grdRelatorio" runat="server" AutoGenerateColumns="False" 
                OnRowCreated="grdRelatorio_RowCreated" CellPadding="4" ForeColor="#333333" GridLines="None" ShowFooter="true" 
                Width="657px">
        <RowStyle BackColor="#E3EAEB" />
        <Columns>
            <asp:TemplateField HeaderText="Retirada em Real (R$)">
                <FooterTemplate>
                    <asp:Label ID="lblTotalRetiradaSaldo" runat="server" />                    
                </FooterTemplate>
                <ItemTemplate>
                    <tenor:TextBox ID="txtRetirada" TextBoxMode="Currency" AutoPostBack="true" Width="100px" runat="server" /><br />                    
                 </ItemTemplate>
            </asp:TemplateField>
        </Columns>        
        <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
        <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
        <EditRowStyle BackColor="#7C6F57" />
        <AlternatingRowStyle BackColor="White" />        
    </asp:GridView>
    
    </p>
</div>


</asp:Content>

