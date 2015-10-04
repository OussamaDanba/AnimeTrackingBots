using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchyrollBot
{
    public class Show
    {
        private int Id;
        private string Source, InternalTitle, Title;
        private decimal InternalOffset, AKAOffset;

        public Show(int id, string source, string internalTitle, string title, decimal internalOffset, decimal AKAOffset)
        {
            Id = id;
            Source = source;
            InternalTitle = internalTitle;
            Title = title;
            InternalOffset = internalOffset;
            this.AKAOffset = AKAOffset;
        }

        public void GetShowDataAndPost(object o, DoWorkEventArgs args)
        {
            // Empty for now
        }

        public override string ToString()
        {
            return Title;
        }

        public override bool Equals(object obj)
        {
            var Show = obj as Show;
            
            if (Show == null)
            {
                return false;
            }

            return Id == Show.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
