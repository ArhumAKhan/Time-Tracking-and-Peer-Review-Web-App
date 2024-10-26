<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeEntry.aspx.cs" Inherits="YourNamespace.TimeEntry" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Time Entry App</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 25px;
            display: flex;
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
            width: calc(75% - 240px);
        }
        h1 {
            text-align: center;
        }
        label {
            font-weight: bold;
        }
        select, input[type="text"], input[type="date"], textarea {
            width: 50%;
            margin: 5px 0;
            padding: 5px;
        }
        .submit-btn {
            width: 25%;
            padding: 15px;
            background-color: #4CAF50;
            color: white;
            border: none;
            cursor: pointer;
        }
        .submit-btn:disabled {
            background-color: #cccccc;
            cursor: not-allowed;
        }
    </style>
    <script type="text/javascript">
    function enableSubmitButton() {
        var description = document.getElementById('<%= entryDescription.ClientID %>').value;
        var submitButton = document.getElementById('<%= submitButton.ClientID %>');

        // Enable the submit button if the description is 30 characters or more
        if (description.length >= 30) {
            submitButton.disabled = false;
        } else {
            submitButton.disabled = true;
        }
    }
    </script>

</head>
<body>

    <div class="sidebar">
        <a href="TimeEntry.aspx">Enter Time</a>
        <a href="EntriesByWeek.aspx?week=current">View Current Week</a>
        <a href="EntriesByWeek.aspx?week=previous">View Previous Week</a>
        <a href="EntriesByWeek.aspx?week=all">View Entire Project</a>
        <a href="PeerReviewEntry.aspx">Peer Review</a>
    </div>

    <div class="container">
        <h1>Time Entry</h1>
        <form id="timeEntryForm" runat="server">
            <asp:TextBox ID="utdId" runat="server" Visible="false"></asp:TextBox>

            <label for="courseId">Course ID:</label>
            <asp:TextBox ID="courseId" runat="server" ReadOnly="true" Text="cs4485"></asp:TextBox>
            <br>

            <label for="entryDate">Date:</label>
            <asp:TextBox ID="entryDate" runat="server" TextMode="Date" required></asp:TextBox>
            <br>

            <label for="hoursLogged">Hours Logged (HH:MM):</label>
            <div style="display: flex; gap: 10px;">
                <asp:DropDownList ID="hoursLogged" runat="server" required></asp:DropDownList>
                <asp:DropDownList ID="minutesLogged" runat="server" required></asp:DropDownList>
            </div>
            <br>

            <label for="entryDescription">Description of Work (Min 30 characters):</label>
            <br>
            <asp:TextBox ID="entryDescription" runat="server" TextMode="MultiLine" Rows="4" required></asp:TextBox>
            <br>
            <!-- Add the Label control here to display messages -->
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
            <br>

            <asp:Button ID="submitButton" runat="server" CssClass="submit-btn" Text="Submit" OnClick="SubmitButton_Click" Enabled="false" />
        </form>
    </div>

</body>
</html>
