using System;
namespace HomeSearch.Shared.Models
{
	public class HomeParameters
	{
        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 6;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public string? SearchTerm { get; set; }
        public string OrderBy { get; set; } = "name";
    }
}

