using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace PSParquetTools
{
    // Windows PowerShell 5.1 has no binding redirects, so dependencies that were
    // built against a different assembly version are resolved from the module folder.
    public class ModuleInitializer : IModuleAssemblyInitializer
    {
        public void OnImport()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolve;
        }

        private static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name).Name;
            var dir = Path.GetDirectoryName(typeof(ModuleInitializer).Assembly.Location);
            var candidate = Path.Combine(dir, name + ".dll");
            return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null;
        }
    }
}
