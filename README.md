GenericRepository
=================

Use repository pattern with support for generics.

The interface is simple and can be separated from the current implementation. You will notice the inclusion of a TExtra generic parameter into the interface. This was done to make it easier to work with Entity Framework, using this parameter as the context. You can use this parameter to whatever may suit your needs.

Repository.cs responds to a Entity Framework IRepository implementation.
