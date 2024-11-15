using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAT.Extensions.Default.DialogSenseAgents.DialogSenseAgentASR
{
    internal class ASRResponse
    {
        public String model_size { get; set; }

        public String timstamp { get; set; }
        public String language { get; set; }

        public decimal duration { get; set; }

        public String text { get; set; }

        public ASRResponse(String model_size, String timstamp, String language, decimal duration, String text)
        {
            this.model_size = model_size;
            this.timstamp = timstamp;
            this.language = language;
            this.duration = duration;
            this.text = text;
        }
    }
}
