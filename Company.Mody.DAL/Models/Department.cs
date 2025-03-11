using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Mody.DAL.Models
{
    public class Department: BaseEntity
    {

        public int Id { get; set; }
        [Required(ErrorMessage ="!!! Code Is Required from backend!!!")]
        public string Code { get; set; }

        [Required(ErrorMessage = "!!! Name Is Required from backend!!!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "!!! Date Of Creation Is Required from backend!!!")]
        [DisplayName("Date Of Creation")]
        public DateTime? DateOfCreation { get; set; }
    }
}
