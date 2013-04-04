using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    class IdeaDatasource : IIdeaDataSource
    {
        public IdeaDatasource()
        {
            
        }

        virtual public bool Match(Idea idea)
        {
            return true;
        }

        public List<Idea> GetIdeasList()
        {
            throw new NotImplementedException();
        }

        public List<Idea> GetSortedIdea()
        {
            return GetIdeasList().FindAll(Match);
        }
    }
}
