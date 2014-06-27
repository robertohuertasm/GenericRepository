GenericRepository
=================

Use repository pattern with support for generics and Ioc based on https://github.com/tugberkugurlu/GenericRepository.

This implementation is simpler and can be used in Ioc scenarios.

You will notice the inclusion of a TExtra generic parameter into the interface in order to support Entity Framework scenarios. This property can serve other purposes if needed in any kind of implementation. In this case, we needed the context to be created whenever the Repository was resolved by our Ioc and to be disposed at then end of the repository lifetime.

Repository.cs responds to a Entity Framework IRepository implementation.
