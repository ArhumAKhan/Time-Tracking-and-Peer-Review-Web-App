<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="YourNamespace.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login Page</title>
    <style>
        body {
            background-image: url('/Images/background1.png');
            background-size: cover;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }
        .login-container {
            background-color: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            width: 500px;
            display: flex;
            align-items: center;
        }
        .logo {
            width: 250px;
            height: 150px;
            margin-right: 20px;
        }
        .form-content {
            flex-grow: 1;
        }
        .input-label {
            width: 100%;
            margin-bottom: 5px;
            font-weight: bold;
        }
        .input-field {
            width: 60%;
            margin-bottom: 15px;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 5px;
        }
        .login-button {
            width: 60%;
            padding: 10px;
            background-color: darkgreen;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 16px;
        }
        .login-button:hover {
            background-color: #006400;
        }
        .message-label {
            margin-top: 10px;
            color: red;
        }     
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <img src="/Images/utdlogo.png" alt="UTD Logo" class="logo" />
            <div class="form-content">
                <label for="txtNetID" class="input-label">NetID</label>
                <br />
                <asp:TextBox ID="txtNetID" runat="server" CssClass="input-field" placeholder="Enter Your NetID"></asp:TextBox>
                <br />
                <label for="txtPassword" class="input-label">Password</label>
                <br />
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="input-field" placeholder="Enter Your Password"></asp:TextBox>
                <br />
                <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" CssClass="login-button" />

                <asp:Label ID="lblMessage" runat="server" CssClass="message-label"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
