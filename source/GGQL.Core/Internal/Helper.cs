using System.Reflection;
using System.Linq;

namespace GGQL.Core.Internal
{
    public static class Helper
    {
        public static string GetProductVersion(Assembly assembly)
        {
            Guard.ArgumentNotNull(assembly, nameof(assembly));
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute));
            AssemblyInformationalVersionAttribute attr = attributes.SingleOrDefault() as AssemblyInformationalVersionAttribute;
            if (attr == null)
            {
                return "0.0.0.0-undefined";
            }
            return attr.InformationalVersion;
        }


        public static string GetAssemblyTitle(Assembly assembly)
        {
            Guard.ArgumentNotNull(assembly, nameof(assembly));
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute));
            AssemblyTitleAttribute attr = attributes.SingleOrDefault() as AssemblyTitleAttribute;
            if (attr == null)
            {
                return string.Empty;
            }
            return attr.Title;
        }

        public static string GetAssemblyDescription(Assembly assembly)
        {
            Guard.ArgumentNotNull(assembly, nameof(assembly));
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute));
            AssemblyDescriptionAttribute attr = attributes.SingleOrDefault() as AssemblyDescriptionAttribute;
            if (attr == null)
            {
                return string.Empty;
            }
            return attr.Description;
        }




        public static string GetProductVersionFromEntryAssembly()
        {
            Assembly ass = Assembly.GetEntryAssembly();
            return GetProductVersion(ass);
        }





    }
}
