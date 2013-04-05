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
            this.DefaultViewModel["Items"] = await data.GetSortedIdeaByVoteCount();
        }
        private void Refresh_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshIdeasPage();
        }

        private async void Add_OnClick(object sender, RoutedEventArgs e)
        {
            IdeaItem idea = new IdeaItem();
            idea.Publisher = "minh";
            idea.ShortContent = "todo app for w8 and wp8 sync together";
            idea.VoteCount = 1;
            idea.Date = DateTime.UtcNow;

            data.AddIdea(idea);

            MessageDialog md = new MessageDialog("Thank you for posting new idea :-)");
            await md.ShowAsync();

            //Now do refresh page
            //RefreshIdeasPage();
        }

        private async void ItemGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            MessageDialog md = new MessageDialog("clicked");
            await md.ShowAsync();
        }
    }
} 
