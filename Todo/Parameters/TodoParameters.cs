using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Todo.Parameters
{
    public class TodoParameters
    {
        public string type { get; set; }
        public string name { get; set; }

        public int? minOrder { get; set; }
        public int? maxOrder { get; set; }

        public string order { 
            get { return this._order; }
            set
            {
                Regex regex = new Regex(@"^\d-\d$");
                if (regex.IsMatch(value)) 
                {
                    minOrder = Convert.ToInt32(value.Split('-')[0]);
                    maxOrder = Convert.ToInt32(value.Split('-')[1]);
                }
                this._order = value;
            }
        }
        private string _order;

    }
}
