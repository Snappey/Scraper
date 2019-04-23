
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Crawler.UI;
using Scraper.Structures;

namespace Crawler
{
    /// <summary>
    /// Manages the UI Section of the Console Application
    /// </summary>
    class ConsoleManager
    {
        private Line[] lines;

        private List<Site> sites;

        private int defX;
        private int defY;

        public ConsoleManager(int consoleWidth, int consoleHeight)
        {
            Console.BufferWidth = consoleWidth;
            Console.BufferHeight = consoleHeight;

            defX = 2;
            defY = consoleHeight - 1;

            Console.Clear();
            // Create an array of UI/Line.cs which represents each line of the console.
            // States based on the command ran influences what is drawn(?)

            Setup();

            Thread thread = new Thread(ThreadStart =>
            {
                while (true)
                {
                    Thread.Sleep(250);
                    Draw();
                }
            });

            Thread testThread = new Thread(ThreadStart =>
            {
                while (true)
                {
                    Thread.Sleep(500);

                    StringBuilder title = new StringBuilder(Console.BufferWidth);
                    title.Append("Hotel Scraping v1.0");
                    title.Append("" + string.Concat(Enumerable.Repeat(" ", Console.BufferWidth - 27)));
                    title.Append(DateTime.Now.ToLongTimeString());

                    lines[0].SetText(title.ToString());
                }
            });

            thread.Start();
            testThread.Start();
        }

        private void Setup()
        {
            lines = new Line[Console.BufferHeight  - 1];

            for (int i = 0; i < Console.BufferHeight - 1; i++)
            {
                lines[i] = new Line(i);
            }

            Console.SetCursorPosition(defX, defY);
        }

        private void Draw()
        {
            // Main Drawing Loop
            Console.CursorVisible = false;

            for (int i = 0; i < Console.BufferHeight - 1; i++)
            {
                var line = lines[i];

                line.Update();
            }

            Console.CursorVisible = true;

        }

        public void Log(string text, LogType type, Site site = null)
        {
            // Outputs to main log in the centre screen
            // Console.WriteLine etc. should be routed here
        }

        public void Attach(Site site)
        {
            if (sites.Contains(site) == false)
            {
                sites.Add(site);

                site.LogReceived += (sender, args) =>
                {
                    var castArgs = (LogEventArgs) args;
                    Log(castArgs.Log, castArgs.Type, site);
                };
            }       
        }
    }
}
