<%@ Page Title="About (WebForms)" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="WebForms.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title"><%: Title %>.</h2>
        <h3>Your application description page.</h3>
        <p>Use this area to provide additional information.</p>
        <h4>Session Variables</h4>
        <ul>
            <li>UserInitialized: <%= Session["UserInitialized"] ?? "N/A" %></li>
            <li>SessionID: <%= Session["SessionID"] ?? "N/A" %></li>
            <li>VisitTime: <%= Session["VisitTime"] ?? "N/A" %></li>
            <li>BlazorString: <%= Session["BlazorString"] ?? "N/A" %></li>
        </ul>
    </main>
</asp:Content>
