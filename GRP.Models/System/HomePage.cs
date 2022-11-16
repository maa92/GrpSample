using System;

namespace GrpSample.Models.System
{
    public class HomePage : BaseHomePage
    {
        public Notification[] UserNotifications { get; set; }
        public string UserNtfs { get; set; }
    }
}
