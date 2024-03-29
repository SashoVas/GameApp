﻿using System.ComponentModel.DataAnnotations;

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
        [CreditCard]
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
        [Required]
        public string UserId { get; set; }
        [Required]
        public User User { get; set; }
        public ICollection<Receipt> Receipts { get; set; }

    }
}
