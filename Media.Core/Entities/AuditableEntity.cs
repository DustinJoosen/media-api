namespace Media.Core.Entities
{
    /// <summary>
    /// The DbContext automatically populates these two fields.
    /// </summary>
    public class AuditableEntity
    {
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
