
using System;
using System.Collections.Generic;
using System.Text;
using Crawler.UI;

namespace Crawler
{
    /// <summary>
    /// Manages the UI Section of the Console Application
    /// </summary>
    class ConsoleManager
    {
        private Line[] lines;


        public ConsoleManager(int consoleWidth, int consoleHeight)
        {
            Console.BufferWidth = consoleWidth;
            Console.BufferHeight = consoleHeight;

            Console.Clear();
            // Create an array of UI/Line.cs which represents each line of the console.
            // States based on the command ran influences what is drawn(?)

            Setup();
        }

        private void Setup()
        {
            lines = new Line[Console.BufferHeight];


        }

        private void Draw()
        {
            // Main Drawing Loop
        }

        public void Log()
        {
            // Outputs to main log in the centre screen
            // Console.WriteLine etc. should be routed here
        }
    }
}
