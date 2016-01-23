using Microsoft.Advertising.Mobile.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace FillWords.Phone._8._0.Advertising
{
    public class MicrosoftAds
    {
        public void AddAds(Grid grid, string unitId)
        {
#if FREE7
            AdControl adControl = new AdControl("01ee02be-8363-4997-8d6b-d2d2a9151b17", unitId, true);
            adControl.Width = 480;
            adControl.Height = 80;
            grid.Children.Add(adControl);
#endif
        }
    }
}
