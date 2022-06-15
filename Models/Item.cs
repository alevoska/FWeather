using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWeather.Models
{

    public class Item
    {
        public string Date { get; set; }

        public int LocationId { get; set; }

        public Location Location { get; set; }

        public double Temperature { get; set; }

        [Range(0, Double.PositiveInfinity)]
        public double Precipitation { get; set; }

        [Range(0, Double.PositiveInfinity)]
        public double WindStrength { get; set; }

        [Range(0, 360)]
        public double WindDirection { get; set; }
    }
}