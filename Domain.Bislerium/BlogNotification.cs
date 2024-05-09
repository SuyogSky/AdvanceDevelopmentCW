using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium
{
    public class BlogNotification
    {
            [Key]
            public Guid Id { get; set; }

            [Required]
            public int UserId { get; set; }  

            [Required]
            public string? Message { get; set; }
            public DateTime NotificationDate { get; set; }

            public virtual AppUser? User { get; set; }
          
        }

}
