using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darktide_Armoury_Monitor
{
    public class ItemOffer
    {
        public string name;
        public int rating;
        public int credits;
        public int marks;

        public ItemOffer(string name, int rating, int credits, int marks)
        {
            this.name = name;
            this.rating = rating;
            this.credits = credits;
            this.marks = marks;
        }

        public override string ToString()
        {
            return name;
        }

    }
}
