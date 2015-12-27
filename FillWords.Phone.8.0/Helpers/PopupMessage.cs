using Coding4Fun.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FillWords.Phone._8._0.Helpers
{
    public class PopupMessage
    {
        public delegate void MyEventHandler();

        public event MyEventHandler OnClick;

        public event MyEventHandler OnOkClick;

        public event MyEventHandler OnCancelClick;

        public StackPanel Body { get; set; }

        public void Show(string message, bool isCancelVisibled = false)
        {
            //var currentAccentBrush = (System.Windows.Media.Brush)Application.Current.Resources["PhoneAccentBrush"];
            var messagePrompt = new MessagePrompt
            {
                Message = message + "\r\n",
                Background = new SolidColorBrush
                {
                    //Color = Colors.Black,
                    Color = Color.FromArgb(255, 194, 11, 138),
                    Opacity = 0.5
                },
                FontSize = 30,
                Width = 400,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsCancelVisible = isCancelVisibled,
            };
            if (Body != null)
            {
                messagePrompt.Body = Body;
            }
            messagePrompt.Completed += messagePrompt_Completed;
            messagePrompt.Show();
        }


        public void messagePrompt_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok)
            {
                if (OnOkClick != null)
                    OnOkClick();
            }
            else
            {
                if (OnCancelClick != null)
                    OnCancelClick();
            }
            if (OnClick != null)
                OnClick();
        }

    }
}
