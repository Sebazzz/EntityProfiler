using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("EntityProfiler.Viewer")]
[assembly: AssemblyDescription("")]
//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]


[assembly: XmlnsDefinition("http://schemas.paulozzico.com/entityprofiler", "EntityProfiler.Viewer")]
[assembly: XmlnsDefinition("http://schemas.paulozzico.com/entityprofiler", "EntityProfiler.Viewer.PresentationCore")]
[assembly: XmlnsDefinition("http://schemas.paulozzico.com/entityprofiler", "EntityProfiler.Viewer.Modules.Connection")]