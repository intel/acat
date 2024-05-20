using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAT.Applications.ConvAssistTestApp
{
    internal enum DialogTurnType
    {
        User,
        Other
    }

    internal class DialogTurn
    {
        public DialogTurnType TurnType { get; set; }
        public String Text { get; set; }

        public DialogTurn(DialogTurnType turnType, String text)
        {
            TurnType = turnType;
            Text = text;
        }
    }
}
