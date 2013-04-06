using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using App1.Models;
using Windows.System.UserProfile;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace App1
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class IdeaView : App1.Common.LayoutAwarePage
    {
        private IdeaDatasource data;
        private TextBox txtContent;
        private TextBlock addMessage;

        public IdeaView()
        {
            this.InitializeComponent();
             data = new IdeaDatasource();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            this.DefaultViewModel["Items"] = await data.GetAllIdeas();
        }

        private async void RefreshIdeasPage()
        {
            this.DefaultViewModel["Items"] = await data.GetAllIdeas();
        }

        private void Refresh_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshIdeasPage();
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            //Create a Popup to host the sort menu.
            Popup popUp = new Popup();
            popUp.IsLightDismissEnabled = true;

            //Create a panel as the root of the menu UI.
            StackPanel panel = new StackPanel();
            panel.Background = BottomAppBar1.Background;
            panel.Height = 140;
            panel.Width = 500;
            panel.Margin = new Thickness(4,0,0,0);

            //Add text box to the menu UI.
            txtContent = new TextBox();
            txtContent.Text = "Your ideas...";
            txtContent.GotFocus += txtContent_GotFocus;

            Button saveButton = new Button();
            saveButton.Content = "Save";
            saveButton.Click += saveButton_Click;

            addMessage = new TextBlock();
            addMessage.Visibility = Visibility.Collapsed;

            panel.Children.Add(txtContent);
            panel.Children.Add(saveButton);
            panel.Children.Add(addMessage);

            //Add the menu root panel as the Popup content.
            popUp.Child = panel;

            //Calculate the placement of the Popup menu.
            popUp.HorizontalOffset = Window.Current.CoreWindow.Bounds.Left + 4;
            popUp.VerticalOffset = Window.Current.CoreWindow.Bounds.Bottom - BottomAppBar1.ActualHeight - panel.Height - 4;

            //Open popup
            popUp.IsOpen = true;
            
        }

        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            IdeaItem idea = new IdeaItem();
            if (txtContent != null)
            {
                if (!String.IsNullOrEmpty(txtContent.Text))
                {
                    idea.FullContent = txtContent.Text.Trim();
                    if (idea.FullContent.Length > 130)
                    {
                        idea.ShortContent = idea.FullContent.Substring(0, 130) + "...";
                    }
                    else
                    {
                        idea.ShortContent = idea.FullContent;
                    }
                }
                else
                {
                    addMessage.Visibility = Visibility.Visible;
                    addMessage.Text = "Oops, too many people have 'empty' as an idea already :)";
                    addMessage.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                    return;
                }
            }
            else
            {
                return;
            }

            string firstName = await UserInformation.GetFirstNameAsync();
            string lastName = await UserInformation.GetLastNameAsync();
            idea.Publisher = firstName + " " + lastName;

            idea.Date = DateTime.UtcNow;
            data.AddIdea(idea);

            MessageDialog md = new MessageDialog("Thank you for posting new idea :-)");
            await md.ShowAsync();

            //Now do refresh page
            RefreshIdeasPage();
        }

        void txtContent_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtContent != null)
                txtContent.Text = String.Empty;
        }
        

        private void Sort_OnClick(object sender, RoutedEventArgs e)
        {
            //Create a Popup to host the sort menu.
            Popup popUp = new Popup();
            popUp.IsLightDismissEnabled = true;

            //Create a panel as the root of the menu UI.
            StackPanel panel = new StackPanel();
            panel.Background = BottomAppBar1.Background;
            panel.Height = 140;
            panel.Width = 180;

            //Add command buttons to the menu UI.
            Button byRatingButton = new Button();
            byRatingButton.Content = "By rating";
            byRatingButton.Style = (Style)App.Current.Resources["TextButtonStyle"];
            byRatingButton.Margin = new Thickness(20, 5, 20, 5);
            byRatingButton.Click += SortByRating_Click;

            Button byDateButton = new Button();
            byDateButton.Content = "By date";
            byDateButton.Style = (Style)App.Current.Resources["TextButtonStyle"];
            byDateButton.Margin = new Thickness(20, 5, 20, 5);
            byDateButton.Click += SortByDate_Click;

            panel.Children.Add(byRatingButton);
            panel.Children.Add(byDateButton);
            //Add the menu root panel as the Popup content.
            popUp.Child = panel;

            //Calculate the placement of the Popup menu.
            popUp.HorizontalOffset = Window.Current.CoreWindow.Bounds.Left + 4;
            popUp.VerticalOffset = Window.Current.CoreWindow.Bounds.Bottom - BottomAppBar1.ActualHeight - panel.Height - 4;

            //Open popup
            popUp.IsOpen = true;
        }

        private async void SortByRating_Click(object sender, RoutedEventArgs e)
        {
            this.DefaultViewModel["Items"] = await data.GetSortedIdeaByVoteCount();
        }

        private async void SortByDate_Click(object sender, RoutedEventArgs e)
        {
            this.DefaultViewModel["Items"] = await data.GetSortedIdeaByDate();
        }

        private async void Settings_OnClick(object sender, RoutedEventArgs e)
        {
            //Create a Popup to host the sort menu.
            Popup popUp = new Popup();
            popUp.IsLightDismissEnabled = true;

            //Create a panel as the root of the menu UI.
            StackPanel panel = new StackPanel();
            panel.Background = BottomAppBar1.Background;
            panel.Height = 140;
            panel.Width = 180;

            //Add command buttons to the menu UI
            Button aboutButton = new Button();
            aboutButton.Content = "About";
            aboutButton.Style = (Style)App.Current.Resources["TextButtonStyle"];
            aboutButton.Margin = new Thickness(20, 5, 20, 5);
            aboutButton.Click += aboutButton_Click;

            panel.Children.Add(aboutButton);
            //Add the menu root panel as the Popup content.
            popUp.Child = panel;

            //Calculate the placement of the Popup menu.
            popUp.HorizontalOffset = Window.Current.CoreWindow.Bounds.Right - panel.Width - 4;
            popUp.VerticalOffset = Window.Current.CoreWindow.Bounds.Bottom - BottomAppBar1.ActualHeight - panel.Height - 4;

            //Open popup
            popUp.IsOpen = true;
        }

        void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog md = new MessageDialog("Thank you for using this app! \n Created by nightowl, any comments and feature requests please send to MINHDEV@outlook.com","About me");
            md.ShowAsync();
        }

        private void BtnVoteUp_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var selectedIdea = btn.DataContext as IdeaItem; 
            if (selectedIdea != null)
            {
                selectedIdea.VoteCount++;
                data.UpdateIdea(selectedIdea);
            }
        }

        private void BtnVoteDown_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var selectedIdea = btn.DataContext as IdeaItem;
            if (selectedIdea != null)
            {
                if (selectedIdea.VoteCount > 0)
                {
                    selectedIdea.VoteCount--;
                    data.UpdateIdea(selectedIdea);
                }
            }
        } 
    }
} 
