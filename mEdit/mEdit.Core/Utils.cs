using System.IO;
using System.Reflection;

namespace mEdit.Core
{
    public class Utils
    {
        public static string GetResourceFile(string file)
        {
            var assembly = typeof(Utils).GetTypeInfo().Assembly;

            using (var stream = assembly.GetManifestResourceStream("mEdit.Core._res." + file))
            using (var textReader = new StreamReader(stream))
            {
                return textReader.ReadToEnd();
            }
        }

        public static Stream GetResourceFileStream(string file)
        {
            var assembly = typeof(Utils).GetTypeInfo().Assembly;

            return assembly.GetManifestResourceStream("mEdit.Core._res." + file);
        }
    }
}