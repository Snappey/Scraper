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

        public void Output(List<NodeResult> outputNodes)
        {
            PipelineOutput outputType = outputNodes.First().Site.OutputType;

            switch (outputType)
            {
                case PipelineOutput.Plaintext:
                    PlaintextHandler(outputNodes);
                    break;
                case PipelineOutput.Json:
                    JsonHandler(outputNodes);
                    break;
                default:
                    throw new NotImplementedException("Handler has not been implemented");
            }
        }

        private void PlaintextHandler(List<NodeResult> outputNodes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Site site = outputNodes.First().Site;
            stringBuilder.AppendLine("| > " + site.URL + " - {" + outputNodes.First().Nodes.First().XPath + "}");

            foreach (NodeResult node in outputNodes)
            {
                stringBuilder.AppendLine("|  -> " + node.Property);

                foreach (HtmlNode htmlNode in node.Nodes)
                {
                    stringBuilder.AppendLine("|    --> " + htmlNode.InnerHtml);
                }
            }

            WriteFile(site, outputNodes.First().Page.Replace('/','-'),stringBuilder.ToString());
        }

        private void JsonHandler(List<NodeResult> outputNodes)
        {

        }

        private void WriteFile(Site site, string filename, string output)
        {
            if (Directory.Exists(@"C:\Users\Jon\Source\Repos\Scraper\Scraper\bin\Debug\netcoreapp2.0\" + site.URL.Host) == false) { Directory.CreateDirectory(@"C:\Users\Jon\Source\Repos\Scraper\Scraper\bin\Debug\netcoreapp2.0\" + site.URL.Host);}

            File.WriteAllText(@"C:\Users\Jon\Source\Repos\Scraper\Scraper\bin\Debug\netcoreapp2.0\" + site.URL.Host + "\\" + filename + ".txt", output);
        }
    }

    public enum PipelineOutput
    {
        Plaintext,
        Json
    }
}
