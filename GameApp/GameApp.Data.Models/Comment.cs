﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Data.Models
{
    public class Comment
    {
        public Comment()
        {
            this.Comments=new HashSet<Comment>();
        }
        public string Id { get; set; }
        [Required]
        [MaxLength(300)]
        public string Content { get; set; }
        [Required]
        public DateTime PostedOn { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public string? CommentedOnId { get; set; }
        public Comment? CommentedOn { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
