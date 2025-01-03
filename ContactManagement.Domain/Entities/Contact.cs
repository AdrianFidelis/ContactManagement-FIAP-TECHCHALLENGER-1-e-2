using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManagement.Domain.Entities
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Phone Phone { get; set; } = new Phone();
        public string Email { get; set; } = string.Empty;        
    }

}
