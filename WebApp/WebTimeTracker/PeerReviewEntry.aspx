<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PeerReviewEntry.aspx.cs" Inherits="PeerReviewApp.PeerReviewEntry" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Peer Review Entry</title>
    <style>
        th, td {
            text-align: center;
            border: 1px solid black;
        }
        th {
            width: 300px;
            height: 20px;
        }
        select {
            width: 100px;
            text-align: center;
        }
        .submit-btn {
            margin-top: 20px;
        }
        .container {
            margin-left: 220px;
            padding: 20px;
            width: calc(100% - 240px);
        }
    </style>
</head>
<body>
    <%@ Register Src="~/SideMenu.ascx" TagPrefix="uc" TagName="Sidebar" %>
    <uc:Sidebar runat="server" />
    <form id="peerReviewForm" runat="server">
        <div class="container">
            <h1>Peer Review Entry</h1>
            <asp:PlaceHolder ID="ReviewTablePlaceholder" runat="server"></asp:PlaceHolder>
            <asp:Button ID="SubmitButton" runat="server" Text="Submit" CssClass="submit-btn" Visible="false" />
        </div>
    </form>
</body>
</html>
