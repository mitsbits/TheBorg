﻿using Borg.Infra.Relational;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;

namespace Borg
{
    [HtmlTargetElement("ul", Attributes = "pagination")]
    public class PaginationTagHelper : TagHelper
    {
        [HtmlAttributeName("model")]
        public IPagedResult Model { get; set; }

        [HtmlAttributeName("settings")]
        public Pagination.PaginationInfo Settings { get; set; } = new Pagination.PaginationInfo();

        [HtmlAttributeName("query")]
        public QueryString Query { get; set; } = new QueryString(null);

        [HtmlAttributeName("url-generator")]
        public Func<int, string> GeneratePageUrl { get; set; } = null;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Model == null) throw new ArgumentNullException(nameof(Model));

            var content = Pagination.GetHtmlPager(Model, x => GeneratePageUrl?.Invoke(x), Query.ToNameValueCollection(), Settings, null);
            var trimstart = content.IndexOf('>') + 1;
            var trimend = content.Length - content.LastIndexOf('<');
            var trimmed = content.Substring(trimstart, content.Length - trimend - trimstart);
            output.TagMode = TagMode.StartTagAndEndTag;

            var oprslt = output.Content.SetHtmlContent(trimmed);

            var cls = Settings.ElementClass;

            if (output.Attributes.ContainsName("class"))
            {
                cls += $" {output.Attributes["class"].Value}";
            }
            output.Attributes.SetAttribute("class", cls);
        }


    }

    public static class Pagination
    {
        #region Pagination

        public static HtmlString HtmlPager<T>(
         this IHtmlHelper helper,
         IPagedResult<T> metaData,
         Func<int, string> generatePageUrl,
         QueryString query,
         PaginationInfo settings = null,
         object htmlAttributes = null)
        {
            if (metaData == null)
                throw new ArgumentNullException(nameof(metaData), "A navigation collection is mandatory.");
            if (!metaData.Any()) return HtmlString.Empty;
            if (settings == null) settings = new PaginationInfo();
            return new HtmlString(GetHtmlPager(metaData, generatePageUrl, query.ToNameValueCollection(), settings, htmlAttributes));
        }

        public static HtmlString HtmlPager<T>(
         this IHtmlHelper helper,
         IPagedResult<T> metaData,
         Func<int, string> generatePageUrl,
         NameValueCollection routedValues = null,
         PaginationInfo settings = null,
         object htmlAttributes = null)
        {
            if (metaData == null)
                throw new ArgumentNullException(nameof(metaData), "A navigation collection is mandatory.");
            if (!metaData.Any()) return HtmlString.Empty;
            if (settings == null) settings = new PaginationInfo();
            return new HtmlString(GetHtmlPager(metaData, generatePageUrl, routedValues, settings, htmlAttributes));
        }

        #region Private Zurb Pagination

        internal static string GetHtmlPager(
            IPagedResult metaData,
            Func<int, string> generatePageUrl,
            NameValueCollection routedValues,
            PaginationInfo settings,
            object htmlAttributes)
        {
            var listItemLinks = new List<TagBuilder>();

            //first

            if (settings.DisplayLinkToFirstPage)
                listItemLinks.Add(First(metaData, generatePageUrl, routedValues, settings));

            if (settings.DisplayLinkToPreviousPage)
                listItemLinks.Add(Previous(metaData, generatePageUrl, routedValues, settings));

            //text
            if (settings.DisplayPageCountAndCurrentLocation)
                listItemLinks.Add(PageCountAndLocationText(metaData, settings));

            //text
            if (settings.DisplayItemSliceAndTotal)
                listItemLinks.Add(ItemSliceAndTotalText(metaData, settings));

            //page
            if (!settings.PagerInChunks)
            {
                if (settings.DisplayLinkToIndividualPages)
                {
                    //calculate start and end of range of page numbers
                    var start = 1;
                    var end = metaData.TotalPages;
                    if (settings.MaximumPageNumbersToDisplay.HasValue && metaData.TotalPages > settings.MaximumPageNumbersToDisplay)
                    {
                        var maxPageNumbersToDisplay = settings.MaximumPageNumbersToDisplay.Value;
                        start = metaData.Page - maxPageNumbersToDisplay / 2;
                        if (start < 1)
                            start = 1;
                        end = maxPageNumbersToDisplay;
                        if ((start + end - 1) > metaData.TotalPages)
                            start = metaData.TotalPages - maxPageNumbersToDisplay + 1;
                    }

                    //if there are previous page numbers not displayed, show an ellipsis
                    if (settings.DisplayEllipsesWhenNotShowingAllPageNumbers && start > 1)
                        listItemLinks.Add(EllipsesPrevious(metaData, generatePageUrl, routedValues, settings));

                    foreach (var i in Enumerable.Range(start, end))
                    {
                        //show page number link
                        listItemLinks.Add(Page(i, metaData, generatePageUrl, routedValues, settings));
                    }

                    //if there are subsequent page numbers not displayed, show an ellipsis
                    if (settings.DisplayEllipsesWhenNotShowingAllPageNumbers && (start + end - 1) < metaData.TotalPages)
                        listItemLinks.Add(EllipsesNext(metaData, generatePageUrl, routedValues, settings));
                }
            }
            else //show page links in chunks
            {
                int current = metaData.Page;

                int chunckStart = current;

                if (current % settings.ChunkCount != 0)
                {
                    while (chunckStart % settings.ChunkCount != 0)
                    {
                        chunckStart -= 1;
                    }
                }
                else
                {
                    chunckStart = current - settings.ChunkCount;
                }
                foreach (var i in Enumerable.Range(chunckStart + 1, settings.ChunkCount))
                {
                    //show page number link
                    listItemLinks.Add(Page(i, metaData, generatePageUrl, routedValues, settings));
                }
            }

            //next
            if (settings.DisplayLinkToNextPage)
                listItemLinks.Add(Next(metaData, generatePageUrl, routedValues, settings));

            //last
            if (settings.DisplayLinkToLastPage)
                listItemLinks.Add(Last(metaData, generatePageUrl, routedValues, settings));

            //collapse all of the list items into one big string
            string listItemLinksString = null;

            listItemLinksString = listItemLinks.Aggregate(
               new StringBuilder(),
               (sb, listItem) => sb.Append(listItem.GetString()),
               sb => sb.ToString()
               );

            var ul = new TagBuilder("ul");
            ul.InnerHtml.AppendHtml(listItemLinksString);

            ul.AddCssClass(settings.ElementClass);
            if (htmlAttributes != null) ul.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            return ul.GetString();
        }

        /// <summary>
        /// Gets the query string to append to the constructed pager url
        /// </summary>
        /// <param name="routedValues">The routed values.</param>
        /// <param name="pageVariable">The page variable.</param>
        /// <returns></returns>
        private static string GetRoutedValues(NameValueCollection routedValues, string pageVariable)
        {
            string paramBuilder = string.Empty;

            if (routedValues.Count > 0)
            {
                foreach (string key in routedValues.Keys)
                {
                    if (!(key.Equals(pageVariable, StringComparison.OrdinalIgnoreCase))) paramBuilder += "&" + key + "=" + routedValues[key];
                }
            }

            return paramBuilder;
        }

        private static TagBuilder Next(IPagedResult metadata,
            Func<int, string> generatePageUrl,
            NameValueCollection routedValues,
            PaginationInfo settings)
        {
            var item = new TagBuilder("li");
            item.AddCssClass(settings.ArrowClass + " next");
            var targetPageNumber = metadata.Page + 1;
            var next = new TagBuilder("a");
            next.InnerHtml.Append(settings.NextDisplay);
            if (metadata.HasNextPage)
            {
                next.MergeAttribute("href",
                    generatePageUrl(targetPageNumber)
                    + GetRoutedValues(routedValues,
                    settings.PageVariableName), true);
            }
            else
            {
                item.AddCssClass(settings.UnavailableClass);
                next.MergeAttribute("href",
                    string.Empty, true);
            }
            var htmlContentBuilder = item.InnerHtml.AppendHtml(next);
            return item;
        }

        private static TagBuilder Last(IPagedResult metadata,
            Func<int, string> generatePageUrl,
            NameValueCollection routedValues,
            PaginationInfo settings)
        {
            var item = new TagBuilder("li");
            item.AddCssClass(settings.ArrowClass);
            var targetPageNumber = metadata.TotalPages;
            var last = new TagBuilder("a");
            last.InnerHtml.Append(settings.LastDisplay);
            if (metadata.Page == metadata.TotalPages)
            {
                item.AddCssClass(settings.UnavailableClass);
                last.MergeAttribute("href",
                    string.Empty, true);
            }
            else
            {
                last.MergeAttribute("href",
                    generatePageUrl(targetPageNumber)
                    + GetRoutedValues(routedValues,
                    settings.PageVariableName), true);
            }
            var htmlContentBuilder = item.InnerHtml.AppendHtml(last);
            return item;
        }

        private static TagBuilder PageCountAndLocationText(IPagedResult metadata, PaginationInfo settings)
        {
            var item = new TagBuilder("li");
            item.AddCssClass(settings.UnavailableClass);
            var text = new TagBuilder("a");

            text.InnerHtml.AppendHtml(string.Format(settings.PageCountAndLocationFormat, metadata.Page, metadata.TotalPages));
            text.MergeAttribute("href",
                string.Empty, true);
            var htmlContentBuilder = item.InnerHtml.AppendHtml(text);
            return item;
        }

        private static TagBuilder ItemSliceAndTotalText(IPagedResult metadata, PaginationInfo settings)
        {
            var item = new TagBuilder("li");
            item.AddCssClass(settings.UnavailableClass);

            int FirstItemOnPage = (metadata.Page - 1) * metadata.PageSize + 1;
            var numberOfLastItemOnPage = FirstItemOnPage + metadata.PageSize - 1;
            int LastItemOnPage = numberOfLastItemOnPage > metadata.TotalRecords
                                ? metadata.TotalRecords
                                : numberOfLastItemOnPage;

            var text = new TagBuilder("a");
            text.InnerHtml.AppendHtml(string.Format(settings.ItemSliceAndTotalFormat, FirstItemOnPage, LastItemOnPage, metadata.TotalRecords));
            text.MergeAttribute("href",
                string.Empty, true);
            item.InnerHtml.AppendHtml(text);
            return item;
        }

        private static TagBuilder EllipsesPrevious(IPagedResult metaData, Func<int, string> generatePageUrl, NameValueCollection routedValues, PaginationInfo settings)
        {
            var targetPageNumber = metaData.Page - (settings.MaximumPageNumbersToDisplay ?? 10);
            if (targetPageNumber < 1) targetPageNumber = 1;

            var item = new TagBuilder("li");
            var a = new TagBuilder("a");
            a.InnerHtml.Append(settings.Ellipses);
            if (targetPageNumber == metaData.Page)
            {
                item.AddCssClass(settings.UnavailableClass);
                a.MergeAttribute("href",
                string.Empty, true);
            }
            else
            {
                a.MergeAttribute("href",
                      generatePageUrl(targetPageNumber)
                      + GetRoutedValues(routedValues,
                      settings.PageVariableName), true);
            }
            var htmlContentBuilder = item.InnerHtml.AppendHtml(a);
            return item;
        }

        private static TagBuilder EllipsesNext(IPagedResult metaData, Func<int, string> generatePageUrl, NameValueCollection routedValues, PaginationInfo settings)
        {
            var targetPageNumber = metaData.Page + (settings.MaximumPageNumbersToDisplay ?? 10);
            if (targetPageNumber > metaData.TotalPages) targetPageNumber = metaData.TotalPages;

            var item = new TagBuilder("li");
            var a = new TagBuilder("a");
            a.InnerHtml.Append(settings.Ellipses);
            if (targetPageNumber == metaData.Page)
            {
                item.AddCssClass(settings.UnavailableClass);
                a.MergeAttribute("href",
                string.Empty, true);
            }
            else
            {
                a.MergeAttribute("href",
                      generatePageUrl(targetPageNumber)
                      + GetRoutedValues(routedValues,
                      settings.PageVariableName), true);
            }
            var htmlContentBuilder = item.InnerHtml.AppendHtml(a);
            return item;
        }

        private static TagBuilder Page(int i, IPagedResult metaData, Func<int, string> generatePageUrl, NameValueCollection routedValues, PaginationInfo settings)
        {
            var item = new TagBuilder("li");

            var targetPageNumber = i;
            var page = new TagBuilder("a");
            page.InnerHtml.AppendHtml(string.Format(settings.PageDisplayFormat, i));

            if (metaData.Page == i)
            {
                item.AddCssClass(settings.CurrentClass);
                item.AddCssClass(settings.UnavailableClass);
                page.MergeAttribute("href",
                    string.Empty, true);
            }
            else
            {
                page.MergeAttribute("href",
                   generatePageUrl(targetPageNumber)
                   + GetRoutedValues(routedValues,
                   settings.PageVariableName), true);
            }

            var htmlContentBuilder = item.InnerHtml.AppendHtml(page);
            return item;
        }

        private static TagBuilder Previous(IPagedResult metaData, Func<int, string> generatePageUrl, NameValueCollection routedValues, PaginationInfo settings)
        {
            var item = new TagBuilder("li");
            item.AddCssClass(settings.ArrowClass + " previous");
            var targetPageNumber = metaData.Page - 1;
            var previous = new TagBuilder("a");
            previous.InnerHtml.Append(settings.PreviousDisplay);
            if (targetPageNumber < 1)
            {
                item.AddCssClass(settings.UnavailableClass);
                previous.MergeAttribute("href",
                    string.Empty, true);
            }
            else
            {
                previous.MergeAttribute("href",
                       generatePageUrl(targetPageNumber)
                       + GetRoutedValues(routedValues,
                       settings.PageVariableName), true);
            }

            var htmlContentBuilder = item.InnerHtml.AppendHtml(previous);
            return item;
        }

        private static TagBuilder First(IPagedResult metaData, Func<int, string> generatePageUrl, NameValueCollection routedValues, PaginationInfo settings)
        {
            var item = new TagBuilder("li");

            const int targetPageNumber = 1;
            var first = new TagBuilder("a");
            first.InnerHtml.Append(settings.FirstDisplay);
            if (metaData.Page == targetPageNumber)
            {
                item.AddCssClass(settings.UnavailableClass);
                first.MergeAttribute("href",
                    string.Empty, true);
            }
            else
            {
                first.MergeAttribute("href",
                     generatePageUrl(targetPageNumber)
                     + GetRoutedValues(routedValues,
                     settings.PageVariableName), true);
            }

            var htmlContentBuilder = item.InnerHtml.AppendHtml(first);
            return item;
        }

        #endregion Private Zurb Pagination

        #endregion Pagination

        public class PaginationInfo
        {
            private IPagedResult<object> _collection = null;

            public virtual IPagedResult<object> Collection
            {
                get { return _collection; }
                set { _collection = value; }
            }

            private string _ElementClass = "pagination";
            private string _CurrentClass = "active";
            private string _UnavailableClass = "disabled";
            private string _ArrowClass = "";
            private string _Ellipses = "...";
            private string _PageVariableName = "p";
            private string _PageDisplayFormat = "{0}";
            private string _NextDisplay = ">";
            private string _LastDisplay = ">>";
            private string _PreviousDisplay = "<";
            private string _FirstDisplay = "<<";
            private string _PageCountAndLocationFormat = "{0} of {1}";
            private string _ItemSliceAndTotalFormat = "{0} to {1} of {2}";
            private bool _pagerInChunks = false;
            private int _chunkCount = 10;

            [DefaultValue("{0} to {1} of {2}")]
            public virtual string ItemSliceAndTotalFormat
            {
                get { return _ItemSliceAndTotalFormat; }
                set { _ItemSliceAndTotalFormat = value; }
            }

            [DefaultValue("{0} of {1}")]
            public virtual string PageCountAndLocationFormat
            {
                get { return _PageCountAndLocationFormat; }
                set { _PageCountAndLocationFormat = value; }
            }

            [DefaultValue(">")]
            public virtual string NextDisplay
            {
                get { return _NextDisplay; }
                set { _NextDisplay = value; }
            }

            [DefaultValue(">>")]
            public virtual string LastDisplay
            {
                get { return _LastDisplay; }
                set { _LastDisplay = value; }
            }

            [DefaultValue("<")]
            public virtual string PreviousDisplay
            {
                get { return _PreviousDisplay; }
                set { _PreviousDisplay = value; }
            }

            [DefaultValue("<<")]
            public virtual string FirstDisplay
            {
                get { return _FirstDisplay; }
                set { _FirstDisplay = value; }
            }

            [DefaultValue("{0}")]
            public virtual string PageDisplayFormat
            {
                get { return _PageDisplayFormat; }
                set { _PageDisplayFormat = value; }
            }

            [DefaultValue("page")]
            public virtual string PageVariableName
            {
                get { return _PageVariableName; }
                set { _PageVariableName = value; }
            }

            [DefaultValue("pagination")]
            public virtual string ElementClass
            {
                get { return _ElementClass; }
                set { _ElementClass = value; }
            }

            [DefaultValue("current")]
            public virtual string CurrentClass
            {
                get { return _CurrentClass; }
                set { _CurrentClass = value; }
            }

            [DefaultValue("unavailable")]
            public virtual string UnavailableClass
            {
                get { return _UnavailableClass; }
                set { _UnavailableClass = value; }
            }

            [DefaultValue("arrow")]
            public virtual string ArrowClass
            {
                get { return _ArrowClass; }
                set { _ArrowClass = value; }
            }

            [DefaultValue("...")]
            public string Ellipses
            {
                get { return _Ellipses; }
                set { _Ellipses = value; }
            }

            [DefaultValue(false)]
            public bool PagerInChunks
            {
                get { return _pagerInChunks; }
                set { _pagerInChunks = value; }
            }

            public int ChunkCount
            {
                get { return _chunkCount; }
                set { _chunkCount = value; }
            }

            #region List Render options

            ///<summary>
            /// When true, includes a hyperlink to the first page of the list.
            ///</summary>
            public bool DisplayLinkToFirstPage { get; set; }

            ///<summary>
            /// When true, includes a hyperlink to the last page of the list.
            ///</summary>
            public bool DisplayLinkToLastPage { get; set; }

            ///<summary>
            /// When true, includes a hyperlink to the previous page of the list.
            ///</summary>
            public bool DisplayLinkToPreviousPage { get; set; }

            ///<summary>
            /// When true, includes a hyperlink to the next page of the list.
            ///</summary>
            public bool DisplayLinkToNextPage { get; set; }

            ///<summary>
            /// When true, includes hyperlinks for each page in the list.
            ///</summary>
            public bool DisplayLinkToIndividualPages { get; set; }

            ///<summary>
            /// When true, shows the current page number and the total number of pages in the list.
            ///</summary>
            ///<example>
            /// "Page 3 of 8."
            ///</example>
            public bool DisplayPageCountAndCurrentLocation { get; set; }

            ///<summary>
            /// When true, shows the one-based index of the first and last items on the page, and the total number of items in the list.
            ///</summary>
            ///<example>
            /// "Showing items 75 through 100 of 183."
            ///</example>
            public bool DisplayItemSliceAndTotal { get; set; }

            ///<summary>
            /// The maximum number of page numbers to display. Null displays all page numbers.
            ///</summary>
            public int? MaximumPageNumbersToDisplay { get; set; }

            ///<summary>
            /// If true, adds an ellipsis where not all page numbers are being displayed.
            ///</summary>
            ///<example>
            /// "1 2 3 4 5 ...",
            /// "... 6 7 8 9 10 ...",
            /// "... 11 12 13 14 15"
            ///</example>
            public bool DisplayEllipsesWhenNotShowingAllPageNumbers { get; set; }

            #endregion List Render options

            ///<summary>
            /// Also includes links to First and Last pages.
            ///</summary>
            public static PaginationInfo DefaultPlusFirstAndLast
            {
                get
                {
                    var result =
                        new PaginationInfo
                        {
                            DisplayLinkToFirstPage = true,
                            DisplayLinkToLastPage = true,
                            DisplayPageCountAndCurrentLocation = true,
                            PagerInChunks = false,
                        };
                    return result;
                }
            }

            ///<summary>
            /// Shows only the Previous and Next links.
            ///</summary>
            public static PaginationInfo Minimal
            {
                get
                {
                    var result =
                        new PaginationInfo
                        {
                            DisplayLinkToNextPage = true,
                            DisplayLinkToPreviousPage = true,
                            PagerInChunks = false,
                        };
                    return result;
                }
            }

            ///<summary>
            /// Shows Previous and Next links along with current page number and page count.
            ///</summary>
            public static PaginationInfo MinimalWithPageCountText
            {
                get
                {
                    var result =
                        new PaginationInfo
                        {
                            DisplayLinkToNextPage = true,
                            DisplayLinkToPreviousPage = true,
                            DisplayPageCountAndCurrentLocation = true,
                            PagerInChunks = false
                        };
                    return result;
                }
            }

            ///<summary>
            ///	Shows Previous and Next links along with index of first and last items on page and total number of items across all pages.
            ///</summary>
            public static PaginationInfo MinimalWithItemCountText
            {
                get
                {
                    var result =
                        new PaginationInfo
                        {
                            DisplayLinkToNextPage = true,
                            DisplayLinkToPreviousPage = true,
                            DisplayItemSliceAndTotal = true,
                            PagerInChunks = false
                        };

                    return result;
                }
            }

            public static PaginationInfo MinimalWithPages
            {
                get
                {
                    var result =
                        new PaginationInfo
                        {
                            DisplayLinkToFirstPage = false,
                            DisplayLinkToLastPage = false,
                            DisplayLinkToPreviousPage = true,
                            DisplayLinkToNextPage = true,
                            DisplayEllipsesWhenNotShowingAllPageNumbers = false,
                            DisplayPageCountAndCurrentLocation = false,
                            PagerInChunks = false,
                            DisplayLinkToIndividualPages = true,
                            MaximumPageNumbersToDisplay = 10,
                        };
                    return result;
                }
            }

            public static PaginationInfo DefaultPager
            {
                get
                {
                    var result =
                        new PaginationInfo
                        {
                            DisplayLinkToNextPage = true,
                            DisplayLinkToPreviousPage = true,
                            DisplayPageCountAndCurrentLocation = true,
                            DisplayLinkToIndividualPages = true,
                            MaximumPageNumbersToDisplay = 10,
                            DisplayEllipsesWhenNotShowingAllPageNumbers = true,
                            PagerInChunks = false,
                        };
                    return result;
                }
            }

            ///<summary>
            ///	Shows only links to each individual page.
            ///</summary>
            public static PaginationInfo PageNumbersOnly
            {
                get
                {
                    var result =
                        new PaginationInfo
                        {
                            DisplayLinkToFirstPage = false,
                            DisplayLinkToLastPage = false,
                            DisplayLinkToPreviousPage = false,
                            DisplayLinkToNextPage = false,
                            DisplayEllipsesWhenNotShowingAllPageNumbers = false,
                            PagerInChunks = false,
                        };
                    return result;
                }
            }

            public static PaginationInfo PagerInChucks
            {
                get
                {
                    var result =
                        new PaginationInfo
                        {
                            DisplayLinkToFirstPage = false,
                            DisplayLinkToLastPage = false,
                            DisplayLinkToPreviousPage = true,
                            DisplayLinkToNextPage = true,
                            DisplayEllipsesWhenNotShowingAllPageNumbers = false,
                            PagerInChunks = true,
                        };
                    return result;
                }
            }
        }

        public static string GetString(this IHtmlContent content)
        {
            var writer = new System.IO.StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }
    }

    internal static class PaginationHelperExtensions
    {
        public static NameValueCollection ToNameValueCollection(this QueryString query)
        {
            var result = new NameValueCollection();
            if (!query.HasValue) return result;
            var q = query.Value.TrimStart('?').Split('&');
            var dict = q.Select(x => Tuple.Create(x.Split('=')[0], (x.Split('=').Length > 1) ? x.Split('=')[1] : string.Empty))
                  .GroupBy(t => t.Item1).Where(g => !string.IsNullOrWhiteSpace(g.Key))
                  .ToDictionary(x => x.Key, x => x.Where(v => !string.IsNullOrWhiteSpace(v.Item2)).Select(v => v.Item2).ToArray());
            foreach (var dictKey in dict.Keys)
            {
                foreach (var value in dict[dictKey])
                {
                    result.Add(dictKey, value);
                }
            }
            return result;
        }
    }
}