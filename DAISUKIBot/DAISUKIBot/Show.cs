using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace DAISUKIBot
{
    public struct Information
    {
        public string Website;
        public string Title;
        public string TitleURL;
    };

    public class Show
    {
        private const string BASE_URL = "https://www.daisuki.net/";
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
            // The Wildcard for the DAISUKIBot is the base URL for streaming links.
            Wildcard = wildcard;
        }

        public void GetShowDataAndPost(object o, DoWorkEventArgs args)
        {
            XmlDocument Feed = new XmlDocument();
            try
            {
                Feed.Load(BASE_URL + "api2/seriesdetail/" + Source);
            }
            catch
            {
                args.Result = "Failed connect for " + Title;
                return;
            }
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
