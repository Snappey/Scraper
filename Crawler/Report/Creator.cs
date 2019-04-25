using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using RazorLight;
using Crawler.Report.Models;
using Microsoft.CodeAnalysis.CSharp;


namespace Crawler.Report
{
    class Creator
    {
        private string name;


        public Creator(string name)
        {
            this.name = name;

            Test();
        }

        private async void Test()
        {
            var engine = new RazorLightEngineBuilder()
                .UseMemoryCachingProvider()
                .UseFilesystemProject(Environment.CurrentDirectory + "/Report/Templates/")
                .Build();

            string template = "Testing, @Model.Name <br> This generated";
            TestViewModel model = new TestViewModel
            {
                Name = name,
            };

            string result = await engine.CompileRenderAsync("Test.cshtml", model);


            Console.WriteLine(result);
        }
    }
}
