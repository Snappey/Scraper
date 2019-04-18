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

        private void BuildCommandStructure()
        {

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

        public bool Invoke(string cmd, CommandArguments args)
        {
            if (commands.ContainsKey(cmd.ToLower()))  // TODO: Iterate attribute data to check for aliases
            {
                var cmds = commands[cmd];

                cmds.Invoke(args);
            }
            return false;
        }
    }
}
