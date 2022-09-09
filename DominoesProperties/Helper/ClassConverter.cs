using System;
using Models.Models;
using Helpers;
using System.Linq;

namespace DominoesProperties.Helper
{
    public class ClassConverter
    {
        internal static Models.CustomerReq ConvertCustomerToModel()
        {
            return null;
        }

        internal static Customer ConvertCustomerToEntity(Models.CustomerReq customer)
        {
            return new Customer
            {
                UniqueRef = Guid.NewGuid().ToString(),
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                Password = CommonLogic.Encrypt(customer.Password)
            };
        }

        internal static Models.Profile ConvertCustomerToProfile(Customer customer)
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

        internal static Admin UserToAdmin(Models.AdminUser admin)
        {
            return new Admin
            {
                Email = admin.Email,
                RoleFk = admin.RoleFk,
                Password = CommonLogic.Encrypt(admin.Password)
            };
        }

        internal static Property PropertyToEntity(Models.Properties property){
            return new Property
            {
                UniqueId = Guid.NewGuid().ToString(),
                Name = property.Name,
                ClosingDate = property.ClosingDate,
                CreatedBy = property.CreatedBy,
                DateCreated = DateTime.Now,
                Latitude = property.Latitude,
                Longitude = property.Longitude,
                Location = property.Location,
                TargetYield = property.TargetYield,
                UnitAvailable = property.UnitAvailable = property.TotalUnits - property.UnitSold,
                UnitPrice = property.UnitPrice,
                Status = property.Status,
                Type = property.Type,
                TotalUnits = property.TotalUnits,
                UnitSold = property.UnitSold,
                Bank = property.BankName,
                Account = property.AccountNumber,
                TotalPrice = property.TotalUnits * property.UnitPrice,
                ProjectedGrowth = property.TargetYield / 100 * property.TotalUnits * property.UnitPrice,
                Summary = property.Summary,
                VideoLink = property.VideoLink,
                AllowSharing = property.AllowSharing,
                MinimumSharingPercentage = property.MinimumSharingPercentage
            };
        }

        internal static Models.Properties EntityToProperty(Property property)
        {
            return new Models.Properties
            {
                UniqueId = property.UniqueId,
                Name = property.Name,
                ClosingDate = property.ClosingDate,
                CreatedBy = property.CreatedBy,
                DateCreated = DateTime.Now,
                InterestRate = property.InterestRate,
                Latitude = property.Latitude,
                Longitude = property.Longitude,
                Location = property.Location,
                ProjectedGrowth = property.ProjectedGrowth,
                UnitAvailable = property.UnitAvailable,
                UnitPrice = property.UnitPrice,
                Status = property.Status,
                TargetYield = property.TargetYield,
                Type = property.Type,
                TotalUnits = property.TotalUnits,
                UnitSold = property.UnitSold,
                TypeName = property.TypeNavigation.Name,
                Summary = property.Summary,
                VideoLink = property.VideoLink,
                AllowSharing = property.AllowSharing.Value,
                MinimumSharingPercentage = property.MinimumSharingPercentage.Value
            };
        }

        internal static Description DescriptionToEntity(Models.PropertyDescription description){
            return new Description{
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

        internal static Models.PropertyDescription ConvertDescription(Description description)
        {
            return new Models.PropertyDescription
            {
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

        internal static Models.Profile ConvertCustomerToFullProfile(Customer customer)
        {
            return new Models.Profile
            {
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
                WalletBalance = customer.Wallet.Balance.Value,
                Investments = customer.Investments.ToList(),
                BankName = customer.BankName
            };
        }

        internal static Models.InvestmentView ConvertInvestmentForView(Investment inv)
        {
            return new Models.InvestmentView
            {
                Id = inv.Id,
                CustomerId = inv.CustomerId,
                PropertyId = inv.PropertyId,
                Units = inv.Units,
                PaymentDate = inv.PaymentDate,
                Amount = inv.Amount,
                Yield = inv.Yield,
                YearlyInterestAmount = inv.YearlyInterestAmount,
                TransactionRef = inv.TransactionRef,
                Status = inv.Status
    };
        }
    }
}
