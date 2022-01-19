using Nest;

namespace FileSearch
{
    public class Document
    {
        public int Id { get; set; }
        public System.DateTime? DataChange { get; set; }
        public string ContentBase64 { get; set; }
        public Attachment Attachment { get; set; }
    }
}
