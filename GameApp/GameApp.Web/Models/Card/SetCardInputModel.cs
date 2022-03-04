using GameApp.Data.Models;

namespace GameApp.Web.Models.Card
{
    public class SetCardInputModel
    {
        public CardType CardType { get; set; }
        public string CardNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNumber { get; set; }
        public string? CardId { get; set; }
    }
}
