
    public class BulkInviteDto
    {
        public Guid CampagneId { get; set; }
        public List<string> Emails { get; set; } = new List<string>();
    }
