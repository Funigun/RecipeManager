---
agent: agent
---
Refactor this endpoint to use 'IUnitOfWork' instead of 'IAppDbContext' directly.

IUnitOfWork is defined in RecipeManager.Api project under 'Application/Database' folder and implemented in 'Persistence/Database' folder.

Use only repository that was mentioned within given prompt, otherwise ask for clarification.

Specified repository should be available as property of 'IUnitOfWork' and should exist under 'Application/Database/Repositories' folder.
Implementation of the repository should exist under 'Persistane/Repositories' folder and it should derive from 'BaseRepository' abastract class for common methods.

Do not try to refactor other parts of the code even if they also use 'IAppDbContext'.

Try to fit code that requires change with existing repository methods at first.
Some methods might not be implemented so try to match them with signatures and implement them in repository if necessary.
Verify if matched methods are implemented in concrete or base repository, implement them in case when NotImplemented exception is provided.

If there is no suitable repository method, prepare an implementation and show me the plan so I can decide if it should go to concrete repository or 'BaseRepository'.