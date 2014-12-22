using System;
using System.ComponentModel.DataAnnotations;

namespace OSProject.Model.Structures
{
    public class User
    {
        [Key]
        public Int64 userId { get; set; }
        [Required]
        public string login { get; set; }
        [Required]
        public string password { get; set; }

        [Required]
        public virtual UserInfo userInfo { get; set; }
    }
}
