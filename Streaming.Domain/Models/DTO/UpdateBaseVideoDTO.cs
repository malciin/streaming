using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Domain.Models.DTO
{
    public class UpdateBaseVideoDTO
    {
        TimeSpan VideoLength { get; set; }
        byte[] VideoZipped { get; set; }
        string Manifest { get; set; }
        string ProcessingInfo { get; set; }
    }
}
