using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.UI
{
    /// <summary>
    /// Each class represents a line on the console, one unit of information that  is redrawn each time as required. This is managed by ConsoleManager.cs based on the ConsoleHeight
    /// </summary>
    class Line
    {
        private string text;
        private int row;

        public Line(int row, string text="")
        {
            this.row = row;
            this.text = text;
        }

        public void Update()
        {
            int x = Console.CursorLeft; // Cache position before update
            int y = Console.CursorTop;

            Console.SetCursorPosition(0, row); // Change to position of line

            Console.WriteLine(text);

            Console.SetCursorPosition(x,y); // Restore position back to original
        }

        public void SetText(string text)
        {
            this.text = text;
        }
    }
}
