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

using System.IO;

using CodemerxDecompile.Service.Interfaces;

namespace CodemerxDecompile.Service.Services
{
    internal class PathService : IPathService
    {
        public PathService()
        {
            this.WorkingDirectory = Path.Join(Path.GetTempPath(), "CD");
            this.MetadataStorageDirectory = Path.Join(this.WorkingDirectory, ".decompilation-metadata");
        }

        public string WorkingDirectory { get; }

        public string MetadataStorageDirectory { get; }
    }
}