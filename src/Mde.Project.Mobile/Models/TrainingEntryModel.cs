using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mde.Project.Mobile.Models
{
    public class TrainingEntryModel
    {
        public DateTime Date { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; } = string.Empty;
        public List<TechniqueScoreModel> TechniqueScores { get; set; } = new();
        public string? Comment { get; set; }
        public int Id { get; set; }
        public List<TrainingAttachmentModel> Attachments { get; set; } = new();
        public List<OpponentNoteModel>? OpponentNotes { get; set; } = new();
        public string Color { get; set; } = string.Empty;
    }
}
