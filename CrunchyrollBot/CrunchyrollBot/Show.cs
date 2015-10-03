using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchyrollBot
{
    public class Show
    {
        private int ID;
        private string source, internalTitle, title;
        private decimal internalOffset, AKAOffset;

        public Show(MainLogic mainLogic, int ID, string source, string internalTitle, string title, decimal internalOffset, decimal AKAOffset)
        {
            this.ID = ID;
            this.source = source;
            this.internalTitle = internalTitle;
            this.title = title;
            this.internalOffset = internalOffset;
            this.AKAOffset = AKAOffset;
        }

        public override string ToString()
        {
            return title;
        }

        public override bool Equals(object obj)
        {
            var show = obj as Show;
            
            if(show == null)
            {
                return false;
            }

            return this.ID == show.ID;
        }

        public override int GetHashCode()
        {
            return ID;
        }
    }
}
