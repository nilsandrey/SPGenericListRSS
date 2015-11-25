using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace SPGenericListRSS.Layouts.SPGenericListRSS
{
    public class RssItem
    {
        public String PermalinkGuid { get; set; }
        public String Title { get; set; }
        public String Link { get; set; }
        public String Description { get; set; }
        public DateTime PubDate { get; set; }
        public String Author { get; set; }


    }

    public class RssFeedDto
    {
        public RssFeedDto()
        {
            Items = new List<RssItem>();
        }
        public DateTime LastBuildDate { get; set; }
        public string Description { get; set; }
        public String ImageUrl { get; set; }
        public String ImageLink { get; set; }
        public String ImageTitle { get; set; }
        public string Language { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public String XsltLink { get; set; }
        public int Ttl { get; set; }
        public List<RssItem> Items { get; set; }

        public String ETag { get; set; }




    }


    public partial class rss : LayoutsPageBase
    {
        private void BindData(RssFeedDto feed)
        {
            litRssLastBuildDate.Text = feed.LastBuildDate.ToString();
            litRssDescription.Text = feed.Description;
            litRssImageUrl.Text = feed.ImageUrl;
            litRssImageLink.Text = feed.ImageLink;
            litRssImageTitle.Text = feed.ImageTitle;
            litRssLanguage.Text = feed.Language;
            litRssLink.Text = feed.Link;
            litRssTitle.Text = feed.Title;
            litRssTtl.Text = feed.Ttl.ToString();
            RepeaterRSS.DataSource = feed.Items;
            RepeaterRSS.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Get previous page title and URL
                SPSecurity.RunWithElevatedPrivileges(delegate
                {
                    using (SPSite site = new SPSite(SPContext.Current.Web.Url))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            var idStr = HttpContext.Current.Request.QueryString["List"];
                            var idViewStr = HttpContext.Current.Request.QueryString["View"];
                            var fieldTitle = HttpContext.Current.Request.QueryString["T"];
                            var fieldAuthor = HttpContext.Current.Request.QueryString["A"];
                            var fieldDate = HttpContext.Current.Request.QueryString["D"];
                            var fieldBody = HttpContext.Current.Request.QueryString["B"];
                            var relUrl = HttpContext.Current.Request.QueryString["U"];
                            var feed = new RssFeedDto();
                            if (PopulateFromList(feed, web, idStr, idViewStr, fieldTitle, fieldAuthor, fieldDate,
                                fieldBody,
                                relUrl))
                            {
                                BindData(feed);
                                Response.AppendHeader("ETag", feed.ETag);
                                Response.AppendHeader("Last-Modified", feed.LastBuildDate.ToString());
                                Response.ContentType = "text/xml";
                                Response.ContentEncoding = Encoding.UTF8;
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {

            }
        }

        public string GetProperty(object val, string defaultVal)
        {
            if (val == null || String.IsNullOrEmpty(val.ToString())) return defaultVal;
            return val.ToString();
        }

        private bool PopulateFromList(RssFeedDto feed, SPWeb web, string listIdStr, string viewIdStr, string fieldTitle, string fieldAuthor, string fieldDate, string fieldBody, string RelURL)
        {

            try
            {
                var list = web.Lists[listIdStr];
                var limit = int.Parse(GetProperty(list.RootFolder.Properties["vti_rss_ItemLimit"], "30"));
                var allItemsLink = SPUtility.ConcatUrls(web.Url, list.DefaultView.Url);
                feed.LastBuildDate = list.LastItemModifiedDate;
                feed.ETag = list.CurrentChangeToken.ToString();
                feed.Description = GetProperty(list.RootFolder.Properties["vti_rss_ChannelDescription"], list.Description);
                feed.ImageUrl = ConvertRelativeUrlToAbsoluteUrl(GetProperty(list.RootFolder.Properties["vti_rss_ChannelImageUrl"], list.ImageUrl));
                feed.ImageLink = allItemsLink;
                feed.ImageTitle = list.Title;
                feed.Language = web.UICulture.Name;
                feed.Link = allItemsLink;
                feed.Title = GetProperty(list.RootFolder.Properties["vti_rss_ChannelTitle"], list.Title);
                feed.XsltLink = web.Url + "/_layouts/RssXslt.aspx?List=" + list.ID;
                feed.Ttl = 60;

                for (var i = 0; i < list.Items.Count; i++)
                {
                    try
                    {
                        var listItem = list.Items[i];

                        var feedItem = new RssItem();
                        var itemUrl = web.Site.RootWeb.Url + RelURL + "?ID=" + listItem.ID;
                        feedItem.PermalinkGuid = itemUrl;
                        feedItem.Title = listItem[fieldTitle].ToString();
                        feedItem.Title = Regex.Replace(feedItem.Title, @"<[^>]+>|&nbsp;", "").Trim();
                        feedItem.Link = itemUrl;
                        feedItem.Description = listItem[fieldBody] != null ? listItem[fieldBody].ToString() : "";
                        try
                        {
                            feedItem.PubDate = Convert.ToDateTime(listItem[fieldDate].ToString());
                        }
                        catch
                        {
                            try
                            {
                                feedItem.PubDate = Convert.ToDateTime(listItem["Modified"].ToString());
                            }
                            catch
                            {
                                feedItem.PubDate = DateTime.Today;
                            }
                        }
                        feedItem.Author = listItem[fieldAuthor].ToString();
                        feed.Items.Add(feedItem);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                feed.Items = feed.Items.OrderByDescending(x => x.PubDate).Take(limit).ToList();


                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public string ConvertRelativeUrlToAbsoluteUrl(string relativeUrl)
        {
            if (Request.IsSecureConnection)
                return string.Format("https://{0}{1}", Request.Url.Host, Page.ResolveUrl(relativeUrl));
            else
                return string.Format("http://{0}{1}", Request.Url.Host, Page.ResolveUrl(relativeUrl));
        }

        protected string RemoveIllegalCharacters(object input)
        {
            // cast the input to a string  
            string data = input.ToString();

            // replace illegal characters in XML documents with their entity references  
            data = data.Replace("&", "&amp;");
            data = data.Replace("\"", "&quot;");
            data = data.Replace("'", "&apos;");
            data = data.Replace("<", "&lt;");
            data = data.Replace(">", "&gt;");

            return data;
        }

    }
}
