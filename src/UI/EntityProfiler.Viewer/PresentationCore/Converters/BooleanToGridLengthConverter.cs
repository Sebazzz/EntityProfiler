using System;
using System.Windows;

namespace EntityProfiler.Viewer.PresentationCore
{
    public class BooleanToGridLengthConverter : 
        BooleanToValueConverter<GridLength>
    {
        public BooleanToGridLengthConverter()
        {
            Console.WriteLine("BooleanToGridLengthConverter");
        }
    }
}