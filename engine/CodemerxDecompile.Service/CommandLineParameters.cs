using System;

namespace CodemerxDecompile.Service
{
    public class CommandLineParameters
    {
        public string Port { get; private set; }

        public bool HasPort => !string.IsNullOrWhiteSpace(this.Port);

        public static CommandLineParameters Parse(string[] arguments)
        {
            CommandLineParameters parameters = new CommandLineParameters();

            foreach (string argument in arguments)
            {
                string[] parts = argument.Split('=', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    switch (parts[0])
                    {
                        case "--port":
                            parameters.Port = parts[1];
                            break;
                    }
                }
            }

            return parameters;
        }
    }
}
