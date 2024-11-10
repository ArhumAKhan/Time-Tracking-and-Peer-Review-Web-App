<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewPeerReview.aspx.cs" Inherits="WebTimeTracker.ViewPeerReview" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View Peer Review</title>
    <style>
        table {
            border-collapse: collapse;
            border-spacing: 0;
        }
        th, td {
            text-align: center;
            border: 2px solid lightgray;
        }
        th {
            width: 300px;
            height: 20px;
            background-color: lightgray;
        }
        td {
            color: forestgreen;
            font-size: 18px;
        }
        select {
            width: 100px;
            text-align: center;
        }
        .submit-btn {
            margin-top: 20px;
        }
        .container {
            margin-top: 200px;
            margin-left: 540px;
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

