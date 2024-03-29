﻿using System;
using System.Linq;
using DominoesProperties.Enums;
using DominoesProperties.Models;
using Helpers;
using Models.Models;

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
                PassportUrl = customer.PassportUrl
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

        internal static Property PropertyToEntity(Models.Properties property)
        {
            var prop = new Property
            {
                UniqueId = Guid.NewGuid().ToString(),
                Name = property.Name,
                ClosingDate = property.ClosingDate,
                CreatedBy = property.CreatedBy,
                DateCreated = DateTime.Now,
                Latitude = property.Latitude,
                Longitude = property.Longitude,
                Location = property.Location,
                UnitAvailable = property.UnitAvailable = property.TotalUnits - property.UnitSold,
                UnitPrice = property.UnitPrice,
                Status = ((PropertyStatus)int.Parse(property.Status)).ToString(),
                TotalUnits = property.TotalUnits,
                UnitSold = property.UnitSold,
                Bank = property.BankName,
                Account = property.AccountNumber,
                TotalPrice = property.TotalUnits * property.UnitPrice,
                ProjectedGrowth = property.TargetYield!.Value / 100 * property.TotalUnits * property.UnitPrice,
                Summary = property.Summary,
                VideoLink = property.VideoLink,
                AllowSharing = property.AllowSharing,
                MinimumSharingPercentage = property.MinimumSharingPercentage
            };
            if (property.TargetYield != null)
                prop.TargetYield = property.TargetYield.Value;
            if (property.Type != null)
                prop.Type = property.Type.Value;

            return prop;
        }

        internal static Models.Properties EntityToProperty(Property property)
        {
            var prop = new Models.Properties
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
                Status = ((int)Enum.Parse(typeof(PropertyStatus), property.Status)).ToString(),
                TargetYield = property.TargetYield,
                Type = property.Type,
                TotalUnits = property.TotalUnits,
                UnitSold = property.UnitSold,
                TypeName = property.TypeNavigation.Name,
                Summary = property.Summary,
                VideoLink = property.VideoLink,
                AllowSharing = property.AllowSharing.GetValueOrDefault(),
                MinimumSharingPercentage = property.MinimumSharingPercentage.GetValueOrDefault(),
                AccountNumber = property.Account,
                BankName = property.Bank
            };

            if (property.PropertyUploads != null)
            {
                prop.Data = property.PropertyUploads;
            }

            return prop;
        }

        internal static Description DescriptionToEntity(Models.PropertyDescription description)
        {
            return new Description
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
                BankName = customer.BankName,
                PassportUrl = customer.PassportUrl
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
                Status = inv.Status,
                PaymentType = inv.PaymentType,
                UnitPrice = inv.UnitPrice,
                Customer = $"{inv.Customer.FirstName} {inv.Customer.LastName}",
                Property = inv.Property.Name
            };
        }

        internal static BlogModel GetBlogModelFromBlogPost(Blogpost blogPost)
        {
            BlogModel model = new();
            model.BlogTitle = blogPost.BlogTitle;
            model.UniqueNumber = blogPost.UniqueNumber;
            model.BlogContent = blogPost.BlogContent;
            if (blogPost.BlogTags != null)
                model.BlogTags = blogPost.BlogTags;

            if (blogPost.BlogImage != null)
                model.BlogImage = blogPost.BlogImage;

            model.CreatedOn = blogPost.CreatedOn.GetValueOrDefault();
            model.CreatedBy = blogPost.CreatedBy;

            return model;
        }

        internal static Blogpost GetBlogPostFromModel(BlogModel blogPost)
        {
            Blogpost model = new()
            {
                BlogTitle = blogPost.BlogTitle,
                BlogContent = blogPost.BlogContent,
                UniqueNumber = string.IsNullOrEmpty(blogPost.UniqueNumber)
                    ? CommonLogic.GetUniqueNumber("BP")
                    : blogPost.UniqueNumber
            };

            if (blogPost.BlogTags != null)
                model.BlogTags = blogPost.BlogTags;

            if (blogPost.BlogImage != null)
                model.BlogImage = blogPost.BlogImage;

            model.CreatedOn = DateTime.Now;
            model.CreatedBy = blogPost.CreatedBy;
            return model;
        }

        internal static CustomerReq ThirdPartyCustomerToCustomer(AffiliateCustomer customer)
        {
            const string password = "@Dominoes3Party!";
            return new CustomerReq
            {
                Email = customer.Email,
                Phone = customer.Phone,
                LastName = customer.LastName,
                FirstName = customer.FirstName,
                Password = password,
                ConfirmPassword = password
            };
        }
    }
}