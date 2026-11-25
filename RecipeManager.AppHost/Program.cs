IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RedisResource> redisCache = builder.AddRedis("Cache")
                                                    .WithRedisInsight();

IResourceBuilder<Aspire.Hosting.Dotnet.DotnetProjectResource>? identityApi = builder.AddDotnetProject(
    "recipe-manager-identity-api",
    "..\\RecipeManager.Identity.API\\RecipeManager.Identity.API.csproj");

IResourceBuilder<Aspire.Hosting.Dotnet.DotnetProjectResource>? coreApi = builder.AddDotnetProject(
                  "recipe-manager-api",
                  "..\\RecipeManager.API\\RecipeManager.Api.csproj")
              .WaitFor(identityApi)
              .WaitFor(redisCache)
              .WithReference(redisCache);

#pragma warning disable ASPIREBLAZOR001
IResourceBuilder<BlazorWasmAppResource> blazorUi = builder.AddBlazorWasmApp(
                                                        "recipe-manager-ui",
                                                        "..\\RecipeManager.UI\\RecipeManager.UI.csproj")
                                                        .WithReference(identityApi)
                                                        .WithReference(coreApi);

builder.AddDotnetProjectBlazorGateway("recipe-manager-ui-gateway")
       .WithExternalHttpEndpoints()
       .WithBlazorClientApp(blazorUi);
#pragma warning restore ASPIREBLAZOR001

await builder.Build().RunAsync();
