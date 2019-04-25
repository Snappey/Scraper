using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Scraper.Structures;

namespace Crawler.UI
{
    /// <summary>
    /// Each class represents a line on the console, one unit of information that  is redrawn each time as required. This is managed by ConsoleManager.cs based on the ConsoleHeight
    /// </summary>
    class Line
    {
        private string text; // TODO: Assign each line a state enum which declares what it is used for, Output, Information, Title.
                             // For output we can iterate through a fixed size queue and check based on the enum for the line to determine if we output
                             // Information, will primarily be used for status on site scraping and data gathering. This can be used as output if the line is not bound to any site
        private string border;
        private int row;
        private LineType type;

        public Site Site;

        public Line(int row, string text="")
        {
            this.row = row;
            this.text = text;
            this.border = "|";
        }

        private void TypeChanged()
        {
            if (type == LineType.Divider)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("|");
                stringBuilder.Append(string.Concat(Enumerable.Repeat("-", Console.BufferWidth - 2)));
                stringBuilder.Append("|");
                this.text = stringBuilder.ToString();
            }
        }

        public void Update()
        {
            int x = Console.CursorLeft; // Cache position before update
            int y = Console.CursorTop;
            int maxW = Console.BufferWidth;

            Console.SetCursorPosition(0, row); // Change to position of line

            if (type == LineType.Output)
            {
                if (text.Length >= maxW)
                {
                    Console.Write(border + text.Substring(0, maxW - (border.Length * 2)) + border);
                }
                else
                {
                    string spacing = string.Concat(Enumerable.Repeat(" ", maxW - text.Length - (border.Length * 2)));
                    Console.Write(border + text + spacing + border);
                }
            }
            else
            {
                if (type == LineType.Information)
                {
                    if (Site != null)
                    {
                        TimeSpan time;
                        if (Site.Status != SiteStatus.Pending)
                        {
                            if (Site.SiteFinished == DateTime.MinValue)
                            {
                                time = DateTime.Now.Subtract(Site.SiteStart).Duration();
                            }
                            else
                            {
                                time = Site.SiteFinished.Subtract(Site.SiteStart).Duration();
                            }
                        }
                        else
                        {
                            time = TimeSpan.Zero;
                        }


                        string content = Site.URL + " - " + Site.Status + " (" + string.Format("{0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds) + ")";
                        if (content.Length >= maxW)
                        {
                            Console.Write(border + content.Substring(0, maxW - content.Length - (border.Length * 2)) + border);
                        }
                        else
                        {
                            string spacing = string.Concat(Enumerable.Repeat(" ", maxW - content.Length - (border.Length * 2)));
                            Console.Write(border + content + spacing + border);
                        }
                        
                    }
                    else
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("|");
                        stringBuilder.Append(string.Concat(Enumerable.Repeat(" ", Console.BufferWidth - 2)));
                        stringBuilder.Append("|");
                        Console.Write(stringBuilder.ToString());
                    }
                }
                else
                {
                    Console.Write(text);
                }            
            }


            Console.SetCursorPosition(x,y); // Restore position back to original
        }

        public void SetText(string text)
        {
            this.text = text;
        }

        public void SetLineType(LineType type)
        {
            this.type = type;
            TypeChanged();
        }

        public LineType GetLineType()
        {
            return type;
        }
    }

    enum LineType
    {
        Output,
        Information,
        Title,
        Divider,
    }
}
