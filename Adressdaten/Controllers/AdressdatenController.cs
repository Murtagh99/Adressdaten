using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Adressdaten.Models;
using Newtonsoft.Json.Linq;
using Adressdaten.Imports;

namespace Adressdaten.Controllers
{
    [Route("api/adressdaten")]
    [ApiController]
    public class AdressdatenController : ControllerBase
    {
        private readonly AdressdatenContext _context;

        public AdressdatenController(AdressdatenContext context)
        {
            _context = context;
        }

        [HttpGet("City")]
        public async Task<ActionResult<IList<City>>> GetCities([FromQuery]City city)
        {
            if(city.PostCode != 0 && !string.IsNullOrEmpty(city.Name))
            {
                return await _context.Cities.Where(c => c.PostCode.ToString().StartsWith(city.PostCode.ToString()) && c.Name.ToLower().StartsWith(city.Name.ToLower())).ToListAsync();
            } else if (city.PostCode != 0)
            {
                return await _context.Cities.Where(c => c.PostCode.ToString().StartsWith(city.PostCode.ToString())).ToListAsync();
            } else if (!string.IsNullOrEmpty(city.Name))
            {
                return await _context.Cities.Where(c => c.Name.ToLower().StartsWith(city.Name.ToLower())).ToListAsync();
            } else
            {
                return new List<City>();
            }
        }

        [HttpGet("Street")]
        public async Task<ActionResult<IList<Street>>> GetStreet([FromQuery]Street street)
        {
            var AdressdatenItem = _context.Streets.Where(s => s.PostCodeFK.ToString().Contains(street.PostCodeFK.ToString()) && s.Name.ToLower().StartsWith(street.Name.ToLower()));
            return await AdressdatenItem.ToListAsync();
        }

        [HttpGet("Streeta")]
        public async Task<ActionResult<IList<Street>>> GetStreeta([FromQuery]Street street)
        {
            var AdressdatenItem = _context.Streets.Where(s => s.PostCodeFK.ToString().Contains(street.PostCodeFK.ToString()) && s.Name.ToLower().StartsWith(street.Name.ToLower()));
            return await AdressdatenItem.ToListAsync();
        }
    }
}