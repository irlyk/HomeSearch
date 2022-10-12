using System;
using System.ComponentModel.DataAnnotations;

namespace HomeSearch.Shared.Models
{
    public class Address
    {
        [Required(ErrorMessage = "Город обязателен")]
        public string City { get; set; }

        [Required(ErrorMessage = "Улица обязательна")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Дом обязателен")]
        public string Home { get; set; }
    }
}
