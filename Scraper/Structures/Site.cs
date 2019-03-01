using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Structures
{
    public class Site
    {
        public Uri URL;
        public Dictionary<string, PageLayout> Pages = new Dictionary<string, PageLayout>();

        #region Config Options
        public bool IncludeIndex;
        public bool ScrapeLinks;
        public bool ScrapeImages;
        public PipelineOutput OutputType;
        #endregion

        public Site(Uri uri, bool incidx = true, bool scplnks = false, bool scpimgs = true, PipelineOutput outputtype = PipelineOutput.Plaintext)
        {
            URL = uri;

            IncludeIndex = incidx;
            ScrapeLinks = scplnks;
            ScrapeImages = scplnks;
            OutputType = outputtype;
        }

        public PageLayout AddPage(string page)
        {
            PageLayout pageLayout = new PageLayout(URL, page);
            Pages.Add(page, pageLayout);
            return pageLayout;
        }

        /*public void AddPages(List<string> pages)
        {
            pages.ForEach((pg) => { Pages.Add(pg, new PageLayout(URL, pg)); });
        }*/

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("| Site Layout");
            stringBuilder.AppendLine("| Base: " + URL.AbsoluteUri);
            foreach(PageLayout page in Pages.Values)
            {
                stringBuilder.AppendLine("| -> /" + page.Path);
            }

            return stringBuilder.ToString();
        }
    }
}
