using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using System;

namespace Borg.Client
{
    /// <summary>
    /// Extensions for providing nested razor Section
    /// http://blogs.msdn.com/b/marcinon/archive/2010/12/15/razor-nested-layouts-and-redefined-sections.aspx
    /// </summary>
    public static class SectionExtensions
    {
        private static readonly object _o = new object();

        #region Render

        public static HelperResult RenderSection(this RazorPage page,
            string sectionName,
            Func<object, HelperResult> defaultContent)
        {
            if (page.IsSectionDefined(sectionName))
                return new HelperResult(_ => _.WriteAsync(page.RenderSection(sectionName).ToString()));
            else return defaultContent(_o);
        }

        public static HelperResult RenderSection(this RazorPage page,
              string sectionName,
              string defaultContent)
        {
            if (page.IsSectionDefined(sectionName))
                return new HelperResult(_ => _.WriteAsync(page.RenderSection(sectionName).ToString()));
            else return new HelperResult(a => a.WriteAsync(defaultContent));
        }

        public static HelperResult RenderSection(this RazorPage page,
              string sectionName,
              HtmlString defaultContent)
        {
            if (page.IsSectionDefined(sectionName))
                return new HelperResult(_ => _.WriteAsync(page.RenderSection(sectionName).ToString()));
            else return new HelperResult(a => a.WriteAsync(defaultContent.ToString()));
        }

        #endregion Render

        #region Redefine

        public static HelperResult RedefineSection(this RazorPage page, string sectionName)
        {
            return RedefineSection(page, sectionName, defaultContent: null);
        }

        public static HelperResult RedefineSections(this RazorPage page, string sectionNames)
        {
            HelperResult a = null;
            foreach (var sectionName in (sectionNames ?? "").Split(','))
            {
                a = RedefineSection(page, sectionName, defaultContent: null);
            }
            return a;
        }

        public static HelperResult RedefineSection(this RazorPage page, string sectionName, Func<object, HelperResult> defaultContent)
        {
            if (page.IsSectionDefined(sectionName))
            {
                page.DefineSection(sectionName, _ => _.WriteAsync(page.RenderSection(sectionName).ToString()));
            }
            else if (defaultContent != null)
            {
                page.DefineSection(sectionName, _ => _.WriteAsync(defaultContent(_o).ToString()));
            }
            return new HelperResult(_ => _.WriteAsync(string.Empty));
        }

        public static HelperResult RedefineSections(this RazorPage page)
        {
            var sections = page.PreviousSectionWriters;
            if (sections != null)
                foreach (var item in sections)
                    page.RedefineSection(item.Key);
            return new HelperResult(_ => _.WriteAsync(string.Empty));
        }

        #endregion Redefine
    }
}