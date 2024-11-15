using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAT.Extensions.Default.DialogSenseAgents.DialogSenseAgentASR
{
    internal class StartMessage
    {
        public bool crg_on { get; set; }

        public int device_index { get; set; }

        public string model_size { get; set; }

        public bool save_audio { get; set; }

        public StartMessage()
        {
            crg_on = false;
            device_index = -1;
            model_size = String.Empty;
            save_audio = false;
        }

        public StartMessage(bool crg_on, int device_index, string model_size, bool save_audio)
        {
            this.crg_on = crg_on;
            this.device_index = device_index;
            this.model_size = model_size;
            this.save_audio = save_audio;
        }
    }
}
