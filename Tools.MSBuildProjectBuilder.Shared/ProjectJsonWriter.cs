using System;
using System.Collections.Generic;
using System.IO;
#if NETSTANDARD
using System.Text.Json;
#else
using System.Web.Script.Serialization;
#endif
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.External.Interfaces;

namespace JustDecompile.Tools.MSBuildProjectBuilder
{
    internal class ProjectJsonWriter : ExceptionThrownNotifier, IExceptionThrownNotifier
    {
        public static readonly string ProjectJsonFileName = "project.json";

        private string outputPath;
        private Dictionary<string, string> dependencies;
        private string framework;
        private List<string> runtimes;

        public ProjectJsonWriter(string outputPath, Dictionary<string, string> dependencies, string framework, List<string> runtimes)
        {
            this.outputPath = outputPath;
            this.dependencies = dependencies;
            this.framework = framework;
            this.runtimes = runtimes;
        }

        public string ProjectJsonFilePath
        {
            get
            {
                return Path.Combine(this.outputPath, ProjectJsonFileName);
            }
        }

        public bool WriteProjectJsonFile()
        {
            ProjectJson projectJson = new ProjectJson(this.dependencies, this.framework, this.runtimes);
            string jsonContent;

            // AGPL License
#if NETSTANDARD
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            jsonContent = JsonSerializer.Serialize(projectJson, typeof(ProjectJson), options);
#else
            jsonContent = new JavaScriptSerializer().Serialize(projectJson);
#endif

            try
            {
                if (File.Exists(ProjectJsonFilePath))
                {
                    File.Delete(ProjectJsonFilePath);
                }

                File.WriteAllText(ProjectJsonFilePath, jsonContent);
            }
            catch (Exception ex)
            {
                OnExceptionThrown(ex);

                return false;
            }

            return true;
        }

        private class ProjectJson
        {
            public ProjectJson(Dictionary<string, string> dependencies, string framework, List<string> runtimes)
            {
                this.dependencies = dependencies;

                this.frameworks = new Dictionary<string, object>();
                this.frameworks.Add(framework, new object());

                this.runtimes = new Dictionary<string, object>();
                foreach (string runtime in runtimes)
                {
                    this.runtimes.Add(runtime, new object());
                }
            }

            public Dictionary<string, string> dependencies { get; private set; }

            // The project.json file for UWP projects support only 1 target framework. This property is of
            // type Dictionary<string, object>, because by serializing Dictionary we can get the proper output
            // structure of the json file. This Dictionary should never contain 2 or more KeyValuePairs.
            public Dictionary<string, object> frameworks { get; private set; }

            public Dictionary<string, object> runtimes { get; private set; }
        }
    }
}
