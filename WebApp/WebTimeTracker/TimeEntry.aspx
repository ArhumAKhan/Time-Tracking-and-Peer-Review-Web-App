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
            background-color: #f9f9f9;
        }
        .container {
            margin-left: 220px;
            padding: 20px;
            width: calc(100% - 240px);
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }
        h1 {
            text-align: center;
            color: #FF8C00; /* Dark orange */
        }
        label {
            font-weight: bold;
            color: #000000;
        }
        select, input[type="text"], input[type="date"], textarea {
            width: 100%;
            margin: 5px 0;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            background-color: #f8f9f9;
        }
        .submit-btn {
            width: 100%;
            padding: 15px;
            background-color: #4CAF50;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
        }
        .submit-btn:disabled {
            background-color: #cccccc;
            cursor: not-allowed;
        }
        .submit-btn:hover:not(:disabled) {
            background-color: #45a049;
        }
    </style>
    <script type="text/javascript">
        function enableSubmitButton() {
            var description = document.getElementById('<%= entryDescription.ClientID %>').value;
            var hoursLogged = document.getElementById('<%= hoursLogged.ClientID %>').value;
            var submitButton = document.getElementById('<%= submitButton.ClientID %>');

            // Check if the time format matches HH:MM (basic validation)
            var timeFormat = /^([0-1]\d|2[0-3]):([0-5]\d)$/;
            var isTimeValid = timeFormat.test(hoursLogged);

            // Enable the submit button if all conditions are met
            if (description.length >= 30 && isTimeValid) {
                submitButton.disabled = false;
            } else {
                submitButton.disabled = true;
            }
        }

        // Attach event listeners to relevant fields
        document.getElementById('<%= entryDescription.ClientID %>').addEventListener('input', enableSubmitButton);
        document.getElementById('<%= hoursLogged.ClientID %>').addEventListener('input', enableSubmitButton);

    </script>

</head>
<body>

    <!-- Include the sidemenu from an external file -->
    <%@ Register Src="~/SideMenu.ascx" TagPrefix="uc" TagName="Sidebar" %>
    <uc:Sidebar runat="server" />

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
            <asp:TextBox ID="hoursLogged" runat="server" required placeholder="HH:MM"></asp:TextBox>
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
