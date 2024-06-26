using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAT.Applications.ConvAssistTestApp
{
    internal class ASRResponseRaw
    {
        public String RawResponse { get; set; }

        public ASRResponseRaw(String rawResponse)
        {
            this.RawResponse = rawResponse;
        }   
    }
}
