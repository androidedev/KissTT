using System;
using System.Windows.Data;

namespace ktt3
{

    public class TimeConverter : IValueConverter
    {
        //Convert method gets called when source updates target object.
        //The data binding engine calls this method when it propagates a value from the binding source to the binding target.
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is null)
                return Binding.DoNothing;

            bool p = DateTime.TryParse(value.ToString(), out DateTime res);
            if (p)
                return res.ToString("dd/MM/yyyy HH:mm:ss");
            else
                return Binding.DoNothing;
        }

        //ConvertBack method gets called when target updates source object.
        //The data binding engine calls this method when it propagates a value from the binding target to the binding source
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is null)
                return Binding.DoNothing; // return value; 

                if (string.IsNullOrEmpty(value.ToString()))
                return null;


            if ((value is null) || string.IsNullOrEmpty(value.ToString()))
                return Binding.DoNothing;

            bool p = DateTime.TryParse(value.ToString(), out DateTime res);
            if (p)
            {
                return res;
            }
            else
            {
                return value;
            }
        }
    }

}
