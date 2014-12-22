using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OSProject.Model.Structures
{
    public class UserInfo
    {
        [Key, ForeignKey("user")]
        public Int64 userId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string UserType { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string SurName { get; set; }
        [Required]
        public string Date { get; set; }
        public virtual User user { get; set; }
    }
}
