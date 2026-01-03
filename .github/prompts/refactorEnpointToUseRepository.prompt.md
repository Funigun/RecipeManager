---
agent: agent
---
Refactor this endpoint to use repository instead of 'IAppDbContext' directly.
Use only repository that was mentioned within given prompt, otherwise ask for clarification.

Do not try to refactor other parts of the code even if they also use 'IAppDbContext'.

Try to fit code that requires change with existing repository methods at first.
If there are no existing methods that fit the need, add new methods to the repository.