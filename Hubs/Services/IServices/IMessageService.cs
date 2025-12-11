using API_CAPITAL_MANAGEMENT.Hubs.Entities;

namespace API_CAPITAL_MANAGEMENT.Hubs.Services.IServices
{
    /// <summary>
    /// Defines a contract for managing messages, including saving and retrieving messages associated with specific
    /// rooms.
    /// </summary>
    /// <remarks>This interface provides methods for saving messages and retrieving messages by room.
    /// Implementations of this interface should ensure thread safety and handle any necessary data persistence or
    /// retrieval logic.</remarks>
    public interface IMessageService
    {
        /// <summary>
        /// Saves the specified message to the underlying storage.
        /// </summary>
        /// <param name="message">The message to save. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task SaveMessage(Message message);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<List<Message>> GetMessagesByRoom(int roomId);
    }
}
