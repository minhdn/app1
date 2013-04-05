using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace App1.Models
{
    public class IdeaDatasource
    {
        private MobileServiceCollectionView<IdeaItem> ideas;
        private IMobileServiceTable<IdeaItem> ideaTable;

        public IdeaDatasource()
        {
            ideaTable = App.MobileService.GetTable<IdeaItem>();
            ideas = ideaTable.ToCollectionView();
        }

        public IMobileServiceTable<IdeaItem> IdeaTable
        {
            get { return ideaTable; }
        }

        public async void AddIdea(IdeaItem idea)
        {
            await ideaTable.InsertAsync(idea);
        }

        public async Task<List<IdeaItem>> GetAllIdeas()
        {
            return await ideaTable.ToListAsync();
        }

        public async Task<List<IdeaItem>> GetSortedIdeaByVoteCount()
        {
            return await ideaTable.OrderByDescending(x => x.VoteCount).ToListAsync();
        }

        public async Task<List<IdeaItem>> GetSortedIdeaByDate()
        {
            return await ideaTable.OrderBy(x => x.Date).ToListAsync();
        }
    }
}
