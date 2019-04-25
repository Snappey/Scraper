using System;
using System.Collections.Generic;
using System.Text;
using Crawler.Attributes;
using Crawler.Structures;

namespace Crawler.Commands
{
    [CommandName("help")]
    [CommandDescription("Lists all commands")]
    class Help : Command
    {
        public override void Invoke(CommandArguments commandArgument)
        {
            var commands = Program.App.GetCommands();

            Program.App.Log("Commands:");
            foreach (var command in commands)
            {
                StringBuilder stringBuilder = new StringBuilder("> ");
                stringBuilder.Append(command.Key);

                stringBuilder.Append(" (");
                for (var index = 0; index < command.Value.Aliases.Count; index++)
                {
                    var alias = command.Value.Aliases[index];

                    if (index + 1 == command.Value.Aliases.Count)
                    {
                        stringBuilder.Append(alias);
                    }
                    else
                    {
                        stringBuilder.Append(alias + ",");
                    }
                    
                }

                stringBuilder.Append(") ");
                stringBuilder.Append("- " + command.Value.Description);

                Program.App.Log(stringBuilder.ToString());

                if (command.Value.Arguments.Count > 0)
                {
                    Program.App.Log("  -Arguments:");
                    foreach (var argument in command.Value.Arguments)
                    {
                        Program.App.Log("   > " + argument.Prefix + ", " + argument.Fullfix + ", " + argument.Description);
                    }
                }
            }
        }
    }
}
