---
agent: agent
---
Implement a repository for domain entity. The entity should be specified in the prompt, otherwise ask for clarification.

Repository interface should be created in the 'Application/[DomainEntity]' folder, where [DomainEntity] is the name of the entity in plural.
It must implement 'IRepository<TEntity>' interface from 'RecipeManager.Api.Shared' project

Repository implementation should be created in the 'Persistance/Repositories/[DomainEntity]' folder, where [DomainEntity] is the name of the entity in plural.