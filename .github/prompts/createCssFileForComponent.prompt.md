---
agent: agent
---

Create a CSS file for a UI component specified in the prompt. If the component is not specified, ask for clarification.

The CSS file should be created in the same location as the component, name the file as [ComponentFileName].css excluding "razor" part of the file name.
Example: For a component named "MyComponent.razor", the CSS file should be named "MyComponent.css".

This will cause the file to not be a standard isolated blazor CSS file, but this is intentional as we are using 3rd party library with components and
it would require additional code refactoring to support isolated CSS files.