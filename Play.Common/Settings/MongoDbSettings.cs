namespace Play.Common
{
    //Represents the MongoDbSettings in appsettings.json
    public class MongoDbSettings
    {
        //Init prevents modification after initialisation, til.
        public string Host { get; init; }

        public int Port { get; init; }   

        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}