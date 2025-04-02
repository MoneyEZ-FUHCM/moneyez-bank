using MoneyEzBank.Repositories.Entities;
using MoneyEzBank.Repositories.Repositories.Interfaces;
using MoneyEzBank.Repositories.UnitOfWork;
using MoneyEzBank.Services.BusinessModels.AccountModels;
using MoneyEzBank.Services.Constants;
using MoneyEzBank.Services.Exceptions;
using MoneyEzBank.Services.Services.Interfaces;
using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Repositories.Utils;
using Microsoft.AspNetCore.Http;
using MoneyEzBank.Repositories.Commons.Filter;
using MoneyEzBank.Services.BusinessModels;
using AutoMapper;
using MoneyEzBank.Services.Utils;
using System;
using MoneyEzBank.Repositories.Enums;
using Microsoft.EntityFrameworkCore;

namespace MoneyEzBank.Services.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsService;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claimsService = claimsService;
        }

        public async Task<BaseResultModel> GetByIdAsync(Guid id)
        {
            var account = await _unitOfWork.AccountsRepository.GetByIdIncludeAsync(id, include: a => a.Include(a => a.User));
            if (account == null)
                throw new NotExistException($"Account with ID {id} not found", MessageConstants.ACCOUNT_NOT_EXIST_CODE);

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Data = _mapper.Map<AccountModel>(account)
            };
        }

        public async Task<BaseResultModel> GetByAccountNumberAsync(string accountNumber)
        {
            var account = await _unitOfWork.AccountsRepository.GetByAccountNumberAsync(accountNumber);
            if (account == null)
                throw new NotExistException($"Account number {accountNumber} not found", MessageConstants.ACCOUNT_NOT_EXIST_CODE);

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Data = _mapper.Map<AccountModel>(account)
            };
        }

        public async Task<BaseResultModel> CreateAccountAsync(CreateAccountModel model)
        {
            // Validate user exists
            var user = await _unitOfWork.UsersRepository.GetByIdAsync(model.UserId);
            if (user == null)
                throw new NotExistException($"User with ID {model.UserId} not found", MessageConstants.USER_NOT_EXIST_CODE);

            // Check if account number is unique
            var existingAccount = await _unitOfWork.AccountsRepository.GetByAccountNumberAsync(model.AccountNumber);
            if (existingAccount != null)
                throw new DefaultException("Account number already exists", MessageConstants.ACCOUNT_DUPLICATE_NUMBER_CODE);

            // Validate initial balance (assuming minimum balance requirement)
            if (model.InitialBalance < 0)
                throw new DefaultException("Initial balance cannot be negative", MessageConstants.ACCOUNT_INVALID_BALANCE_CODE);

            var account = new Account
            {
                AccountNumber = model.AccountNumber,
                Balance = model.InitialBalance,
                UserId = model.UserId
            };

            await _unitOfWork.AccountsRepository.AddAsync(account);
            await _unitOfWork.SaveAsync();

            return new BaseResultModel
            {
                Status = StatusCodes.Status201Created,
                Message = MessageConstants.ACCOUNT_CREATE_SUCCESS_MESSAGE,
                Data = _mapper.Map<AccountModel>(account)
            };
        }

        public async Task<BaseResultModel> UpdateAccountAsync(UpdateAccountModel model)
        {
            var account = await _unitOfWork.AccountsRepository.GetByIdAsync(model.Id);
            if (account == null)
                throw new NotExistException($"Account with ID {model.Id} not found", MessageConstants.ACCOUNT_NOT_EXIST_CODE);

            // Validate user exists
            var user = await _unitOfWork.UsersRepository.GetByIdAsync(model.UserId);
            if (user == null)
                throw new NotExistException($"User with ID {model.UserId} not found", MessageConstants.USER_NOT_EXIST_CODE);

            // Check if new account number is unique (if it's different from current)
            if (account.AccountNumber != model.AccountNumber)
            {
                var existingAccount = await _unitOfWork.AccountsRepository.GetByAccountNumberAsync(model.AccountNumber);
                if (existingAccount != null)
                    throw new DefaultException("Account number already exists", MessageConstants.ACCOUNT_DUPLICATE_NUMBER_CODE);
            }

            account.AccountNumber = model.AccountNumber;
            account.UserId = model.UserId;

            _unitOfWork.AccountsRepository.UpdateAsync(account);
            await _unitOfWork.SaveAsync();

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Data = _mapper.Map<AccountModel>(account),
                Message = MessageConstants.ACCOUNT_UPDATE_SUCCESS_MESSAGE
            };
        }

        public async Task<BaseResultModel> GetAccountsByFilterAsync(PaginationParameter paginationParameter, AccountFilter filter)
        {
            var user = await _unitOfWork.UsersRepository.GetUserByEmailAsync(_claimsService.GetCurrentUserEmail);
            if (user == null)
            {
                throw new NotExistException($"User not found", MessageConstants.USER_NOT_EXIST_CODE);
            }

            var accounts = new Pagination<Account>();

            if (user.Role == Roles.ADMIN)
            {
                accounts = await _unitOfWork.AccountsRepository.GetByFilterAsync(paginationParameter, filter);
            }
            else
            {
                filter.UserId = user.Id;
                accounts = await _unitOfWork.AccountsRepository.GetByFilterAsync(paginationParameter, filter);
            }

            var accountModels = _mapper.Map<List<AccountModel>>(accounts);

            var result = PaginationHelper.GetPaginationResult(accounts, accountModels);

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Data = result
            };
        }

        public async Task<BaseResultModel> CreateAccountUserAsync()
        {
            // Validate user exists
            var user = await _unitOfWork.UsersRepository.GetUserByEmailAsync(_claimsService.GetCurrentUserEmail);
            if (user == null)
                throw new NotExistException($"User not found", MessageConstants.USER_NOT_EXIST_CODE);

            string newAccountNumber = GenerateRandomNumber(12);

            // Check if account number is unique
            var existingAccount = await _unitOfWork.AccountsRepository.GetByAccountNumberAsync(newAccountNumber);
            if (existingAccount == null)
            {
                var account = new Account
                {
                    AccountNumber = newAccountNumber,
                    AccountHolder = StringUtils.ConvertToUnSign(user.FullName).ToUpper(),
                    Balance = 0,
                    UserId = user.Id
                };

                await _unitOfWork.AccountsRepository.AddAsync(account);
                await _unitOfWork.SaveAsync();

                return new BaseResultModel
                {
                    Status = StatusCodes.Status201Created,
                    Message = MessageConstants.ACCOUNT_CREATE_SUCCESS_MESSAGE,
                    Data = _mapper.Map<AccountModel>(account)
                };
            }

            throw new DefaultException("", MessageConstants.ACCOUNT_CREATE_FAIL_MESSAGE_CODE);

        }

        private static string GenerateRandomNumber(int digitCount)
        {
            Random random = new Random();
            string result = ((char)('1' + random.Next(9))).ToString();
            for (int i = 1; i < digitCount; i++)
            {
                result += random.Next(10).ToString();
            }

            return result;
        }
    }
}
