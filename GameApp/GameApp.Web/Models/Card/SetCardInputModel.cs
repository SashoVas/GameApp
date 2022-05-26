using GameApp.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace GameApp.Web.Models.Card
{
    public class SetCardInputModel
    {
        [Required]
        public CardType CardType { get; set; }
        [Required]
        [RegularExpression("^([0-9]{10})$",ErrorMessage ="Your card number should be 10 numbers between 0-9")]
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
        [Required]
        public DateTime ExpirationDate { get; set; }
        [Required]
        [MaxLength(30)]
        public string City { get; set; }
        [Required]
        [MaxLength(4)]
        [RegularExpression("^([0-9]{4})$",ErrorMessage ="Your zip code should be 4 numbers between 0-9")]
        public string ZipCode { get; set; }
        [Required]
        [MaxLength(10)]
        [RegularExpression("^([0-9]{10})$", ErrorMessage = "Your phone number should be 10 numbers between 0-9")]
        public string PhoneNumber { get; set; }

        public string? CardId { get; set; }
        public string? RetrunUrl { get; set; }
    }
}
