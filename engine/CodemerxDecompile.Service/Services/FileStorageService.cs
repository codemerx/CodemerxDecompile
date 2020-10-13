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
    internal class FileStorageService : IStorageService
    {
        private readonly IPathService pathService;
        private readonly ISerializationService serializationService;

        public FileStorageService(IPathService pathService, ISerializationService serializationService)
        {
            this.pathService = pathService;
            this.serializationService = serializationService;

            this.EnsureFileStorageDirectoryIsPresent();
        }

        public void Store<T>(T obj)
        {
            string filePath = this.GetFilePathForType<T>();
            string serialized = this.serializationService.Serialize(obj);
            File.WriteAllText(filePath, serialized);
        }

        public K Retrieve<T, K>()
        {
            string filePath = this.GetFilePathForType<T>();
            string serialized = File.ReadAllText(filePath);
            File.Delete(filePath);
            return this.serializationService.Deserialize<K>(serialized);
        }

        public bool HasStored<T>()
        {
            string filePath = this.GetFilePathForType<T>();
            return File.Exists(filePath);
        }

        private string GetFilePathForType<T>() => Path.Join(this.pathService.MetadataStorageDirectory, $"{typeof(T).Name}.{this.serializationService.SerializedFileExtension}");

        private void EnsureFileStorageDirectoryIsPresent()
        {
            if (!Directory.Exists(this.pathService.MetadataStorageDirectory))
            {
                Directory.CreateDirectory(this.pathService.MetadataStorageDirectory);
            }
        }
    }
}