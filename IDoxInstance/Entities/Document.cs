using System.ComponentModel.DataAnnotations.Schema;

namespace IDoxInstance.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public int ReceiverUserId { get; set; }
        public int CreatedUserId { get; set; }
        
        public string DocumentName { get; set; }
        public string DocumentType  { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Path { get; set; }


        public virtual User ReceiverUser { get; set; }
        
    }
}
