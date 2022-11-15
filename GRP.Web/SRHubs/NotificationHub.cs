using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GRP.Web.SRHubs
{
    [Authorize]
    [HubName("reqNotifyHub")]
    public class RequestsNotificationHub : Hub
    {
        //public override Task OnConnected()
        //{
        //    string userId = ((ClaimsIdentity)Context.User.Identity).Claims.First(c => c.Type == "uid").Value; ;
        //    Groups.Add(Context.ConnectionId, userId);

        //    return base.OnConnected();
        //}
    }
}