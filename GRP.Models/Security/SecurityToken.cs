using Newtonsoft.Json;

namespace GRP.Models.Security
{
    public class SecurityToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string userId { get; set; }
        public string userLoginSerial { get; set; }
        public string userFName { get; set; }
        public string userDeptName { get; set; }
        public string userType { get; set; }
        public string userCanChangeLogin { get; set; }

        [JsonProperty(".issued")]
        public string issued { get; set; }

        [JsonProperty(".expires")]
        public string expires { get; set; }

        public string error { get; set; }

        public string error_description { get; set; }
    }
}
