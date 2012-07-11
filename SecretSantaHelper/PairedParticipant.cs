using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecretSantaHelper
{
    public class PairedParticipant : Participant
    {
        public Participant PairedWith { get; set; }
    }
}
