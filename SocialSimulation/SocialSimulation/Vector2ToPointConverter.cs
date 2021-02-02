using System;
using System.Globalization;
using System.Numerics;
using System.Windows;
using System.Windows.Data;

namespace SocialSimulation
{
    public class Vector2ToPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Vector2 v)
            {
                if (parameter is string p && !string.IsNullOrEmpty(p))
                {
                    if (p.Equals("x"))
                        return v.X;
                    return v.Y;
                }
            }

            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Point v)
            {
                return new Vector2((float)v.X, (float)v.Y);
            }

            return new Vector2(0, 0);
        }
    }
}