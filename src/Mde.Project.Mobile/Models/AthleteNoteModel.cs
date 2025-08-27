using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mde.Project.Mobile.Models
{
    public class AthleteNoteModel
    {
        public int JudokaId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }        
        public string Comment { get; set; } = string.Empty;
    }
}
