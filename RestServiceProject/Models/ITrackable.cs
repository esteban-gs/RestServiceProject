using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestServiceProject.Models
{
    public interface ITrackable
    {
        public DateTime CreatedDate { get; set; }
    }
}
