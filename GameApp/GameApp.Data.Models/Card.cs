using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        [MaxLength(10)]
        public string CardNumber { get; set; }
        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(30)]
        public string Address { get; set; }
        [Required]
        [MaxLength(30)]
        public string Country { get; set; }
        public DateTime ExpirationDate { get; set; }
        [Required]
        [MaxLength(30)]
        public string City { get; set; }
        [Required]
        [MaxLength(4)]
        public string ZipCode { get; set; }
        [Required]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<Receipt> Receipts { get; set; }

    }
}
