# Disclaimer

*This project is in an experimental stage*

# Cotidico

A *co*mpile *ti*me *di* *co*ntainer

## Introduction

Cotidico is a dependency injection (DI) container for C#. While other DI containers usually register their dependencies when starting the application, Cotidico creates factories when compiling your project.

While that may lead to a slower build process it also leads to faster application startup times since there is no registration process going on. And while finding the correct factory Method to construct an object still relies on reflection, resolving any constructor dependencies has already been done at compile time.

## When to use what
Cotidico is split into two parts. A generator as well as an external library. 

The external library contains the *IModule* interface, which is used to implement modules where dependencies are registered, as well as the *Container* class, which is used to resolve any of the registered dependency.

The generator can be executed against a solution file, it analyses all projects and searches for implementations of *IModule*. It then generates factories for the registered dependencies in said *IModule* implementations.
