// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>(optional: true);
var config = builder.Configuration;

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
    .WithEnvironment("CHAINLIT_URL", config["CHAINLIT_URL"])
    .WithEnvironment("CHAINLIT_AUTH_SECRET", config["CHAINLIT_AUTH_SECRET"])
    .WithEnvironment("OAUTH_AZURE_AD_CLIENT_ID", config["EntraID_CLIENT_ID"])
    .WithEnvironment("OAUTH_AZURE_AD_CLIENT_SECRET", config["EntraID_CLIENT_SECRET"])
    .WithEnvironment("OAUTH_AZURE_AD_TENANT_ID", config["EntraID_TENANT_ID"])
    .WithEnvironment("OAUTH_AZURE_AD_ENABLE_SINGLE_TENANT", "True")
    .WaitFor(apiService);

builder.Build().Run();
