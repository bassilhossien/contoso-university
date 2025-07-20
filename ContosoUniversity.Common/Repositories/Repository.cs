using ContosoUniversity.Data.Entities;
using ContosoUniversity.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Common;
using ContosoUniversity.Data.DbContexts;

namespace ContosoUniversity.Data
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _entities;

        public Repository(DbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public IQueryable<T> Get(int id)
        {
            return _entities.Where(s => s.ID == id);
        }

        public IQueryable<T> GetAll()
        {
            return _entities;
        }

        public void Add(T entity)
        {
            _entities.Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _entities.Remove(entity);
        }

        public void Update(T entity, byte[] rowVersion)
        {
            _context.Entry(entity).Property("RowVersion").OriginalValue = rowVersion;
            _entities.Update(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> ExecuteSqlCommandAsync(string queryString)
        {
            return await _context.Database.ExecuteSqlRawAsync(queryString);
        }

        public DbConnection GetDbConnection()
        {
            return _context.Database.GetDbConnection();
        }
    }
}
