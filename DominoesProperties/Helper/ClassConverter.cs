using System;
using Models.Models;

namespace DominoesProperties.Helper
{
    public class ClassConverter
    {
        public static Models.Customer ConvertCustomerToModel()
        {
            return null;
        }

        public static Customer ConvertCustomerToEntity(Models.Customer customer)
        {
            return new Customer
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                Password = CommonLogic.Encrypt(customer.Password)
            };
        }

        public static Models.Profile ConvertCustomerToProfile(Customer customer)
        {
            return new Models.Profile {
                UniqueReference = customer.UniqueRef,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Address = customer.Address,
                Email = customer.Email,
                IsAccountVerified = customer.IsAccountVerified,
                AccountNumber = customer.AccountNumber,
                IsActive = customer.IsActive,
                IsSubscribed = customer.IsSubscribed,
                IsVerified = customer.IsVerified,
                Phone = customer.Phone,
                WalletId = customer.Wallet.WalletNo,
                WalletBalance = customer.Wallet.Balance.Value
            };
        }

        public static Property PropertyToEntity(Models.Properties property){
            return new Property
            {
                UniqueId = Guid.NewGuid().ToString(),
                Name = property.Name,
                Description = DescriptionToEntity(property.Description).Id,
                ClosingDate = property.ClosingDate,
                CreatedBy = property.CreatedBy,
                DateCreated = DateTime.Now,
                InterestRate = property.InterestRate,
                Latitude = property.Latitude,
                Longitude = property.Longitude,
                Location = property.Location,
                ProjectedGrowth = property.ProjectedGrowth,
                UnitAvailable = property.TotalUnits - property.UnitSold,
                UnitPrice = property.UnitPrice,
                Status = property.Status.ToString(),
                TargetYield = property.TargetYield,
                Type = property.Type,
                TotalUnits = property.TotalUnits,
                UnitSold = property.UnitSold
            };
        }

        public static Description DescriptionToEntity(Models.PropertyDescription description){
            return new Description{
                PropertyId = description.PropertyId,
                Bathroom = description.Bathroom,
                Toilet = description.Toilet,
                AirConditioned = description.AirConditioned,
                Basement = description.Basement,
                Bedroom = description.Bedroom,
                Fireplace = description.Fireplace,
                FloorLevel = description.FloorLevel,
                Gym = description.Gym,
                LandSize = description.LandSize,
                Laundry = description.Laundry,
                Parking = description.Parking,
                Refrigerator = description.Refrigerator,
                SecurityGuard = description.SecurityGuard,
                SwimmingPool = description.SwimmingPool
            };
        }
    }
}
