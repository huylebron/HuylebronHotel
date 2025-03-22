using System . Linq . Expressions ;
using HotelProject . Domain . Entities ;

namespace HotelProject.Domain ;

public interface IGenericRepository<TEntity, TKey> where TEntity : class
{
    Task<TEntity?> FindByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includeProperties);

    Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties);

    IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties);

    void Add(TEntity entity);
    void AddRange(List<TEntity> entities);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    void RemoveMultiple(List<TEntity> entities);

    IQueryable < Booking > FindAll ( Func < Booking , AppUser > includeProperties , Func < Booking , Room > includeProperty , Func < Booking , RoomType > expression , Func < Booking , Hotel > func ) ;
}