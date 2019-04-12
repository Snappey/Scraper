using System;
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

        public Pipeline()
        {
            
        }

        public void Output(List<NodeResult> outputNodes, Site site, string page)
        {
            //PipelineOutput outputType = outputNodes.First().Site.OutputType;

            switch (site.OutputType)
            {
                case PipelineOutput.Plaintext:
                    PlaintextHandler(outputNodes, site, page);
                    break;
                case PipelineOutput.Json:
                    JsonHandler(outputNodes, site, page);
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

        private void WriteFile(Site site, string filename, string output)
        {
            string BasePath = Environment.CurrentDirectory; // File Structure: ExecDir/data/url/files (index is '-', '\' is also replaced by '-')
            if (Directory.Exists(BasePath + "\\data") == false) {
                Directory.CreateDirectory(BasePath + "\\data"); }
            if (Directory.Exists(BasePath + "\\data\\" + site.URL.Host) == false) {
                Directory.CreateDirectory(BasePath + "\\data\\" + site.URL.Host);}
            File.WriteAllText($"{BasePath}\\data\\{site.URL.Host}\\{filename}.txt", output);
        }
    }

    public enum PipelineOutput
    {
        Plaintext,
        Json
    }
}
