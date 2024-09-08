using hardware_store_api.Controllers.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;


namespace hardware_store_api.Controllers.websockets
{
    [Route("api/notification")]
    public class NotificationWebSocketController : ControllerBase
    {
        private IHubContext<MessageHub> _hubContext;

        public NotificationWebSocketController(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
        }
    }
}
