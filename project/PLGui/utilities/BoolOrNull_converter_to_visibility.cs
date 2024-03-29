﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PLGui.utilities
{
    class BoolOrNull_converter_to_visibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (value is bool flag)
            {
                if (targetType.Name == "Double")//if the target type is property "width"
                {
                    if (flag)
                        return "auto";
                    return 0;
                }

                if (flag)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
            if (value is null)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
