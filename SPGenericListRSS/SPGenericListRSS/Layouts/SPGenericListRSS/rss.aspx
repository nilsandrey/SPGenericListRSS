<?xml version="1.0" encoding="UTF-8"?>
<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rss.aspx.cs" Inherits="SPGenericListRSS.Layouts.SPGenericListRSS.rss"%>
<rss version="2.0">
  <channel>

    <title><asp:Literal runat="server" ID="litRssTitle"></asp:Literal></title>

    <link><asp:Literal runat="server" ID="litRssLink"></asp:Literal></link>  
    <description><asp:Literal runat="server" ID="litRssDescription"></asp:Literal></description>  
                    
    <lastBuildDate><asp:Literal runat="server" ID="litRssLastBuildDate"></asp:Literal></lastBuildDate>
    <generator>generator</generator>
    <ttl><asp:Literal runat="server" ID="litRssTtl"></asp:Literal></ttl>
    <language><asp:Literal runat="server" ID="litRssLanguage"></asp:Literal></language>
    <image>
        <title><asp:Literal runat="server" ID="litRssImageTitle"></asp:Literal></title>
        <url><asp:Literal runat="server" ID="litRssImageUrl"></asp:Literal></url>
        <link><asp:Literal runat="server" ID="litRssImageLink"></asp:Literal></link>
    </image>    

<asp:Repeater ID="RepeaterRSS" runat="server">  
        <HeaderTemplate>  

        </HeaderTemplate>  
        <ItemTemplate>  
            <item>  
                <title><%# RemoveIllegalCharacters(DataBinder.Eval(Container.DataItem, "Title")) %></title>  
                <link><%# DataBinder.Eval(Container.DataItem, "Link") %></link>  
                <author><%# RemoveIllegalCharacters(DataBinder.Eval(Container.DataItem, "Author"))%></author>  
                <pubDate><%# String.Format("{0:R}", DataBinder.Eval(Container.DataItem, "PubDate"))%></pubDate>  
                <description><%# RemoveIllegalCharacters(DataBinder.Eval(Container.DataItem, "Description"))%></description>  
                <guid isPermaLink="true"><%# DataBinder.Eval(Container.DataItem, "PermalinkGuid") %></guid>
        </item>  
        </ItemTemplate>  
        <FooterTemplate>  
                
        </FooterTemplate>  
</asp:Repeater>  
      </channel>  
</rss>  