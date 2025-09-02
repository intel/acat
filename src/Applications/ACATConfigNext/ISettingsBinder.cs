using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACATConfig
{
    public interface ISettingsBinder
    {
        Control CreateControl(object initialValue, Action<object> onValueChanged);
    }

    public class IntBinder : ISettingsBinder
    {
        public Control CreateControl(object initialValue, Action<object> onValueChanged)
        {
            var nud = new NumericUpDown
            {
                Value = Convert.ToDecimal(initialValue),
                Minimum = int.MinValue,
                Maximum = int.MaxValue,
            };

            nud.ValueChanged += (s, e) => onValueChanged((int)nud.Value);

            return nud;
        }
    }

}
