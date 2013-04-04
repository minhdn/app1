using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    public interface IIdeaDataSource
    {
        List<Idea> GetIdeasList();

        List<Idea> GetSortedIdea();
    }
}
