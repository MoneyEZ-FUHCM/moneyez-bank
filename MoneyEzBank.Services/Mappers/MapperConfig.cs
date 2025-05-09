﻿using AutoMapper;
using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Repositories.Entities;
using MoneyEzBank.Services.BusinessModels.AccountModels;
using MoneyEzBank.Services.BusinessModels.TransactionModels;
using MoneyEzBank.Services.BusinessModels.UserModels;
using MoneyEzBank.Services.BusinessModels.WebhookModels;
using MoneyEzBank.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Services.Mappers
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            // user mapper
            CreateMap<User, UserModel>();
            CreateMap<Pagination<User>, Pagination<UserModel>>().ConvertUsing<PaginationConverter<User, UserModel>>();
            CreateMap<CreateUserModel, User>();

            // transaction mapper
            CreateMap<Transaction, TransactionModel>()
                .ForMember(dest => dest.SourceAccountHolder, otp => otp.MapFrom(src => src.SourceAccount.AccountHolder))
                .ForMember(dest => dest.SourceAccountNumber, otp => otp.MapFrom(src => src.SourceAccount.AccountNumber))
                .ForMember(dest => dest.DestinationAccountHolder, otp => otp.MapFrom(src => src.DestinationAccount.AccountHolder))
                .ForMember(dest => dest.DestinationAccountNumber, otp => otp.MapFrom(src => src.DestinationAccount.AccountNumber));
            CreateMap<CreateTransferModel, Transaction>();
            CreateMap<Pagination<Transaction>, Pagination<TransactionModel>>()
                .ConvertUsing<PaginationConverter<Transaction, TransactionModel>>();

            // account mapper
            CreateMap<Account, AccountModel>();
            CreateMap<Pagination<Account>, Pagination<AccountModel>>().ConvertUsing<PaginationConverter<Account, AccountModel>>();

            // webhook mapper
            CreateMap<WebhookConfig, WebhookConfigModel>()
                .ForMember(dest => dest.AccountHolder, otp => otp.MapFrom(src => src.Account.AccountHolder))
                .ForMember(dest => dest.AccountNumber, otp => otp.MapFrom(src => src.Account.AccountNumber));
            CreateMap<Pagination<WebhookConfig>, Pagination<WebhookConfigModel>>().ConvertUsing<PaginationConverter<WebhookConfig, WebhookConfigModel>>();
        }

        public class PaginationConverter<TSource, TDestination> : ITypeConverter<Pagination<TSource>, Pagination<TDestination>>
        {
            public Pagination<TDestination> Convert(Pagination<TSource> source, Pagination<TDestination> destination, ResolutionContext context)
            {
                var mappedItems = context.Mapper.Map<List<TDestination>>(source);
                return new Pagination<TDestination>(mappedItems, source.TotalCount, source.CurrentPage, source.PageSize);
            }
        }
    }
}
