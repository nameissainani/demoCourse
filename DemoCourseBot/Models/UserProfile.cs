using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoCourseBot.Models
{
    public class UserProfile
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Description { get; set; }
        public DateTime CallBackTime { get; set; }
        public string PhoneNumber { get; set; }
        public string Bug { get; set; }
    }
}
