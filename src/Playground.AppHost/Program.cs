// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.Playground_ApiService>("apiservice");

builder.AddProject<Projects.Playground_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddContainer("chainlit-ui", "playground-chainlit-app:latest")
    .WithEndpoint(port: 8000, targetPort: 8000, scheme: "http")
    .WaitFor(apiService);

builder.Build().Run();
