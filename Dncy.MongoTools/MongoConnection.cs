using Dncy.MongoTools.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Dncy.MongoTools
{
    public class MongoConnection
    {
        private readonly string connectionString;
        private readonly IMongoClient mongoClient;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MongoConnection(string connectionString)
        {
            connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            EnsureInitialized();
        }

        public IMongoClient MongoClient => this.mongoClient;


        public IMongoDatabase? GetDatabase(string dataBaseName)
        {
            return MongoClient.GetDatabase(dataBaseName);
        }


        public IMongoCollection<T>? GetCollection<T>(string dataBaseName, string collectionName)
        {
            return MongoClient.GetDatabase(dataBaseName).GetCollection<T>(collectionName);
        }


        public IAggregateFluent<T> GetBucket<T>(string dataBaseName, string collectionName) where T : BucketBaseModel
        {
            var aggregate = GetDatabase(dataBaseName)?.GetCollection<T>(collectionName).Aggregate(new PipleLine<T>());
            return (IAggregateFluent<T>)aggregate;
        }


        private IMongoClient EnsureInitialized()
        {
            return new MongoClient(connectionString);
        }

    }


    public class PipleLine<T> : PipelineDefinition<T, BucketBaseModel<T>>
    {
        /// <inheritdoc />
        public override RenderedPipelineDefinition<BucketBaseModel<T>> Render(IBsonSerializer<T> inputSerializer, IBsonSerializerRegistry serializerRegistry,
            LinqProvider linqProvider)
        {
            return null;
        }

        /// <inheritdoc />
        public override IBsonSerializer<BucketBaseModel<T>> OutputSerializer => BsonSerializer.SerializerRegistry.GetSerializer<BucketBaseModel<T>>();

        /// <inheritdoc />
        public override IEnumerable<IPipelineStageDefinition> Stages { get; }
    }
}