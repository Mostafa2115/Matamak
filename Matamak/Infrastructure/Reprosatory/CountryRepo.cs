using Core.DTO;
using Core.IReprosatory;
using Core.Models;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Reprosatory
{
    public class CountryRepo : IcountryRepo
    {
        private readonly DataContext dataContext;

        public CountryRepo(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        void IcountryRepo.AddCountry(CountryD country)
        {
            Country country1 = new Country();
            country1.Name = country.Name;
            dataContext.Add(country1);
            dataContext.SaveChanges();

        }

        List<CounteryMV> IcountryRepo.GetAllCountries()
        {
            var countries = dataContext.Countries.ToList();
            var countryDList = new List<CounteryMV>();
            foreach (var country in countries)
            {
                var countryD = new CounteryMV
                {
                    Id = country.id,
                    Name = country.Name
                };
                countryDList.Add(countryD);
            }   
            return countryDList;

        }

        CounteryMV IcountryRepo.GetCountryById(int id)
        {
            var country = dataContext.Countries.FirstOrDefault(c => c.id == id);
            if (country == null)
            {
                throw new Exception("Country not found");
            }
            var countryD = new CounteryMV
            {
                Id = country.id,
                Name = country.Name
            };

            return countryD;
        }

        void IcountryRepo.RemoveCountry(int id)
        {
            var Items = dataContext.Items.Where(i => i.CountryId == id).ToList();
            if (Items.Count > 0)
            {
                throw new Exception("Cannot delete country because it has related items , Delete the items first.");
            }
            var country = dataContext.Countries.FirstOrDefault(c => c.id == id);
            if (country == null)
            {
                throw new Exception("Country not found");
            }
            if (country != null)
            {
                dataContext.Countries.Remove(country);
                dataContext.SaveChanges();
            }

        }

        void IcountryRepo.UpdateCountry(CountryD country, int id)
        {
            var country1 = dataContext.Countries.FirstOrDefault(c => c.id == id);
            if (country1 != null)
            {
                country1.Name = country.Name;
                dataContext.SaveChanges();
            }

        }
    }
}
