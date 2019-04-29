using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RazorLight;
using Crawler.Report.Models;
using Crawler.Structures;
using Microsoft.CodeAnalysis.CSharp;


namespace Crawler.Report
{
    class Creator
    {
        private string name;

        public async Task<string> Create(RequestArgs args, List<Hotel> hotels)
        {
            var engine = new RazorLightEngineBuilder()
                .UseMemoryCachingProvider()
                .UseFilesystemProject(Environment.CurrentDirectory + "/Report/Templates/")
                .Build();

            ReportViewModel model = new ReportViewModel
            {
                SearchLocation = args.City,
                CheckIn = args.CheckIn.ToShortDateString(),
                CheckOut = args.CheckOut.ToShortDateString(),
                PersonAmount = args.People,
                RoomAmount = args.Rooms,
                Gathered = DateTime.Now,
                Hotels = hotels
            };

            string result = await engine.CompileRenderAsync("Template.cshtml", model);

            Directory.CreateDirectory(Environment.CurrentDirectory + "/Reports/");

            string filePath = Environment.CurrentDirectory + $"\\Reports\\{args.City}-{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}-{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}.html";

            File.WriteAllText(filePath, result);

            return filePath;
        }
    }
}
