using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API_CAPITAL_MANAGEMENT.Hubs.Entities
{
    /// <summary>
    /// Represents a message sent by a user, including its content, sender, and timestamp.
    /// </summary>
    /// <remarks>This class is designed to store information about a single message, including the sender's
    /// username,  the message content, and the time the message was sent. The <see cref="Id"/> property serves as a
    /// unique  identifier for the message.</remarks>
    public class Message
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        [BsonId]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the unique identifier for the room.
        /// </summary>
        public required int RoomId { get; set; } //OrgId

        //public required int UserId { get; set; } //UserId (Not used xD)
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public required string UserEmail { get; set; } //User name of chat (email)

        /// <summary>
        /// Gets or sets the text of the message.
        /// </summary>
        public required string MessageText { get; set; }
        /// <summary>
        /// Gets or sets the date and time when the message was sent.
        /// </summary>
        public DateTime SentAt { get; set; }
    }
}
