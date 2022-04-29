using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;

namespace Client.Ext
{
    public class RandomColorExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Random rnd = new Random();
            var color = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(1, 255),
              (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255)));
            return color;
        }
    }
}
