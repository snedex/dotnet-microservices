

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using B = MongoDB.Bson.Serialization;
using D = MongoDB.Driver;
using S = MongoDB.Bson.Serialization.Serializers;
using M = MongoDB;
using Play.Common.Settings;

namespace Play.Common.MongoDB
{
    public static class MongoExtensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            // Add services to the container.
            B.BsonSerializer.RegisterSerializer(new S.GuidSerializer(M.Bson.BsonType.String));
            B.BsonSerializer.RegisterSerializer(new S.DateTimeOffsetSerializer(M.Bson.BsonType.String));

            //Now configure how to build the IMongoDatabase instance
            services.AddSingleton(serviceProvider => {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                var mongoClient = new D.MongoClient(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });

            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) 
            where T : IEntity
        {
            //Wire up the interface to repository class
            services.AddSingleton<IRepository<T>>(provider => {
                var database = provider.GetService<D.IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName);
            });

            return services;
        }
    }
}