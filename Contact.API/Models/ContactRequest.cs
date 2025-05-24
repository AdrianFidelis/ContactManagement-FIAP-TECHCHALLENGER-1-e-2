namespace Contact.API.Models
{
    public class ContactRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public PhoneRequest Phone { get; set; }
    }
}
