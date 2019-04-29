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
        private string text;
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

        /// <summary>
        /// Called whenever the line has its type set
        /// </summary>
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

        /// <summary>
        /// Updates the line contents to reflect any changes, ensures that the formatting is kept within the limits of the line
        /// </summary>
        public void Update()
        {
            int x = Console.CursorLeft; // Cache position before update
            int y = Console.CursorTop;
            int maxW = Console.BufferWidth;

            Console.SetCursorPosition(0, row); // Change to position of line

            if (type == LineType.Output) // used for formatting the console output
            {
                if (text.Length >= maxW)
                {
                    Console.Write(border + text.Substring(0, maxW - (border.Length * 2)) + border);
                }
                else
                {
                    string spacing = string.Concat(Enumerable.Repeat(" ", maxW - text.Length - (border.Length * 2)));
                    Console.Write(border + text + spacing + border);
                } // Ensure that line doesnt exceed the console width buffer
            }
            else
            {
                if (type == LineType.Information) // Used for formating the site section of the console
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


                        var oldColour = Console.ForegroundColor; // Cache original colour so we can restore it later
                        if (Site.Status == SiteStatus.Finished) // Set finished sites a different colour to make them stand out
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
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
                        } // Ensure that line doesnt exceed the console width buffer

                        Console.ForegroundColor = oldColour;

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

        /// <summary>
        /// Sets text the content of the line
        /// </summary>
        public void SetText(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Determines the type of data to be displayed on the line
        /// </summary>
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

    /// <summary>
    /// Type of lines that can be used for display, determines how they're formatted
    /// </summary>
    enum LineType
    {
        Output,
        Information,
        Title,
        Divider,
    }
}
