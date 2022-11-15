using System;

namespace GRP.Models.System
{
    public class HomePage : BaseHomePage
    {
        public Notification[] UserNotifications { get; set; }
        public string UserNtfs { get; set; }
    }
}
