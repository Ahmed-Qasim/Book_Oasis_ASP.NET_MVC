using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Book_Oasis.Models
{
	public class Category
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(30)]
		public string Name { get; set; }

		[DisplayName("Display Order")]
		[Range(1, 100, ErrorMessage = "Range must be between 1 and 100")]
		public int DisplayOrder { get; set; }
	}
}
