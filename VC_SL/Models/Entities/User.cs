using System.ComponentModel.DataAnnotations.Schema;

namespace VC_SL.Models.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Column("username_history")]
        public string UsernameHistory { get; set; } = null!;

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
