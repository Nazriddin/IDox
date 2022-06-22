namespace IDoxInstance.Entities.DTOs
{
    public class FileDTO
    {
        public IList<IFormFile> formFiles { get; set; }
       // public string UserEmail { get; set; } //убрать
        public int ReceiverUserId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
