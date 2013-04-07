using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private int ideaMaxLength = 310;
        private SortBy currentSort;
        private Button btnUp;
        private Button btnDown;
        private int currentPage = 1;

        enum SortBy
        {
            RatingDescending,
            Rating,
            DateDescending,
            Date,
            Default
        }


        public IdeaView()
        {
            this.InitializeComponent();
            data = new IdeaDatasource();
            Load();
        }

        async void Load()
        {
             currentSort = SortBy.Default;
             this.DefaultViewModel["Items"] = await data.GetIdeas(currentPage);
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
            currentSort = SortBy.Default;
            this.DefaultViewModel["Items"] = await data.GetIdeas(currentPage);
        }

        #region Refresh
        private async void RefreshIdeasPage()
        {
            if (currentSort == SortBy.Default)
            {
                this.DefaultViewModel["Items"] = await data.GetIdeas(currentPage);
            } 
            else if (currentSort == SortBy.Date)
            {
                this.DefaultViewModel["Items"] = await data.GetSortedIdeaByDate(false,currentPage);
            }
            else if (currentSort == SortBy.DateDescending)
            {
                this.DefaultViewModel["Items"] = await data.GetSortedIdeaByDate(true, currentPage);
            }
            else if (currentSort == SortBy.Rating)
            {
                this.DefaultViewModel["Items"] = await data.GetSortedIdeaByVoteCount(false, currentPage);
            }
            else if (currentSort == SortBy.RatingDescending)
            {
                this.DefaultViewModel["Items"] = await data.GetSortedIdeaByVoteCount(true, currentPage);
            }
        }

        private void Refresh_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshIdeasPage();
        }

        #endregion

        #region Save Content

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            //Create a panel as the root of the menu UI.
            StackPanel panel = new StackPanel();
            panel.Background = BottomAppBar1.Background;
            panel.Height = 140;
            panel.Width = 500;
            panel.Margin = new Thickness(4, 0, 0, 0);

            //Add text box to the menu UI.
            txtContent = new TextBox();
            txtContent.MaxLength = ideaMaxLength;
            txtContent.Text = "Your ideas...";
            txtContent.GotFocus += txtContent_GotFocus;

            Button saveButton = new Button();
            saveButton.Content = "Save Idea";
            saveButton.Click += saveButton_Click;

            addMessage = new TextBlock();
            addMessage.Visibility = Visibility.Collapsed;

            panel.Children.Add(txtContent);
            panel.Children.Add(saveButton);
            panel.Children.Add(addMessage);

            //Add the menu root panel as the Popup content.
            Popup popUp = ViewUtilities.CreatePopup(panel, Window.Current.CoreWindow.Bounds.Left + 4
                                                    , Window.Current.CoreWindow.Bounds.Bottom - BottomAppBar1.ActualHeight - panel.Height - 4);

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
                    if (idea.FullContent.Length > ideaMaxLength)
                    {
                        idea.ShortContent = idea.FullContent.Substring(0, ideaMaxLength - 3) + "...";
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
        #endregion

        #region Help Button
        /// <summary>
        /// Trigger help button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Help_OnClick(object sender, RoutedEventArgs e)
        {
            //Add command buttons to the menu UI
            Button aboutButton = ViewUtilities.CreateButton("About", aboutButton_Click);

            //Create a panel as the root of the menu UI.
            StackPanel panel = new StackPanel();
            panel.Background = BottomAppBar1.Background;
            panel.Height = 140;
            panel.Width = 180;

            panel.Children.Add(aboutButton);

            //Add the menu root panel as the Popup content.
            Popup popUp = ViewUtilities.CreatePopup(panel, Window.Current.CoreWindow.Bounds.Right - panel.Width - 4
                                                    , Window.Current.CoreWindow.Bounds.Bottom - BottomAppBar1.ActualHeight - panel.Height - 4);
            //Open popup
            popUp.IsOpen = true;
        }

        async void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog md = new MessageDialog("Thank you for using this app! \n Created by nightowl, any comments and feature requests please send to MINHDEV@outlook.com", "About me");
            await md.ShowAsync();
        }
        #endregion

        #region Sorting
        /// <summary>
        /// trigger sort button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sort_OnClick(object sender, RoutedEventArgs e)
        {

            //Add command buttons to the menu UI.
            Button byRatingHighButton = ViewUtilities.CreateButton("highest rating", SortByRatingHigh_Click);
            Button byRatingLowButton = ViewUtilities.CreateButton("lowest rating", SortByRatingLow_Click);
            Button byDateLastestButton = ViewUtilities.CreateButton("latest", SortByDateLatest_Click);
            Button byDateEarliesButton = ViewUtilities.CreateButton("earliest", SortByDateEarliest_Click);

            //Create a panel as the root of the menu UI.
            StackPanel panel = new StackPanel();
            panel.Background = BottomAppBar1.Background;
            panel.Height = 140;
            panel.Width = 180;

            panel.Children.Add(byRatingHighButton);
            panel.Children.Add(byRatingLowButton);
            panel.Children.Add(byDateLastestButton);
            panel.Children.Add(byDateEarliesButton);
            //Add the menu root panel as the Popup content.
            Popup popUp = ViewUtilities.CreatePopup(panel, Window.Current.CoreWindow.Bounds.Left + 4
                                                    , Window.Current.CoreWindow.Bounds.Bottom - BottomAppBar1.ActualHeight - panel.Height - 4);
            //Open popup
            popUp.IsOpen = true;
        }

        private async void SortByRatingHigh_Click(object sender, RoutedEventArgs e)
        {
            currentSort = SortBy.Rating;
            this.DefaultViewModel["Items"] = await data.GetSortedIdeaByVoteCount(true, currentPage);
        }

        private async void SortByDateLatest_Click(object sender, RoutedEventArgs e)
        {
            currentSort = SortBy.Date;
            this.DefaultViewModel["Items"] = await data.GetSortedIdeaByDate(true, currentPage);
        }

        private async void SortByRatingLow_Click(object sender, RoutedEventArgs e)
        {
            currentSort = SortBy.Rating;
            this.DefaultViewModel["Items"] = await data.GetSortedIdeaByVoteCount(false, currentPage);
        }

        private async void SortByDateEarliest_Click(object sender, RoutedEventArgs e)
        {
            currentSort = SortBy.Date;
            this.DefaultViewModel["Items"] = await data.GetSortedIdeaByDate(false, currentPage);
        }

        #endregion

        #region Navigation

        private async void Home_OnClick(object sender, RoutedEventArgs e)
        {
            currentPage = 1;
            CurrentPage.Text = "[ " + currentPage + " ]";
            await MovePage();
        }

        private async void Next_OnClick(object sender, RoutedEventArgs e)
        {
            currentPage++;
            CurrentPage.Text = "[ " + currentPage +" ]";
            await MovePage();
        }

        private async void Prev_OnClick(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                CurrentPage.Text = "[ " + currentPage + " ]";
                await MovePage();
            } 
        }

        private async Task MovePage()
        {
            List<IdeaItem> listItems = new List<IdeaItem>();
            if (currentSort == SortBy.Default)
            {
                listItems = await data.GetIdeas(currentPage);
            }
            else if (currentSort == SortBy.Date)
            {
                listItems = await data.GetSortedIdeaByDate(false, currentPage);
            }
            else if (currentSort == SortBy.Rating)
            {
                listItems = await data.GetSortedIdeaByVoteCount(false, currentPage);
            }
            else if (currentSort == SortBy.DateDescending)
            {
                listItems = await data.GetSortedIdeaByDate(true, currentPage);
            }
            else if (currentSort == SortBy.RatingDescending)
            {
                listItems = await data.GetSortedIdeaByVoteCount(true, currentPage);
            }
            //No more, disable next button now
            if (listItems.Count == 0)
            {
                Next.IsEnabled = false;
            }
            else
            {
                Next.IsEnabled = true;
            }
            this.DefaultViewModel["Items"] = listItems;
        }

        #endregion 

        #region Voting


        private void BtnVoteUp_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btnUp = btn;
            var selectedIdea = btn.DataContext as IdeaItem; 
            if (selectedIdea != null)
            {
                selectedIdea.VoteCount++;
                data.UpdateIdea(selectedIdea);
                btn.IsEnabled = false;
                if (btnDown != null) btnDown.IsEnabled = true;
            }
        }

        private void BtnVoteDown_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btnDown = btn;
            var selectedIdea = btn.DataContext as IdeaItem;
            if (selectedIdea != null)
            {
                if (selectedIdea.VoteCount > 0)
                {
                    selectedIdea.VoteCount--;
                    data.UpdateIdea(selectedIdea);
                    btn.IsEnabled = false;
                    if (btnUp != null) btnUp.IsEnabled = true;
                }
            }
        }

        #endregion
    }
} 
