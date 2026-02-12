using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreIdentity.API.Identity.Models
{
    public class TwilioSettingModel
    {
        public string TwilioAccountSid { get; set; }
        public string TwilioAuthToken { get; set; }
        public string TwilioFromNumber { get; set; }
        public string TwilioUrl { get; set; }

    }
}
