//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

using System;
using System.Linq;

namespace CodemerxDecompile.Service
{
    public class CommandLineParameters
    {
        private const char Quote = '\"';

        public string Port { get; private set; }

        public bool HasPort => !string.IsNullOrWhiteSpace(this.Port);

        public string WorkingDir { get; private set; }

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
                        case "--workingDir":
                            parameters.WorkingDir = ParseStringParameterValue(parts[1]);
                            break;
                    }
                }
            }

            return parameters;
        }

        private static string ParseStringParameterValue(string value)
        {
            if (value.First() == Quote && value.Last() == Quote)
            {
                return value.Substring(1, value.Length - 2);
            }
            else if (!value.Any(c => c == Quote))
            {
                return value;
            }
            else
            {
                throw new ArgumentOutOfRangeException("String parameter values must either be surrounded in quotes or must not contain quotes at all.");
            }
        }
    }
}
