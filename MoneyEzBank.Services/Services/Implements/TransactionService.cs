using AutoMapper;
using Microsoft.AspNetCore.Http;
using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Repositories.Entities;
using MoneyEzBank.Repositories.Enums;
using MoneyEzBank.Repositories.UnitOfWork;
using MoneyEzBank.Repositories.Utils;
using MoneyEzBank.Services.BusinessModels;
using MoneyEzBank.Services.BusinessModels.TransactionModels;
using MoneyEzBank.Services.Constants;
using MoneyEzBank.Services.Exceptions;
using MoneyEzBank.Services.Services.Interfaces;
using MoneyEzBank.Services.Utils;

namespace MoneyEzBank.Services.Services.Implements
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResultModel> DepositAsync(CreateTransactionModel model)
        {
            if (model.Amount <= 0)
                throw new DefaultException(MessageConstants.TRANSACTION_INVALID_AMOUNT_MESSAGE, 
                    MessageConstants.TRANSACTION_INVALID_AMOUNT_CODE);

            var account = await _unitOfWork.AccountsRepository.GetByIdAsync(model.SourceAccountId)
                ?? throw new NotExistException(MessageConstants.ACCOUNT_NOT_EXIST_MESSAGE, 
                    MessageConstants.ACCOUNT_NOT_EXIST_CODE);

            var transaction = new Transaction
            {
                SourceAccountId = model.SourceAccountId,
                Amount = model.Amount,
                Type = TransactionType.Deposit,
                Description = model.Description ?? "Deposit",
                TransactionDate = CommonUtils.GetCurrentTime(),
                Status = TransactionStatus.SUCCESS.ToString()
            };

            await _unitOfWork.TransactionsRepository.AddAsync(transaction);

            account.Balance += model.Amount;
            _unitOfWork.AccountsRepository.UpdateAsync(account);
            _unitOfWork.Save();

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Message = MessageConstants.TRANSACTION_DEPOSIT_SUCCESS_MESSAGE,
                Data = _mapper.Map<TransactionModel>(transaction)
            };
        }

        public async Task<BaseResultModel> WithdrawAsync(CreateTransactionModel model)
        {
            if (model.Amount <= 0)
                throw new DefaultException(MessageConstants.TRANSACTION_INVALID_AMOUNT_MESSAGE,
                    MessageConstants.TRANSACTION_INVALID_AMOUNT_CODE);

            var account = await _unitOfWork.AccountsRepository.GetByIdAsync(model.SourceAccountId)
                ?? throw new NotExistException(MessageConstants.ACCOUNT_NOT_EXIST_MESSAGE,
                    MessageConstants.ACCOUNT_NOT_EXIST_CODE);

            if (account.Balance < model.Amount)
                throw new DefaultException(MessageConstants.ACCOUNT_INSUFFICIENT_FUNDS_MESSAGE,
                    MessageConstants.ACCOUNT_INSUFFICIENT_FUNDS_CODE);

            var transaction = new Transaction
            {
                SourceAccountId = model.SourceAccountId,
                Amount = model.Amount,
                Type = TransactionType.Withdrawal,
                Description = model.Description ?? "Withdrawal",
                TransactionDate = DateTime.UtcNow,
                Status = TransactionStatus.SUCCESS.ToString()
            };

            await _unitOfWork.TransactionsRepository.AddAsync(transaction);
            account.Balance -= model.Amount;
            _unitOfWork.AccountsRepository.UpdateAsync(account);
            _unitOfWork.Save();

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Message = MessageConstants.TRANSACTION_WITHDRAWAL_SUCCESS_MESSAGE,
                Data = _mapper.Map<TransactionModel>(transaction)
            };
        }

        public async Task<BaseResultModel> TransferAsync(CreateTransactionModel model)
        {
            if (model.Amount <= 0)
                throw new DefaultException(MessageConstants.TRANSACTION_INVALID_AMOUNT_MESSAGE,
                    MessageConstants.TRANSACTION_INVALID_AMOUNT_CODE);

            if (model.DestinationAccountId == null)
                throw new DefaultException(MessageConstants.TRANSACTION_DESTINATION_REQUIRED_MESSAGE,
                    MessageConstants.TRANSACTION_DESTINATION_REQUIRED_CODE);

            if (model.SourceAccountId == model.DestinationAccountId)
                throw new DefaultException(MessageConstants.ACCOUNT_SAME_TRANSFER_MESSAGE,
                    MessageConstants.ACCOUNT_SAME_TRANSFER_CODE);

            var sourceAccount = await _unitOfWork.AccountsRepository.GetByIdAsync(model.SourceAccountId)
                ?? throw new NotExistException(MessageConstants.ACCOUNT_NOT_EXIST_MESSAGE,
                    MessageConstants.ACCOUNT_NOT_EXIST_CODE);

            var destinationAccount = await _unitOfWork.AccountsRepository.GetByIdAsync(model.DestinationAccountId.Value)
                ?? throw new NotExistException(MessageConstants.ACCOUNT_DESTINATION_NOT_EXIST_MESSAGE,
                    MessageConstants.ACCOUNT_DESTINATION_NOT_EXIST_CODE);

            if (sourceAccount.Balance < model.Amount)
                throw new DefaultException(MessageConstants.ACCOUNT_INSUFFICIENT_FUNDS_MESSAGE,
                    MessageConstants.ACCOUNT_INSUFFICIENT_FUNDS_CODE);

            var transaction = new Transaction
            {
                SourceAccountId = model.SourceAccountId,
                DestinationAccountId = model.DestinationAccountId,
                Amount = model.Amount,
                Type = TransactionType.Transfer,
                Description = model.Description ?? "Transfer",
                TransactionDate = CommonUtils.GetCurrentTime(),
                Status = TransactionStatus.SUCCESS.ToString()
            };

            await _unitOfWork.TransactionsRepository.AddAsync(transaction);
            sourceAccount.Balance -= model.Amount;
            destinationAccount.Balance += model.Amount;

            _unitOfWork.AccountsRepository.UpdateAsync(sourceAccount);
            _unitOfWork.AccountsRepository.UpdateAsync(destinationAccount);

            _unitOfWork.Save();

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Message = MessageConstants.TRANSACTION_TRANSFER_SUCCESS_MESSAGE,
                Data = _mapper.Map<TransactionModel>(transaction)
            };
        }

        public async Task<BaseResultModel> GetTransactionsByAccountIdAsync(PaginationParameter paginationParameter, Guid accountId)
        {
            var account = await _unitOfWork.AccountsRepository.GetByIdAsync(accountId)
                ?? throw new NotExistException(MessageConstants.ACCOUNT_NOT_EXIST_MESSAGE,
                    MessageConstants.ACCOUNT_NOT_EXIST_CODE);

            var transactions = await _unitOfWork.TransactionsRepository.ToPaginationIncludeAsync(paginationParameter, 
                    filter: t => t.DestinationAccountId == accountId || t.SourceAccountId == accountId);

            var transactionModels = _mapper.Map<List<TransactionModel>>(transactions);

            var result = PaginationHelper.GetPaginationResult(transactions, transactionModels);

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Data = result
            };
        }
    }
}
