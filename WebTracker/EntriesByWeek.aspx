<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EntriesByWeek.aspx.cs" Inherits="YourNamespace.EntriesByWeek" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Time Entries for Current Week</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Time Entries for Current Week</h1>   
            
            <!-- GridView to display the time entries -->
            <asp:GridView ID="gvTimeEntries" runat="server" AutoGenerateColumns="False" ShowFooter="True" CssClass="table table-striped" OnRowDataBound="gvTimeEntries_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="log_date" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy}" />
                     <asp:TemplateField HeaderText="Hours Logged">
                        <ItemTemplate>
                            <%# String.Format("{0:00}:{1:00}", Math.Floor(Convert.ToDouble(Eval("hours_logged"))), (Convert.ToDouble(Eval("hours_logged")) % 1) * 60) %>
                        </ItemTemplate>
                        <FooterTemplate>
                            <strong>Total Hours: </strong><asp:Label ID="lblTotalHours" runat="server" />
                        </FooterTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="work_desc" HeaderText="Description" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
