<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EntriesByWeek.aspx.cs" Inherits="YourNamespace.EntriesByWeek" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Time Entries for Week</title>
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
            width: calc(100% - 240px);
        }
        h1 {
            text-align: center;
            color: #FF8C00; /* Dark orange */
        }
        label {
            font-weight: bold;
        }
        .week-info {
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
        .table-header {
            background-color: #808080; /* Grey background */
            color: white; /* White font */
        }
        .table td {
            padding: 10px; /* Adds padding to table cells */
        }
    </style>
</head>
<body>
 
    <form id="form1" runat="server">
    <!-- Include the sidemenu from an external file -->
    <%@ Register Src="~/SideMenu.ascx" TagPrefix="uc" TagName="Sidebar" %>
    <uc:Sidebar runat="server" />
        <div class="container">
            <h1>Time Entries for Week</h1>
            <!-- Label to display whether it's current or previous week and the date range -->
            <asp:Label ID="lblWeekInfo" runat="server" CssClass="week-info"></asp:Label>

            <br /><br />            

            <!-- GridView to display the time entries -->
            <asp:GridView ID="gvTimeEntries" runat="server" AutoGenerateColumns="False" ShowFooter="False" CssClass="table table-striped">
                <Columns>
                    <asp:BoundField DataField="log_date" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy}" HeaderStyle-CssClass="table-header" ItemStyle-CssClass="table-cell" />
                     <asp:TemplateField HeaderText="Hours Logged" HeaderStyle-CssClass="table-header" ItemStyle-CssClass="table-cell">
                        <ItemTemplate>
                            <%# String.Format("{0:00}:{1:00}", Eval("hours_logged"), Eval("minutes_logged")) %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="work_desc" HeaderText="Description" HeaderStyle-CssClass="table-header" ItemStyle-CssClass="table-cell" />
                </Columns>
            </asp:GridView>

            <br />

            <!-- Label to display the total logged hours outside the table -->
            <asp:Label ID="lblTotalHours" runat="server" CssClass="total-hours-info" />
        </div>
    </form>
</body>
</html>
