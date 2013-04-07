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
        private int ideasPerPage = 30;
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

        public async void UpdateIdea(IdeaItem idea)
        {
            await ideaTable.UpdateAsync(idea);
        }
        public MobileServiceCollectionView<IdeaItem> GetCollectionView()
        {
            return ideaTable.ToCollectionView();
        }

        /// <summary>
        /// Each page we get ideasPerPage records
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<List<IdeaItem>> GetIdeas(int page)
        {
            if(page <= 0)
                throw new Exception("Invalid Arg");

            return await ideaTable.Skip(ideasPerPage * (page - 1)).Take(ideasPerPage).OrderByDescending(x => x.Date).ToListAsync();
        }

        public async Task<List<IdeaItem>> GetSortedIdeaByVoteCount(bool descending, int page)
        {
            if (descending)
            {
                return await ideaTable.Skip(ideasPerPage * (page - 1)).Take(ideasPerPage).OrderByDescending(x => x.VoteCount).ToListAsync();
            }
            else
            {
                return await ideaTable.Skip(ideasPerPage * (page - 1)).Take(ideasPerPage).OrderBy(x => x.VoteCount).ToListAsync();
            }
        }

        public async Task<List<IdeaItem>> GetSortedIdeaByDate(bool descending, int page)
        {
            if (descending)
            {
                return await ideaTable.Skip(ideasPerPage * (page - 1)).Take(ideasPerPage).OrderByDescending(x => x.Date).ToListAsync();
            }
            else
            {
                return await ideaTable.Skip(ideasPerPage * (page - 1)).Take(ideasPerPage).OrderBy(x => x.Date).ToListAsync();
            }
        }

        public async Task<List<IdeaItem>> GetAllIdeas()
        {
            return await ideaTable.OrderByDescending(x => x.Date).ToListAsync();
        }

        public async Task<List<IdeaItem>> GetSortedIdeaByVoteCount(bool descending)
        {
            if (descending)
            {
                return await ideaTable.OrderByDescending(x => x.VoteCount).ToListAsync();
            }
            else
            {
                return await ideaTable.OrderBy(x => x.VoteCount).ToListAsync();
            }
        }

        public async Task<List<IdeaItem>> GetSortedIdeaByDate(bool descending)
        {
            if (descending)
            {
                return await ideaTable.OrderByDescending(x => x.Date).ToListAsync();
            }
            else
            {
                return await ideaTable.OrderBy(x => x.Date).ToListAsync();
            }
            
        }
    }
}
