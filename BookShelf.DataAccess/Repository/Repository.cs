using BookShelf.DataAccess.Data;
using BookShelf.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookShelf.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity); // Implementation for adding an entity
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet; // Start with the DbSet
            query = query.Where(filter);
            return query.FirstOrDefault(); // Return the first entity that matches the filter or null if none found
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet;
            return query.ToList(); // Ensure the method returns the list of entities
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity); // Implementation for removing an entity
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity); // Implementation for removing multiple entities
        }
    }
}
