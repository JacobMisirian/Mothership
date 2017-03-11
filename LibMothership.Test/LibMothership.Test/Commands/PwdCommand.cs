using System.IO;
using System.Text;

using LibMothership.Networking;

namespace LibMothership.Test.Commands
{
    public class PwdCommand : ICommand
    {
        public string Name { get { return "pwd"; } }
        public string Syntax { get { return "pwd"; } }

        public void Invoke(MothershipConnection connection, StringBuilder output, params string[] args)
        {
            ArgumentLengthException.ValidateArgumentLength(Name, args, 0);

            output.Append(Directory.GetCurrentDirectory());
        }
    }
}
