<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="ConsumerWebClient.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:textbox ID="txt" runat="server"></asp:textbox>
            <asp:button ID="btn" text="Button" runat="server" OnClick="btn_Click"></asp:button>
            <br/>
            <asp:label ID="lbl" runat="server"></asp:label>
        </div>
    </form>
</body>
</html>
