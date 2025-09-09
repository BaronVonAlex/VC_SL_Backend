using System.ComponentModel.DataAnnotations.Schema;

namespace VC_SL.Models
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string UsernameHistory { get; set; } = null!;
    }
}
