using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace App1.Models
{
    public static class ViewUtilities
    {
        public static Popup CreatePopup(StackPanel panel, double horizontalOffset, double verticalOffset)
        {
            //Create a Popup to host the sort menu.
            Popup popUp = new Popup();
            popUp.IsLightDismissEnabled = true;

            //Add the menu root panel as the Popup content.
            popUp.Child = panel;

            //Calculate the placement of the Popup menu.
            popUp.HorizontalOffset = horizontalOffset;
            popUp.VerticalOffset = verticalOffset;

            return popUp;
        }

        public static Button CreateButton(string content, RoutedEventHandler button_Click)
        {
            var button = new Button();
            button.Content = content;
            button.Style = (Style)App.Current.Resources["TextButtonStyle"];
            button.Margin = new Thickness(20, 5, 20, 5);
            button.Click += button_Click;
            return button;
        }
    }
}
