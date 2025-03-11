using AutoMapper;
using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Repositories.Entities;
using MoneyEzBank.Services.BusinessModels.AccountModels;
using MoneyEzBank.Services.BusinessModels.TransactionModels;
using MoneyEzBank.Services.BusinessModels.UserModels;
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

            // transaction mapper
            CreateMap<Transaction, TransactionModel>();
            CreateMap<CreateTransactionModel, Transaction>();
            CreateMap<Pagination<Transaction>, Pagination<TransactionModel>>()
                .ConvertUsing<PaginationConverter<Transaction, TransactionModel>>();

            // account mapper
            CreateMap<Account, AccountModel>();
            CreateMap<Pagination<Account>, Pagination<AccountModel>>().ConvertUsing<PaginationConverter<Account, AccountModel>>();
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
