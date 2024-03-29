﻿using System;
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
        // Site (www.dotnetperls.com) -> Page (www.dotnetperls.com/index -> NodeResult List
        public Dictionary<Site, Dictionary<string, List<List<NodeResult>>>> Data;

        public Pipeline()
        {
            Data = new Dictionary<Site, Dictionary<string, List<List<NodeResult>>>>();
        }

        public void Output(List<NodeResult> outputNodes, Site site, string page)
        {
            switch (site.OutputType)
            {
                case PipelineOutput.Plaintext:
                    PlaintextHandler(outputNodes, site, page);
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
            stringBuilder.AppendLine("| > " + site.URL);

            foreach (NodeResult node in outputNodes) // Used for debugging, outputs the structure from the website found using the custom page layouts
            {
                stringBuilder.AppendLine("|  -> " + node.Property);

                foreach (HtmlNode htmlNode in node.Nodes)
                {
                    stringBuilder.AppendLine("|    --> " + htmlNode.InnerHtml);
                }
            }

            WriteFile(site, page.Replace('/','-'), stringBuilder.ToString());
        }

        private void ObjectHandler(List<NodeResult> outputNodes, Site site, string page)
        {
            if (Data.ContainsKey(site) == false)
            {
                Data.Add(site, new Dictionary<string, List<List<NodeResult>>>());
            }

            if (Data[site].ContainsKey(page) == false)
            {
                Data[site].Add(page, new List<List<NodeResult>>()); // Store the results into the data structure so it can be accessed later using information the user has available
                Data[site][page].Add(outputNodes);
            }
            else
            {
                Data[site][page].Add(outputNodes);
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
