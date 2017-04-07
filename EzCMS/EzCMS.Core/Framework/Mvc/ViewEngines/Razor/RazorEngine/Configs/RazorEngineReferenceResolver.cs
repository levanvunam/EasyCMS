namespace EzCMS.Core.Framework.Mvc.ViewEngines.Razor.RazorEngine.Configs
{
    //public class RazorEngineReferenceResolver : IReferenceResolver
    //{
    //    public string FindLoaded(IEnumerable<string> refs, string find)
    //    {
    //        return refs.First(r => r.EndsWith(Path.DirectorySeparatorChar + find));
    //    }

    //    public IEnumerable<CompilerReference> GetReferences(TypeContext context, IEnumerable<CompilerReference> includeAssemblies)
    //    {
    //        // TypeContext gives you some context for the compilation (which templates, which namespaces and types)

    //        // You must make sure to include all libraries that are required!
    //        // Mono compiler does add more standard references than csc! 
    //        // If you want mono compatibility include ALL references here, including mscorlib!
    //        // If you include mscorlib here the compiler is called with /nostdlib.
    //        IEnumerable<string> loadedAssemblies = (new UseCurrentAssembliesReferenceResolver())
    //            .GetReferences(context, includeAssemblies)
    //            .Select(r => r.GetFile())
    //            .ToArray();

    //        yield return CompilerReference.From(FindLoaded(loadedAssemblies, "mscorlib.dll"));
    //        //yield return CompilerReference.From(FindLoaded(loadedAssemblies, "EzCMS.Bussiness.dll"));
    //        //yield return CompilerReference.From(FindLoaded(loadedAssemblies, "System.Web.dll"));
    //        //yield return CompilerReference.From(FindLoaded(loadedAssemblies, "System.Linq.dll"));
    //        //yield return CompilerReference.From(FindLoaded(loadedAssemblies, "RazorEngine.dll"));

    //        #region Load Plugins

    //        var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
    //        var assemblies = currentAssemblies.SelectMany(i => i.GetReferencedAssemblies()).
    //            Where(i => i.FullName.Contains("EzCMS")).Distinct().ToList();

    //        var pluginFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
    //        foreach (var plugin in Directory.GetDirectories(pluginFolder, "*", SearchOption.TopDirectoryOnly))
    //        {
    //            var binFolder = Path.Combine(plugin, "bin");
    //            foreach (var dllFile in Directory.GetFiles(binFolder, "EzCMS.*.dll", SearchOption.AllDirectories))
    //            {
    //                var fileName = Path.GetFileNameWithoutExtension(dllFile) ?? string.Empty;
    //                if (!assemblies.Any(a => fileName.Equals(a.Name)))
    //                {
    //                    yield return CompilerReference.From(dllFile);
    //                }
    //            }
    //        }

    //        #endregion

    //        // There are several ways to load an assembly:
    //        //yield return CompilerReference.From("Path-to-my-custom-assembly"); // file path (string)
    //        //byte[] assemblyInByteArray = --- Load your assembly ---;
    //        //yield return CompilerReference.From(assemblyInByteArray); // byte array (roslyn only)
    //        //string assemblyFile = --- Get the path to the assembly ---;
    //        //yield return CompilerReference.From(File.OpenRead(assemblyFile)); // stream (roslyn only)
    //    }
    //}
}
