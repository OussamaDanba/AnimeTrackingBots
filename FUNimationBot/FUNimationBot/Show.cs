using System.ComponentModel;

namespace FUNimationBot
{

    public class Show
    {
        private int Id;
        private string Source, InternalTitle, Title, Wildcard;
        private decimal InternalOffset, AKAOffset;

        public Show(int id, string source, string internalTitle, string title, decimal internalOffset, decimal AKAOffset, string wildcard)
        {
            Id = id;
            Source = source;
            InternalTitle = internalTitle;
            Title = title;
            InternalOffset = internalOffset;
            this.AKAOffset = AKAOffset;
            Wildcard = wildcard;
        }

        public void GetShowDataAndPost(object o, DoWorkEventArgs args)
        {

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