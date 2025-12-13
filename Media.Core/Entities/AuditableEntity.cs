namespace Media.Core.Entities
{
    public class AuditableEntity
    {
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
