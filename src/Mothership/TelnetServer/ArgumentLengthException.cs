using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mothership.TelnetServer
{
    public class ArgumentLengthException : Exception
    {
        public string Target { get; private set; }
        public int InvokedWith { get; private set; }
        public int Expected { get; private set; }

        public ArgumentLengthException(string target, int invokedWith, int expected)
        {
            Target = target;
            InvokedWith = invokedWith;
            Expected = expected;
        }

        public static void ValidateArgumentLength(string target, string[] args, params int[] lengths)
        {
            if (lengths.Length == 0)
                return;
            if (lengths[0] == -1)
                return;

            foreach (int length in lengths)
                if (args.Length == length)
                    return;

            throw new ArgumentLengthException(target, args.Length, lengths[0]);
        }
    }
}
