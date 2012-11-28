using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecretSantaHelper
{
    [Serializable]
    public class Template
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string FromAddress { get; set; }
        public string DiagnosticDeliveryAddress { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public Template()
        {
            Host = string.Empty;
            Port = string.Empty;
            FromAddress = string.Empty;
            Subject = string.Empty;
            Content = string.Empty;
            DiagnosticDeliveryAddress = string.Empty;
        }
    }
}
