using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Scraper.Structures;

namespace Scraper
{
    class Pipeline
    {
        // Site (www.dotnetperls.com) -> Page (www.dotnetperls.com/index -> Property (Description) -> NodeResult List
        public Dictionary<Site, Dictionary<string, Dictionary<string, NodeResult>>> Data;

        public Pipeline()
        {
            Data = new Dictionary<Site, Dictionary<string, Dictionary<string, NodeResult>>>();
        }

        public void Output(List<NodeResult> outputNodes, Site site, string page)
        {
            switch (site.OutputType)
            {
                case PipelineOutput.Plaintext:
                    PlaintextHandler(outputNodes, site, page);
                    break;
                case PipelineOutput.Json:
                    JsonHandler(outputNodes, site, page);
                    break;
                case PipelineOutput.Object:
                    ObjectHandler(outputNodes, site, page);
                    break;
                default:
                    throw new NotImplementedException("Handler has not been implemented");
            }
        }

        private void PlaintextHandler(List<NodeResult> outputNodes, Site site, string page)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("| > " + site.URL + " - { TODO IMPLEMENT XPATH }");

            foreach (NodeResult node in outputNodes)
            {
                stringBuilder.AppendLine("|  -> " + node.Property);

                foreach (HtmlNode htmlNode in node.Nodes)
                {
                    stringBuilder.AppendLine("|    --> " + htmlNode.InnerHtml);
                }
            }

            WriteFile(site, page.Replace('/','-'), stringBuilder.ToString());
        }

        private void JsonHandler(List<NodeResult> outputNodes, Site site, string page)
        {

        }

        private void ObjectHandler(List<NodeResult> outputNodes, Site site, string page)
        {
            if (Data.ContainsKey(site) == false)
            {
                Data.Add(site, new Dictionary<string, Dictionary<string, NodeResult>>());
            }

            if (Data[site].ContainsKey(page) == false)
            {
                Data[site].Add(page, new Dictionary<string, NodeResult>());

                var results = outputNodes.Where(pg => pg.Page == page);
                foreach (NodeResult result in results)
                {
                    Data[site][page].Add(result.Property, result);
                }
            }
        }

        private void WriteFile(Site site, string filename, string output)
        {
            string BasePath = Environment.CurrentDirectory; // File Structure: ExecDir/data/url/files (index is '-', '\' is also replaced by '-')
            if (Directory.Exists(BasePath + "\\data") == false) {
                Directory.CreateDirectory(BasePath + "\\data");}
            if (Directory.Exists(BasePath + "\\data\\" + site.URL.Host) == false) {
                Directory.CreateDirectory(BasePath + "\\data\\" + site.URL.Host);}
            File.WriteAllText($"{BasePath}\\data\\{site.URL.Host}\\{filename}.txt", output);
        }
    }

    public enum PipelineOutput
    {
        Plaintext,
        Json,
        Object,
    }
}
