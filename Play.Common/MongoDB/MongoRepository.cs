
using System.Linq.Expressions;
using MongoDB.Driver;

namespace Play.Common
{

    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        //MongoDB collection, almost equivalent to a table
        private readonly IMongoCollection<T> dbCollection;

        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).ToListAsync();
        }


        public async Task<T> GetAsync(Guid id)
        {
            var filter = filterBuilder.Eq<Guid>(i => i.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        
        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
              return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var filter = filterBuilder.Eq<Guid>(i => i.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(Guid id)
        {
            var filter = filterBuilder.Eq<Guid>(i => i.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }

    }
}