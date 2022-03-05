using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Models
{
    public class Card
    {
        public Card()
        {
            this.Receipts = new HashSet<Receipt>();
        }
        public string Id { get; set; }
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
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<Receipt> Receipts { get; set; }

    }
}
