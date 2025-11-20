### Build:

----

#### `dotnet build`

```terminaloutput
Restore complete (0,2s)
  Product Management API succeeded (0,3s) → Product Management API/bin/Debug/net9.0/Product Management API.dll

Build succeeded in 0,7s
```

---

#### `dotnet test`

```terminaloutput

Test summary: total: 10, failed: 0, succeeded: 10, skipped: 0, duration: 0,7s
Build succeeded with 2 warning(s) in 1,4s
mariotescovschi@MacBook-Pro Product Management API.Tests % dotnet remove package AutoMapper
dotnet add package AutoMapper --version 12.0.1

info : Removing PackageReference for package 'AutoMapper' from project '/Users/mariotescovschi/Facultate/III/SemI/DotNet/Product Management API/Product Management API.Tests/Product Management API.Tests.csproj'.

Build succeeded in 0,3s
info : X.509 certificate chain validation will use the fallback certificate bundle at '/usr/local/share/dotnet/sdk/9.0.305/trustedroots/codesignctl.pem'.
info : X.509 certificate chain validation will use the fallback certificate bundle at '/usr/local/share/dotnet/sdk/9.0.305/trustedroots/timestampctl.pem'.
info : Adding PackageReference for package 'AutoMapper' into project '/Users/mariotescovschi/Facultate/III/SemI/DotNet/Product Management API/Product Management API.Tests/Product Management API.Tests.csproj'.
info : Restoring packages for /Users/mariotescovschi/Facultate/III/SemI/DotNet/Product Management API/Product Management API.Tests/Product Management API.Tests.csproj...
info :   CACHE https://api.nuget.org/v3/vulnerabilities/index.json
info :   CACHE https://api.nuget.org/v3-vulnerabilities/2025.11.19.23.31.42/vulnerability.base.json
info :   CACHE https://api.nuget.org/v3-vulnerabilities/2025.11.19.23.31.42/2025.11.19.23.31.42/vulnerability.update.json
info : Package 'AutoMapper' is compatible with all the specified frameworks in project '/Users/mariotescovschi/Facultate/III/SemI/DotNet/Product Management API/Product Management API.Tests/Product Management API.Tests.csproj'.
info : PackageReference for package 'AutoMapper' version '12.0.1' added to file '/Users/mariotescovschi/Facultate/III/SemI/DotNet/Product Management API/Product Management API.Tests/Product Management API.Tests.csproj'.
info : Writing assets file to disk. Path: /Users/mariotescovschi/Facultate/III/SemI/DotNet/Product Management API/Product Management API.Tests/obj/project.assets.json
log  : Restored /Users/mariotescovschi/Facultate/III/SemI/DotNet/Product Management API/Product Management API.Tests/Product Management API.Tests.csproj (in 92 ms).
mariotescovschi@MacBook-Pro Product Management API.Tests % dotnet test                     
Restore complete (0,2s)
  Product Management API succeeded (0,1s) → /Users/mariotescovschi/Facultate/III/SemI/DotNet/Product Management API/Product Management API/bin/Debug/net9.0/Product Management API.dll
  Product Management API.Tests succeeded (0,2s) → bin/Debug/net9.0/Product Management API.Tests.dll
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.4.1+b9eacec401 (64-bit .NET 9.0.9)
[xUnit.net 00:00:00.02]   Discovering: Product Management API.Tests
[xUnit.net 00:00:00.04]   Discovered:  Product Management API.Tests
[xUnit.net 00:00:00.04]   Starting:    Product Management API.Tests
[xUnit.net 00:00:00.33]   Finished:    Product Management API.Tests
  Product Management API.Tests test succeeded (0,7s)

Test summary: total: 10, failed: 0, succeeded: 10, skipped: 0, duration: 0,6s
Build succeeded in 1,3s
```

### Tested:

#### `dotnet run`

