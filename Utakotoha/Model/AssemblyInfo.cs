using System;
using System.Linq;
using System.Reflection;

namespace Utakotoha.Model
{
    public class AssemblyInfo
    {
        public static readonly AssemblyInfo ExecutingAssembly = new AssemblyInfo(Assembly.GetExecutingAssembly());

        public string FileName { get; private set; }
        public string Version { get; private set; }
        public string FileVersion { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Configuration { get; private set; }
        public string Company { get; private set; }
        public string Product { get; private set; }
        public string Copyright { get; private set; }
        public string Trademark { get; private set; }
        public string Culture { get; private set; }

        public AssemblyInfo(Assembly assembly)
        {
            // Windows Phone 7 can't get GetName()
            // FileName = assembly.GetName().Name;
            // Version = assembly.GetName().Version.ToString();
            var fullnames = assembly.FullName.Split(',');
            FileName = fullnames[0].Trim();
            Version = fullnames[1].Replace("Version=", "").Trim();

            FileVersion = GetAttributeName<AssemblyFileVersionAttribute>(assembly, a => a.Version);
            Title = GetAttributeName<AssemblyTitleAttribute>(assembly, a => a.Title);
            Description = GetAttributeName<AssemblyDescriptionAttribute>(assembly, a => a.Description);
            Configuration = GetAttributeName<AssemblyConfigurationAttribute>(assembly, a => a.Configuration);
            Company = GetAttributeName<AssemblyCompanyAttribute>(assembly, a => a.Company);
            Product = GetAttributeName<AssemblyProductAttribute>(assembly, a => a.Product);
            Copyright = GetAttributeName<AssemblyCopyrightAttribute>(assembly, a => a.Copyright);
            Trademark = GetAttributeName<AssemblyTrademarkAttribute>(assembly, a => a.Trademark);
            Culture = GetAttributeName<AssemblyCultureAttribute>(assembly, a => a.Culture);
        }

        private string GetAttributeName<T>(Assembly assembly, Func<T, string> selector) where T : Attribute
        {
            var attr = assembly.GetCustomAttributes(typeof(T), true).Cast<T>().FirstOrDefault();
            return (attr == null) ? "" : selector(attr);
        }
    }

    public class AssemblyInfoBindingHelper
    {
        public static AssemblyInfo Value { get { return AssemblyInfo.ExecutingAssembly; } }
    }

}