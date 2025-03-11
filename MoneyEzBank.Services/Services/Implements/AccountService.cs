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

namespace MoneyEzBank.Services.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResultModel> GetByIdAsync(Guid id)
        {
            var account = await _unitOfWork.AccountsRepository.GetByIdAsync(id);
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
            var accounts = await _unitOfWork.AccountsRepository.GetByFilterAsync(paginationParameter, filter);

            var accountModels = _mapper.Map<List<AccountModel>>(accounts);

            var result = PaginationHelper.GetPaginationResult(accounts, accountModels);

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Data = result
            };
        }
    }
}
