using API_CAPITAL_MANAGEMENT.Hubs.Entities;
using API_CAPITAL_MANAGEMENT.Hubs.Services.IServices;
using MongoDB.Driver;

namespace API_CAPITAL_MANAGEMENT.Hubs.Services
{
    /// <summary>
    /// Provides functionality for managing and retrieving messages stored in a MongoDB database.
    /// </summary>
    /// <remarks>This service is designed to handle operations related to messages, such as saving messages to
    /// the database and retrieving messages associated with a specific room. It relies on a MongoDB collection for data
    /// storage and retrieval. Ensure that the MongoDB connection string and database name are properly configured in
    /// the application's configuration file under the "MongoDB" section.</remarks>
    public class MessageService : IMessageService
    {
        private readonly IMongoCollection<Message> _collection;
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageService"/> class, configuring the connection to the
        /// MongoDB database.
        /// </summary>
        /// <remarks>This constructor establishes a connection to the MongoDB database using the provided
        /// configuration. The "MongoDB:ConnectionString" key specifies the connection string for the MongoDB server, 
        /// and the "MongoDB:Database" key specifies the name of the database to use.</remarks>
        /// <param name="configuration">The application configuration containing the MongoDB connection settings.  The configuration must include
        /// the "MongoDB:ConnectionString" and "MongoDB:Database" keys.</param>
        public MessageService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoDB:ConnectionString"]);
            var database = client.GetDatabase(configuration["MongoDB:Database"]);
            _collection = database.GetCollection<Message>("Messages");
        }

        /// <summary>
        /// Saves the specified message to the database.
        /// </summary>
        /// <remarks>This method asynchronously inserts the provided message into the underlying database
        /// collection. Ensure that the <paramref name="message"/> object is properly initialized before calling this
        /// method.</remarks>
        /// <param name="message">The message to save. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task SaveMessage(Message message)
        {
            await _collection.InsertOneAsync(message);
        }

        /// <summary>
        /// Retrieves a list of messages associated with the specified room.
        /// </summary>
        /// <param name="roomId">The unique identifier of the room whose messages are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of  <see cref="Message"/>
        /// objects associated with the specified room. If no messages are found,  the list will be empty.</returns>
        public async Task<List<Message>> GetMessagesByRoom(int roomId)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.RoomId, roomId);
            return await _collection.Find(filter).ToListAsync();
        }
    }
}
