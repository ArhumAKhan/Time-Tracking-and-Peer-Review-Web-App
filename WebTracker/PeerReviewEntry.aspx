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
    </style>
</head>
<body>
    <form id="peerReviewForm" runat="server">
        <div class="container">
            <h1>Peer Review Entry</h1>
            <asp:PlaceHolder ID="ReviewTable" runat="server"></asp:PlaceHolder>
            <asp:Button ID="SubmitButton" runat="server" Text="Submit" CssClass="submit-btn" OnClick="SubmitButton_Click" />
        </div>
    </form>
</body>
</html>
