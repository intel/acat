using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAT.Applications.ConvAssistTestApp
{
    internal class DialogContext
    {
        private List<DialogTurn> Turns;

        public DialogContext()
        {
            Clear();
        }

        public void AddTurn(DialogTurn turn)
        {
            Turns.Add(turn);
        }

        public void Clear()
        {
            Turns = new List<DialogTurn>();
        }

        public override String ToString()
        {
            return ToString(0);
        }

        public String ToString(int numTurns)
        {
            if (numTurns <= 0)
            {
                numTurns = Turns.Count;
            }

            int startIndex = Turns.Count - numTurns;

            if (startIndex < 0)
            {
                startIndex = 0;
            }

            StringBuilder sb = new StringBuilder();

            for (int ii = startIndex; ii < Turns.Count; ii++)
            {
                sb.Append(Turns[ii].TurnType + ":\t" + Turns[ii].Text + "\n");
            }

            return sb.ToString();
        }
    }
}
