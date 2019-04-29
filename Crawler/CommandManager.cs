using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Crawler.Structures;
using Crawler.Commands;

namespace Crawler
{
    class CommandManager
    {
        private Dictionary<string, Command> commands = new Dictionary<string, Command>();

        public CommandManager()
        {
            LoadCommands();
        }

        public Dictionary<string, Command> GetCommands()
        {
            return commands;
        }

        private void LoadCommands()
        {
            // Loop through each of the commands via an attribution
            Type[] types = Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                String.Equals(t.Namespace, "Crawler.Commands", StringComparison.Ordinal)).ToArray();

            foreach (Type type in types)
            {
                var clss = (Command)Activator.CreateInstance(type);
                var list = type.GetCustomAttributesData();

                for (int i = 0; i < list.Count; i++)
                {
                    var atrb = list[i];

                    if (atrb.AttributeType.Name == "CommandName")
                    {
                        clss.Name = atrb.ConstructorArguments[0].Value.ToString();
                    }
                    if (atrb.AttributeType.Name == "CommandAlias")
                    {
                        for (int k = 0; k < atrb.ConstructorArguments.Count; k++)
                        {
                            clss.Aliases.Add(atrb.ConstructorArguments[k].Value.ToString());
                        }
                    }
                    if (atrb.AttributeType.Name == "CommandDescription")
                    {
                        clss.Description = atrb.ConstructorArguments[0].Value.ToString();
                    }           
                    if (atrb.AttributeType.Name == "CommandArgument")
                    {

                        CommandArgument argument = new CommandArgument
                        {
                            Prefix = atrb.ConstructorArguments[0].Value.ToString(),
                            Fullfix = atrb.ConstructorArguments[1].Value.ToString(),
                            Description = atrb.ConstructorArguments[2].Value.ToString()
                        };

                        clss.Arguments.Add(argument);

                    }
                }

                commands.Add(clss.Name, clss);
            }
        }

        public void CommandInput(string text)
        {
            if (text == null) { return; }
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
                    if (arg == string.Empty) { continue; }
                    if (arg.Substring(0, 1) == "-")
                    {
                        // arg is a flag
                        flags[paramcount] = arg;
                        paramcount++;
                    }
                    else
                    {
                        // arg is an parameter
                        if (paramcount - 1 >= 0)
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
            Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] {text}"); // Echo back into console

            if (Invoke(command, arguments) == false)
            {
                Program.App.Log($"[{DateTime.Now.ToShortTimeString()}] No command found '" + command + "'");
            }
        }

        public bool Invoke(string cmd, CommandArguments args)
        {
            if (commands.ContainsKey(cmd.ToLower()))
            {
                var cmds = commands[cmd];

                cmds.Invoke(args);

                return true;
            }
            return false;
        }
    }
}
