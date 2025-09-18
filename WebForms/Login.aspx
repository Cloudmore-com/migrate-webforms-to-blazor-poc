<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebForms.Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-6 mx-auto">
            <h2><%: Title %>.</h2>
            <hr />
            <div class="form-horizontal">
                <div class="form-group row mb-3">
                    <asp:Label runat="server" AssociatedControlID="Username" CssClass="col-md-3 col-form-label">Username</asp:Label>
                    <div class="col-md-9">
                        <asp:TextBox runat="server" ID="Username" CssClass="form-control" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Username"
                            CssClass="text-danger" ErrorMessage="The username field is required." />
                    </div>
                </div>
                <div class="form-group row mb-3">
                    <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-3 col-form-label">Password</asp:Label>
                    <div class="col-md-9">
                        <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Password"
                            CssClass="text-danger" ErrorMessage="The password field is required." />
                    </div>
                </div>
                <div class="form-group row mb-3">
                    <div class="offset-md-3 col-md-9">
                        <div class="form-check">
                            <asp:CheckBox runat="server" ID="RememberMe" CssClass="form-check-input" />
                            <asp:Label runat="server" AssociatedControlID="RememberMe" CssClass="form-check-label">Remember me</asp:Label>
                        </div>
                    </div>
                </div>
                <div class="form-group row">
                    <div class="offset-md-3 col-md-9">
                        <asp:Button runat="server" OnClick="LogIn_Click" Text="Log in" CssClass="btn btn-primary" />
                    </div>
                </div>
                <div class="form-group row">
                    <div class="offset-md-3 col-md-9">
                        <asp:Literal runat="server" ID="FailureText" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>