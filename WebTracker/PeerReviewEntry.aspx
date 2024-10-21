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
    </style>
</head>
<body>
    <form id="peerReviewForm" runat="server">
        <div class="container">
            <h1>Peer Review Entry</h1>
            <asp:GridView ID="PeerReviewGrid" runat="server" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField DataField="StudentName" HeaderText="Student Name" />
                    <asp:TemplateField HeaderText="Skills" HeaderStyle-CssClass="criteria">
                        <ItemTemplate>
                            <asp:DropDownList ID="Criteria1" runat="server" CssClass="criteria">
                                <asp:ListItem Text="0" Value="0"></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Effort" HeaderStyle-CssClass="criteria">
                        <ItemTemplate>
                            <asp:DropDownList ID="Criteria2" runat="server" CssClass="criteria">
                                <asp:ListItem Text="0" Value="0"></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Collaboration" HeaderStyle-CssClass="criteria">
                        <ItemTemplate>
                            <asp:DropDownList ID="Criteria3" runat="server" CssClass="criteria">
                                <asp:ListItem Text="0" Value="0"></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Research" HeaderStyle-CssClass="criteria">
                        <ItemTemplate>
                            <asp:DropDownList ID="Criteria4" runat="server" CssClass="criteria">
                                <asp:ListItem Text="0" Value="0"></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Punctuation" HeaderStyle-CssClass="criteria">
                        <ItemTemplate>
                            <asp:DropDownList ID="Criteria5" runat="server" CssClass="criteria">
                                <asp:ListItem Text="0" Value="0"></asp:ListItem>
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:Button ID="SubmitButton" runat="server" Text="Submit" CssClass="submit-btn" OnClick="SubmitButton_Click" />
        </div>
    </form>
</body>
</html>
