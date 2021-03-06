using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Blip.Entities.Customers;
using Blip.Entities.Customers.ViewModels;

namespace Blip.Data
{
    public class CustomersRepository
    {
        public Customer GetCustomer(Guid customerid)
        {
            if (customerid != Guid.Empty)
            { 
                using (var context = new ApplicationDbContext())
                {
                    var customer = context.Customers.AsNoTracking()
                        .Where(x => x.CustomerID == customerid)
                        .Single();
                    return customer;
                }
            }
            return null;
        }

        public List<CustomerDisplayViewModel> GetCustomers()
        {
            using (var context = new ApplicationDbContext())
            {
                List<Customer> customers = new List<Customer>();
                customers = context.Customers.AsNoTracking()
                    .Include(x => x.Country)
                    .Include(x => x.Region)
                    .ToList();

                if (customers != null)
                {
                    List<CustomerDisplayViewModel> customersDisplay = new List<CustomerDisplayViewModel>();
                    foreach (var x in customers)
                    {
                        var customerDisplay = new CustomerDisplayViewModel()
                        {
                            CustomerID = x.CustomerID,
                            CustomerName = x.CustomerName,
                            CountryName = x.Country.CountryNameEnglish,
                            RegionName = x.Region.RegionNameEnglish
                        };
                        customersDisplay.Add(customerDisplay);
                    }
                    return customersDisplay;
                }
                return null;
            }
        }


        public CustomerEditViewModel CreateCustomer()
        {
            var cRepo = new CountriesRepository();
            var rRepo = new RegionsRepository();
            var customer = new CustomerEditViewModel()
            {
                CustomerID = Guid.NewGuid().ToString(),
                Countries = cRepo.GetCountries(),
                Regions = rRepo.GetRegions()
            };
            return customer;
        }

        public bool SaveCustomer(CustomerEditViewModel customeredit)
        {
            if (customeredit != null)
            {
                using (var context = new ApplicationDbContext())
                {
                    if (Guid.TryParse(customeredit.CustomerID, out Guid newGuid))
                    {
                        var customer = new Customer()
                        {
                            CustomerID = newGuid,
                            CustomerName = customeredit.CustomerName,
                            CountryIso3 = customeredit.SelectedCountryIso3,
                            RegionCode = customeredit.SelectedRegionCode
                        };
                        customer.Country = context.Countries.Find(customeredit.SelectedCountryIso3);
                        customer.Region = context.Regions.Find(customeredit.SelectedRegionCode);

                        context.Customers.Add(customer);
                        context.SaveChanges();
                        return true;
                    }
                }
            }
            // Return false if customeredit == null or CustomerID is not a guid
            return false;
        }
    }
}