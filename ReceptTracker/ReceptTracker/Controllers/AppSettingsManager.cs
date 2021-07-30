using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;

namespace ReceptTracker.Controllers
{
    class AppSettingsManager
    {
        private static AppSettingsManager Instance;
        public static AppSettingsManager Settings
        {
            get
            {
                if (Instance == null) Instance = new AppSettingsManager();

                return Instance;
            }
        }

        private const string Namespace = "ReceptTracker";
        private const string Filename = "AppSettings.json";

        private JObject secrets;

        private AppSettingsManager()
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(AppSettingsManager)).Assembly;
            var stream = assembly.GetManifestResourceStream($"{ Namespace }.{ Filename }");

            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                secrets = JObject.Parse(json);
            }
        }

        public string this[string name]
        {
            get
            {
                try
                {
                    var path = name.Split(':');

                    JToken node = secrets[path[0]];
                    for (int i = 1; i < path.Length; i++) node = node[path[i]];

                    return node.ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return string.Empty;
                }
            }
        }
    }
}
