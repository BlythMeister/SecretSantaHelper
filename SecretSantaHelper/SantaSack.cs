using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecretSantaHelper
{
    [Serializable]
    public class SantaSack
    {
        public Template Template { get; set; }
        public List<Participant> Participants { get; set; }

        public SantaSack()
        {
            Participants=new List<Participant>();
            Template = new Template();
        }
    }
}
