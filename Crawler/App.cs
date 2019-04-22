using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Structures;
using Scraper;
using Scraper.Structures;

namespace Crawler
{
    class App
    {
        private bool IsRunning = true;

        private CommandManager Commands;
        private SiteManager Sites;
        private Storage Storage;

        public App()
        {
            Commands = new CommandManager();
            Sites = new SiteManager();
            Storage = new Storage("local.db");

            // TODO: Register site structure for scraping
            Sites.Register();

            Sites.GetAllData();
            // Sites.GetData();
            // TODO: Start scraper
            // TODO: Register progress on pages based on 

            Loop();
        }

        private void CommandInput(string text)
        {
            var splitText = text.Split(' ');
            string command = splitText[0];
            CommandArguments arguments = new CommandArguments();

            if (splitText.Length > 1)
            {
                string[] flags = new string[splitText.Length];
                string[] parameters = new string[splitText.Length];

                var paramcount = 0;
                for (int i = 1; i < splitText.Length; i++)
                {
                    var arg = splitText[i];
                    if (arg.Substring(0, 1) == "-")
                    {
                        // arg is a flag
                        flags[paramcount] = arg;
                        paramcount++;
                    }
                    else
                    {
                        // arg is an parameter
                        if (paramcount - 1 > 0)
                        {
                            parameters[paramcount - 1] = arg;
                        }
                    }


                }

                for (int i = 0; i < flags.Length; i++)
                {
                    if (flags[i] == null) { continue; }
                    if (parameters.Length > i && parameters[i] != null)
                    {
                        arguments.Add(flags[i], parameters[i]);
                    }
                    else
                    {
                        arguments.Add(flags[i], " ");
                    }
                }
            }


            Commands.Invoke(command, arguments);
        }

        private void Loop()
        {
            while (IsRunning)
            {
                var text = Console.ReadLine();
                if (text == String.Empty) { continue; }

                CommandInput(text);

            }
        }
    }
}
