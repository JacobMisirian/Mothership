using System.IO;
using System.Text;

using LibMothership.Networking;

namespace LibMothership.Test.Commands
{
    public class CdCommand : ICommand
    {
        public string Name { get { return "cd"; } }
        public string Syntax { get { return "cd [DIRECTORY]"; } }

        public void Invoke(MothershipConnection connection, StringBuilder output, params string[] args)
        {
            ArgumentLengthException.ValidateArgumentLength(Name, args, 1);

            if (!Directory.Exists(args[0]))
            {
                output.AppendFormat("No such directory {0}!", args[0]);
                return;
            }

            Directory.SetCurrentDirectory(args[0]);
            output.Append("Changed directory!");
        }
    }
}
