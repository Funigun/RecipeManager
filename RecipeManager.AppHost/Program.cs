IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ProjectResource> identityApi = builder.AddProject<Projects.RecipeManager_Identity_Api>("recipe-manager-identity-api");

IResourceBuilder<ProjectResource> coreApi = builder.AddProject<Projects.RecipeManager_Api>("recipe-manager-api")
                                                   .WaitFor(identityApi);

builder.AddProject<Projects.RecipeManager_UI_Blazor>("recipe-manager-ui-blazor")
       .WaitFor(identityApi)
       .WaitFor(coreApi);

await builder.Build().RunAsync();
