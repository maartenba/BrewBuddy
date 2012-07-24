using System.Linq;

namespace BrewBuddy.Entities
{
    public class EntityRepository<T> : IEntityRepository<T>
        where T : class, IEntity, new()
    {
        readonly EntitiesContext _entities;

        public EntityRepository(EntitiesContext entities)
        {
            this._entities = entities;
        }

        public void CommitChanges()
        {
            _entities.SaveChanges();
        }

        public void DeleteOnCommit(T entity)
        {
            _entities.Set<T>()
                .Remove(entity);
        }

        public T Get(int key)
        {
            return _entities.Set<T>().Find(key);
        }

        public IQueryable<T> GetAll()
        {
            return _entities.Set<T>();
        }

        public int InsertOnCommit(T entity)
        {
            _entities.Set<T>()
                .Add(entity);

            return entity.Id;
        }
    }
}