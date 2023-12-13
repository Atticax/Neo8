using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PatchManager.Model.Entity
{
    [Table("managed_program")]
    class ManagedProgramEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column]
        [MaxLength(256)]
        [Required]
        public string Name { get; set; }
        public virtual ICollection<PatchEntity> Patches { get; set; }
        public virtual ICollection<ProgramVersionEntity> ProgramVersions { get; set; }
    }
}
