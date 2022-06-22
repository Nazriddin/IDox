namespace IDoxInstance.Entities.DTOs
{
    public class UpdatefileDTO
    {
        public int? fileId { get; set; }
        public IList<IFormFile>? file { get; set; }
        public int? ReceiverUserId { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
