using API_CAPITAL_MANAGEMENT.Hubs.Entities;
using API_CAPITAL_MANAGEMENT.Hubs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API_CAPITAL_MANAGEMENT.Hubs
{
    /// <summary>
    /// Represents a SignalR hub for managing real-time chat functionality.
    /// </summary>
    /// <remarks>The <see cref="ChatHub"/> class provides a central point for handling client-server
    /// communication  in a chat application. Clients can connect to this hub to send and receive messages in real
    /// time.</remarks>
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly MessageService _messageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatHub"/> class.
        /// </summary>
        /// <param name="messageService">The service used to handle message-related operations within the chat hub.  This parameter cannot be <see
        /// langword="null"/>.</param>
        public ChatHub( MessageService messageService)
        {
            _messageService = messageService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="HubException"></exception>
        public async Task SendMessage( int roomId, string message)
        {
            // Tomar el usuario autenticado del token
            var userEmail = Context.User?.FindFirst("email")?.Value
              ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
              ?? Context.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;



            if (userEmail == null)
                throw new HubException("No se pudo determinar el usuario autenticado.");

            var chatMessage = new Message
            {
                RoomId = roomId,
                UserEmail = userEmail,
                MessageText = message,
                SentAt = DateTime.UtcNow
            };

            await _messageService.SaveMessage(chatMessage);

            await Clients.Group(roomId.ToString())
                         .SendAsync("ReceiveMessage", userEmail, message, chatMessage.SentAt);
        }

        /// <summary>
        /// Loads the chat history for the specified room and sends it to the caller.
        /// </summary>
        /// <remarks>This method retrieves the messages associated with the specified room and sends them
        /// to the caller  using the "LoadChatHistory" event. Ensure the caller has appropriate permissions to access
        /// the room.</remarks>
        /// <param name="roomId">The unique identifier of the chat room whose messages are to be loaded.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task LoadMessages(int roomId)
        {
            var messages = await _messageService.GetMessagesByRoom(roomId);
            await Clients.Caller.SendAsync("LoadChatHistory", messages);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var roomId = httpContext.Request.Query["roomId"];

            if (!string.IsNullOrEmpty(roomId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            }

            await base.OnConnectedAsync();
        }

    }
}
