using System.Text;
using LibMothership.Networking;

namespace LibMothership
{
    public interface ICommand
    {
        string Name { get; }
        string Syntax { get; }

        void Invoke(MothershipConnection connection, StringBuilder output, params string[] args);
    }
}
