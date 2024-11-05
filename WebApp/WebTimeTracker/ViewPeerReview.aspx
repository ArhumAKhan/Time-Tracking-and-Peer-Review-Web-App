<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewPeerReview.aspx.cs" Inherits="WebTimeTracker.ViewPeerReview" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View Peer Review</title>
    <style>
        .criteria {
            text-align: center;
            width: 200px;
        }
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
        .sidebar {
            width: 200px;
            background-color: #f4f4f4;
            padding: 20px;
            height: 100vh;
            position: fixed;
            top: 0;
            left: 0;
            overflow-y: auto;
        }
        .sidebar h2 {
            font-size: 18px;
            text-align: center;
        }
        .sidebar a {
            display: block;
            padding: 10px;
            margin: 5px 0;
            color: #333;
            text-decoration: none;
            background-color: #e2e2e2;
            text-align: center;
            border-radius: 5px;
        }
        .sidebar a:hover {
            background-color: #ddd;
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

