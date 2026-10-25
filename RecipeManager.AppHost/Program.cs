IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RedisResource> redisCache = builder.AddRedis("Cache")
                                                    .WithRedisInsight();

IResourceBuilder<ProjectResource> identityApi = builder.AddProject<Projects.RecipeManager_Identity_Api>("recipe-manager-identity-api");

IResourceBuilder<ProjectResource> coreApi = builder.AddProject<Projects.RecipeManager_Api>("recipe-manager-api")
                                                   .WaitFor(identityApi)
                                                   .WaitFor(redisCache)
                                                   .WithReference(redisCache);

builder.AddProject<Projects.RecipeManager_UI_Blazor>("recipe-manager-ui-blazor")
       .WaitFor(identityApi)
       .WaitFor(coreApi)
       .WithReference(identityApi)
       .WithReference(coreApi);

#pragma warning disable ASPIREBLAZOR001
IResourceBuilder<BlazorWasmAppResource> blazorUi = builder.AddBlazorWasmProject<Projects.RecipeManager_UI>("recipe-manager-ui")
                                                          .WithReference(identityApi)
                                                          .WithReference(coreApi);

builder.AddBlazorGateway("recipe-manager-ui-gateway")
       .WithExternalHttpEndpoints()
       .WithBlazorClientApp(blazorUi, proxyTelemetry: true);
#pragma warning restore ASPIREBLAZOR001

await builder.Build().RunAsync();
