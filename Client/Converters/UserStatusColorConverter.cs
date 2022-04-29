using CommonLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Client.Converters
{
    public class UserStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (UserConnectionStatus)value;
            SolidColorBrush solidColorBrush;
            switch (status)
            {
                case UserConnectionStatus.Offline:
                    solidColorBrush = new SolidColorBrush(Colors.Gray);
                    break;
                case UserConnectionStatus.Online:
                    solidColorBrush = new SolidColorBrush(Colors.Green);
                    break;
                default:
                    solidColorBrush = new SolidColorBrush(Colors.DarkGoldenrod);
                    break;
                
            }
            return solidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
