<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PeerReviewEntry.aspx.cs" Inherits="PeerReviewApp.PeerReviewEntry" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Peer Review Entry</title>
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
    <form id="peerReviewForm" runat="server">
        <div class="sidebar">
            <a href="TimeEntry.aspx">Enter Time</a>
            <a href="EntriesByWeek.aspx?week=current">View Current Week</a>
            <a href="EntriesByWeek.aspx?week=previous">View Previous Week</a>
            <a href="EntriesByWeek.aspx?week=all">View Entire Project</a>
            <a href="PeerReviewEntry.aspx">Peer Review</a>
        </div>
        <div class="container">
            <h1>Peer Review Entry</h1>
            <asp:PlaceHolder ID="ReviewTablePlaceholder" runat="server"></asp:PlaceHolder>
            <asp:Button ID="SubmitButton" runat="server" Text="Submit" CssClass="submit-btn" Visible="false" />
        </div>
    </form>
</body>
</html>
