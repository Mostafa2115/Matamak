using Core.DTO;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IReprosatory
{
    public interface IcountryRepo
    {
        public void AddCountry(CountryD country);
            public void RemoveCountry(int id);
            public void UpdateCountry(CountryD country, int id);
            public CounteryMV GetCountryById(int id);
            public List<CounteryMV> GetAllCountries();
    }
}
