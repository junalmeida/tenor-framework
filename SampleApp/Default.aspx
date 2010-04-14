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
    
    <asp:GridView ID="gridReport" runat="server" AutoGenerateColumns="False" 
                OnRowCreated="gridReport_RowCreated" CellPadding="4" ForeColor="#333333" GridLines="None" ShowFooter="true" 
                Width="657px">
        <RowStyle BackColor="#E3EAEB" />
        <Columns>
            <asp:TemplateField HeaderText="Value (US$)">
                <FooterTemplate>
                    <asp:Label ID="lblTotal" runat="server" />                    
                </FooterTemplate>
                <ItemTemplate>
                    <tenor:TextBox ID="txtValue" TextBoxMode="Currency" AutoPostBack="true" Width="100px" runat="server" onblur="Calculate(this);" />              
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
    <asp:HiddenField runat="server" ID="hidden" Value="50" />
    </p>
</div>
<script type ="text/javascript">
    function Calculate(textBox) {
        var userValue = parseFloat(textBox.value);
        var hidden = parseFloat($("ctl00_Content_hidden").value);
        alert(userValue + hidden);        
    }
</script>

</asp:Content>

