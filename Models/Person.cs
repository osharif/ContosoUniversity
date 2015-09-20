using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ContosoUniversity.Models
{
    public abstract class Person
    {
        [Key]
        public int PersonID { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        [StringLength(50, MinimumLength=1)]
        [Display(Name="Last Name")]
        public string LastName { get; set; }

        [Column("FirstName")]
        [StringLength(50, MinimumLength=2, ErrorMessage="First name must be between 2 and 50 characters.")]
        [Display(Name="First Name")]
        public string FirstMidName { get; set; }

        public string FullName
        {
            get { return LastName +", "+ FirstMidName;  }
        }

        public virtual ICollection<File> Files { get; set; }
    }
}