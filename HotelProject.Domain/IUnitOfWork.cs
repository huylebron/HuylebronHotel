﻿namespace HotelProject.Domain ;

public interface IUnitOfWork : IDisposable
{
     Task SaveChangesAsync();
     Task BeginTransactionAsync();
     Task CommitAsync();
     Task RollbackAsync();
}