<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewPeerReview.aspx.cs" Inherits="WebTimeTracker.ViewPeerReview" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View Peer Review</title>
    <style>
        th, td {
            text-align: center;
            border: 1px solid black;
        }
        th {
            width: 300px;
            height: 20px;
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
    <div class="container">
        <h1>Your Ratings</h1>
        <asp:PlaceHolder ID="RatingsTablePlaceholder" runat="server"></asp:PlaceHolder>
        <asp:Button ID="SubmitButton" runat="server" Text="Submit" CssClass="submit-btn" Visible="false" />
    </div>
</body>
</html>

