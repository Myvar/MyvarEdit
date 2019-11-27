using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace mEdit.Core
{
    static class Utils
    {
        public static Expected GetAttributeValue<T, Expected>(this Enum enumeration, Func<T, Expected> expression)
            where T : Attribute
        {
            T attribute =
                enumeration
                    .GetType()
                    .GetMember(enumeration.ToString())
                    .Where(member => member.MemberType == MemberTypes.Field)
                    .First()
                    .GetCustomAttributes(typeof(T), false)
                    .Cast<T>()
                    .SingleOrDefault();

            if (attribute == null)
                return default(Expected);

            return expression(attribute);
        }

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