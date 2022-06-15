using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FWeather.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        public string Title { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}