```terminaloutput 
Using launch settings from /Users/mariotescovschi/Facultate/III/SemI/DotNet/Product Management API/Product Management API/Properties/launchSettings.json...
Building...
warn: Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServer[8]
      The ASP.NET Core developer certificate is not trusted. For information about trusting the ASP.NET Core developer certificate, see https://aka.ms/aspnet/https-trust-dev-cert
warn: Microsoft.AspNetCore.Server.Kestrel[0]
      Overriding address(es) 'http://localhost:5069'. Binding to endpoints defined via IConfiguration and/or UseKestrel() instead.
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/mariotescovschi/Facultate/III/SemI/DotNet/Product Management API/Product Management API
```

---

#### request:

```terminaloutput
curl -k -X POST https://localhost:7001/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "iPhone Duplicate",
    "brand": "Apple Inc",
    "sku": "IPHONE-15-PRO-001",
    "category": 0,
    "price": 1099.99,
    "releaseDate": "2024-09-22T00:00:00Z",
    "imageUrl": "https://example.com/iphone15pro.jpg",
    "stockQuantity": 10
  }'
```

#### response:

```json
{
  "id": "1a313bb0-9730-488c-8419-aaf2b1fe9be4",
  "name": "iPhone Duplicate",
  "brand": "Apple Inc",
  "sku": "IPHONE-15-PRO-001",
  "price": 1099.99,
  "releaseDate": "2024-09-22T00:00:00Z",
  "createdAt": "2025-11-20T17:26:58.33494Z",
  "imageUrl": "https://example.com/iphone15pro.jpg",
  "isAvailable": false,
  "stockQuantity": 10,
  "categoryDisplayName": "",
  "formattedPrice": "",
  "productAge": "",
  "brandInitials": "",
  "availabilityStatus": ""
}
```

#### response when sending the same item:

```terminaloutput
Product_Management_API.Exceptions.ValidationException: A product with SKU 'IPHONE-15-PRO-001' already exists. SKU must be unique.
at Product_Management_API.Handlers.CreateProductHandler.Handle(CreateProductCommand request, CancellationToken cancellationToken) in /Users/mariotescovschi/Facultate/III/SemI/DotNet/Product Management API/Product Management API/Handlers/CreateProductHandler.cs: line 72
at MediatR.Pipeline.RequestExceptionProcessorBehavior`2.Handle(TRequest request, RequestHandlerDelegate`1 next, CancellationToken cancellationToken)
at MediatR.Pipeline.RequestExceptionProcessorBehavior`2.Handle(TRequest request, RequestHandlerDelegate`1 next, CancellationToken cancellationToken)
at MediatR.Pipeline.RequestExceptionActionProcessorBehavior`2.Handle(TRequest request, RequestHandlerDelegate`1 next, CancellationToken cancellationToken)
at MediatR.Pipeline.RequestExceptionActionProcessorBehavior`2.Handle(TRequest request, RequestHandlerDelegate`1 next, CancellationToken cancellationToken)
at MediatR.Pipeline.RequestPostProcessorBehavior`2.Handle(TRequest request, RequestHandlerDelegate`1 next, CancellationToken cancellationToken)
at MediatR.Pipeline.RequestPreProcessorBehavior`2.Handle(TRequest request, RequestHandlerDelegate`1 next, CancellationToken cancellationToken)
at Program.<>c.<<<Main>$>b__0_3>d.MoveNext() in /Users/mariotescovschi/Facultate/III/SemI/DotNet/Product Management API/Product Management API/Program.cs: line 74
--- End of stack trace from previous location ---
at Microsoft.AspNetCore.Http.RequestDelegateFactory.<ExecuteTaskOfT>g__ExecuteAwaited|132_0[T](Task`1 task, HttpContext httpContext, JsonTypeInfo`1 jsonTypeInfo)
at Microsoft.AspNetCore.Http.RequestDelegateFactory.<>c__DisplayClass101_2.<<HandleRequestBodyAndCompileRequestDelegateForJson>b__2>d.MoveNext()
--- End of stack trace from previous location ---
at Product_Management_API.Middleware.CorrelationMiddleware.InvokeAsync(HttpContext context) in /Users/mariotescovschi/Facultate/III/SemI/DotNet/Product Management API/Product Management API/Common/Middleware/CorrelationMiddleware.cs: line 35
at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)
```