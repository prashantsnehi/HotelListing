using System;
using System.Collections.Generic;

namespace MyWebAPI.Models
{
    public class CountryDTO: CreateCountryDTO
    {
        public int Id { get; set; }
        public IList<HotelDTO> Hotels { get; set; }
    }
}
