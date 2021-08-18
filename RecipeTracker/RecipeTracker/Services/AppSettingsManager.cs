using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace RecipeTracker.Services
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

        private const string Namespace = "RecipeTracker";
        private const string Filename = "AppSettings.json";

        private JObject secrets;

        private AppSettingsManager()
        {
            try
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(AppSettingsManager)).Assembly;
                var stream = assembly.GetManifestResourceStream($"{ Namespace }.{ Filename }");

                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    secrets = JObject.Parse(json);
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
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
