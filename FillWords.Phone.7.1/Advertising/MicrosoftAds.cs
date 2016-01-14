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
            AdControl adControl = new AdControl("4a87b272-5433-4003-a8ce-e0faab24a6d1", unitId, true);
            adControl.Width = 480;
            adControl.Height = 80;
            grid.Children.Add(adControl);
#endif
        }
    }
}
