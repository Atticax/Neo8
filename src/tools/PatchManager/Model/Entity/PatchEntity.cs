using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PatchManager.Model.Entity
{
    [Table("patch")]
    class PatchEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        public int ManagedProgramId { get; set; }
        public virtual ManagedProgramEntity ManagedProgram { get; set; }

        [Column]
        public int FromVersionId { get; set; }
        public virtual ProgramVersionEntity FromVersion { get; set; }

        [Column]
        public int ToVersionId { get; set; }
        public virtual ProgramVersionEntity ToVersion { get; set; }
        // Todo: File(s)
    }
}
