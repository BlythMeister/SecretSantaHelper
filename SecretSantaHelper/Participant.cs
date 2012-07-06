using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecretSantaHelper
{
    public class Participant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        
        public string DisplayValue()
        {
            return Name + " - " + EmailAddress;
        }

        public string OutputValue()
        {
            return Name + "," + EmailAddress;
        }
    }
}
