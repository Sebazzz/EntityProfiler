using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("")]
#endif

[assembly: AssemblyCompany("Entity Profiler initiative")]
[assembly: AssemblyProduct("Entity Profiler")]
[assembly: AssemblyCopyright("Copyright © Sebastiaan Dammann and contributors 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0")]


[assembly:InternalsVisibleTo("EntityProfiler.Interceptor, PublicKey=" +
                             "00240000048000009400000006020000002400005253413100040000010001000df575755ef693"+
                             "58fb3d4427fcc7c07fb44589d52ff0e31cbc622e2ff18e8b9c81cdceb7d0ddc2cff6c1505fc4c3"+
                             "038ebe7654313be0ccf8165b1ef896d5b124dfa36bebdbf379c2babeea10bc9cf75ef8f10b8e03"+
                             "f67f034c023c61eab7cd068108ad078177b9553c70be7f9045eb79d672a73000540252b0741bcd"+
                             "f422f5cf")]

[assembly:InternalsVisibleTo("EntityProfiler.Interceptor.Reader, PublicKey=" +
                             "00240000048000009400000006020000002400005253413100040000010001000df575755ef693"+
                             "58fb3d4427fcc7c07fb44589d52ff0e31cbc622e2ff18e8b9c81cdceb7d0ddc2cff6c1505fc4c3"+
                             "038ebe7654313be0ccf8165b1ef896d5b124dfa36bebdbf379c2babeea10bc9cf75ef8f10b8e03"+
                             "f67f034c023c61eab7cd068108ad078177b9553c70be7f9045eb79d672a73000540252b0741bcd"+
                             "f422f5cf")]

[assembly:InternalsVisibleTo("EntityProfiler.UI, PublicKey=" +
                             "00240000048000009400000006020000002400005253413100040000010001000df575755ef693"+
                             "58fb3d4427fcc7c07fb44589d52ff0e31cbc622e2ff18e8b9c81cdceb7d0ddc2cff6c1505fc4c3"+
                             "038ebe7654313be0ccf8165b1ef896d5b124dfa36bebdbf379c2babeea10bc9cf75ef8f10b8e03"+
                             "f67f034c023c61eab7cd068108ad078177b9553c70be7f9045eb79d672a73000540252b0741bcd"+
                             "f422f5cf")]

[assembly:InternalsVisibleTo("EntityProfiler.Tests.Integration, PublicKey=" +
                             "00240000048000009400000006020000002400005253413100040000010001000df575755ef693"+
                             "58fb3d4427fcc7c07fb44589d52ff0e31cbc622e2ff18e8b9c81cdceb7d0ddc2cff6c1505fc4c3"+
                             "038ebe7654313be0ccf8165b1ef896d5b124dfa36bebdbf379c2babeea10bc9cf75ef8f10b8e03"+
                             "f67f034c023c61eab7cd068108ad078177b9553c70be7f9045eb79d672a73000540252b0741bcd"+
                             "f422f5cf")]

[assembly:InternalsVisibleTo("EntityProfiler.Tests.Unit, PublicKey=" +
                             "00240000048000009400000006020000002400005253413100040000010001000df575755ef693"+
                             "58fb3d4427fcc7c07fb44589d52ff0e31cbc622e2ff18e8b9c81cdceb7d0ddc2cff6c1505fc4c3"+
                             "038ebe7654313be0ccf8165b1ef896d5b124dfa36bebdbf379c2babeea10bc9cf75ef8f10b8e03"+
                             "f67f034c023c61eab7cd068108ad078177b9553c70be7f9045eb79d672a73000540252b0741bcd"+
                             "f422f5cf")]