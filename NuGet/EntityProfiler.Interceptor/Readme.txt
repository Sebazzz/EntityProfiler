# EntityProfiler

An initiative for creating an open-source Entity Framework profiler. 

![EntityProfiler](EntityProfiler.png)

## Features
- View syntax highlighted queries and their source, including the stack trace.
- Context-aware: Knows about HttpContext when using ASP.NET.
- Duplicate or SELECT 1+N query detection: shows the number of duplicated queries.
- Stack Trace reduction: removes confusing and unneccesary frames from Entity Framework from the top of the call stack.
- Works with ASP.NET and desktop apps.

## How to use it
Install the EntityProfiler.Interceptor NuGet package or install manually:

1. Add references to EntityProfiler.Common and EntityProfiler.Interceptor to your project.
2. Register the interceptor in either the `DbConfiguration` or in your web.config/app.config file:

        <entityFramework>
           ...
           
           <interceptors>
             <interceptor type="EntityProfiler.Interceptor.Core.ProfilingInterceptor, EntityProfiler.Interceptor"/>
           </interceptors>
        </entityFramework>
        
3. Run the EntityProfiler.UI application which will automatically connect.


## Current limitations
Only one profiling application instance is allowed. Currently the interceptor opens a TCP socket on a fixed port on localhost and no configuration is yet possible.

## Contributions
This project accepts contributions. Please put an issue in the issue track first to prevent confusion.
https://github.com/Sebazzz/EntityProfiler