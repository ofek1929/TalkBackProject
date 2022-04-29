using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Client.Models
{
    public struct ChatMessageDetails
    {
        public string Text { get; set; }
        public bool IsYou { get; set; }
        public SolidColorBrush Color { get; set; }
        public string Time { get; set; }
    }
}
