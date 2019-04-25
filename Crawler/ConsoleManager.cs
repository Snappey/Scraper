using System;
using System.Collections.Generic;
using System.IO;
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
        private List<Site> sites = new List<Site>();
        private Queue<string> outputQueue = new Queue<string>();

        private int defX;
        private int defY;

        public ConsoleManager(int consoleWidth, int consoleHeight)
        {
            Console.WindowWidth = consoleWidth;
            Console.WindowHeight = consoleHeight;
            Console.BufferHeight = consoleHeight;

            defX = 2;
            defY = consoleHeight - 1;

            Console.Clear();
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
                    title.Append("|Hotel Scraping v1.0");
                    title.Append(string.Concat(Enumerable.Repeat(" ", Console.BufferWidth - 29)));
                    title.Append(DateTime.Now.ToLongTimeString() + "|");

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

                if (i == 0)
                {
                    lines[i].SetLineType(LineType.Title);
                }

                if (i == 1)
                {
                    lines[i].SetLineType(LineType.Divider);
                }

                if (i > 1 && i < 12)
                {
                    lines[i].SetLineType(LineType.Information);
                }

                if (i == 12)
                {
                    lines[i].SetLineType(LineType.Divider);
                }

                if (i >= 13 && i < Console.BufferHeight - 2)
                {
                    lines[i].SetLineType(LineType.Output);
                }

                if (i == Console.BufferHeight - 2)
                {
                    lines[Console.BufferHeight - 2].SetLineType(LineType.Divider);
                }                
            }

            Console.SetCursorPosition(0, defY);
            Console.Write("|>");
            Console.SetCursorPosition(defX, defY);
        }

        private void Draw()
        {
            // Main Drawing Loop
            Console.CursorVisible = false;
            var oldIn = Console.In;
            Console.SetIn(TextReader.Null);

            DrawLog();

            for (int i = 0; i < Console.BufferHeight - 1; i++)
            {
                var line = lines[i];

                line.Update();   
            }

            Console.SetIn(oldIn);
            Console.CursorVisible = true;

        }

        private void DrawLog()
        {
            var output = outputQueue.ToArray();
            var lines = GetLines(LineType.Output);

            for (var i = 0; i < lines.Count; i++)
            {
                Line line = lines[i];

                if (i < output.Length)
                {
                    line.SetText(output[i]);
                }
            }
        }

        public void ResetCursor()
        {
            Console.SetCursorPosition(0, defY);
            Console.Write("|>");
            Console.SetCursorPosition(defX, defY);
        }

        public void Log(string text, LogType type, Site site = null)
        {
            // Outputs to main log in the centre screen, Only insert it into Output lines, for loop reverse so bottom to top of screen
            // Layer between Log and Console, fixed size queue and update the Line by given index from the for loop
            // Console.WriteLine etc. should be routed here

            outputQueue.Enqueue(text);
            var outputLines = GetLines(LineType.Output);

            while (outputQueue.Count > outputLines.Count)
            {
                outputQueue.Dequeue();
            }

        }

        public void Attach(Site site)
        {
            if (sites.Contains(site) == false)
            {
                sites.Add(site);

                var lines = GetLines(LineType.Information);
                AttachInfo(lines, site);

                site.LogReceived += (sender, args) =>
                {
                    var castArgs = (LogEventArgs) args;
                    Log(castArgs.Log, castArgs.Type, site);
                };
            }       
        }

        private void AttachInfo(List<Line> lines, Site site)
        {
            foreach (Line line in lines)
            {
                if (line.Site == null)
                {
                    line.Site = site;
                    break;
                } 
            }
        }

        private List<Line> GetLines(LineType type)
        {
            List<Line> res = new List<Line>();
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.GetLineType() == type)
                {
                    res.Add(line);
                }
            }

            return res;
        }
    }
}
