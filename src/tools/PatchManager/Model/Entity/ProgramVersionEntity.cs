using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PatchManager.Model.Entity
{
    [Table("program_version")]
    class ProgramVersionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column]
        public int ManagedProgramId { get; set; }
        public virtual ManagedProgramEntity ManagedProgram { get; set; }

        [Column]
        public int Major { get; set; }

        [Column]
        public int Minor { get; set; }

        [Column]
        public int Patch { get; set; }
        // Todo: Verification
    }
}
