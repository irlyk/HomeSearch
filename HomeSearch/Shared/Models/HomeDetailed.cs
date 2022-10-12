using System.ComponentModel.DataAnnotations;

namespace HomeSearch.Shared.Models
{
	public class HomeDetailed
	{
		public string? Id { get; set; }

		public List<ImageFile> Screens { get; set; }

		public ImageFile DefaultScreen { get; set; }

		public Address HomeAddress { get; set; } = new Address();

        [Required(ErrorMessage = "Название обязательно")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Описание обязательно")]
        public string Details { get; set; }

		public decimal Rating { get; set; }

		public List<string> Tags { get; set; }

		[Required(ErrorMessage = "Цена не указана")]
		public decimal Price { get; set; }

		public List<Reserve> Reserves { get; set; }
    }
}

