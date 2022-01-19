using System.Collections.Generic;

namespace FileSearch
{
    public class ResponseElastic<T>
    {
        public string Id { get; set; }

        public T Documents { get; set; }

        public IEnumerable<string> Hits { get; set; }

        public IEnumerable<string> Suggest { get; set; }
    }
}
