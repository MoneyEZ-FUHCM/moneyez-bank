using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Repositories.Entities;
using MoneyEzBank.Repositories.Enums;
using MoneyEzBank.Repositories.UnitOfWork;
using MoneyEzBank.Repositories.Utils;
using MoneyEzBank.Services.BusinessModels;
using MoneyEzBank.Services.BusinessModels.TransactionModels;
using MoneyEzBank.Services.BusinessModels.WebhookModels;
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
        private readonly IWebhookService _webhookService;

        public TransactionService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IWebhookService webhookService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webhookService = webhookService;
        }

        private async Task NotifyBalanceChange(Account account, decimal oldBalance, decimal amount, TransactionType type, string transactionId, string description)
        {
            var payload = new WebhookPayload
            {
                AccountNumber = account.AccountNumber,
                OldBalance = oldBalance,
                NewBalance = account.Balance,
                Amount = amount,
                TransactionType = type,
                Timestamp = CommonUtils.GetCurrentTime(),
                TransactionId = transactionId,
                Description = description,
                BankName = "EZB"
            };

            await _webhookService.NotifyBalanceChangeAsync(payload);
        }

        public async Task<BaseResultModel> DepositAsync(CreateDepositModel model)
        {
            if (model.Amount <= 0)
                throw new DefaultException(MessageConstants.TRANSACTION_INVALID_AMOUNT_MESSAGE,
                    MessageConstants.TRANSACTION_INVALID_AMOUNT_CODE);

            var account = await _unitOfWork.AccountsRepository.GetByIdAsync(model.AccountId)
                ?? throw new NotExistException(MessageConstants.ACCOUNT_NOT_EXIST_MESSAGE,
                    MessageConstants.ACCOUNT_NOT_EXIST_CODE);

            var oldBalance = account.Balance;
            account.Balance += model.Amount;

            var transaction = new Transaction
            {
                SourceAccountId = model.AccountId,
                Amount = model.Amount,
                Type = TransactionType.Deposit,
                Description = model.Description ?? "Deposit",
                TransactionDate = CommonUtils.GetCurrentTime(),
                Status = TransactionStatus.SUCCESS.ToString()
            };

            await _unitOfWork.TransactionsRepository.AddAsync(transaction);
            _unitOfWork.AccountsRepository.UpdateAsync(account);
            await _unitOfWork.SaveAsync();

            // Notify after successful transaction
            await NotifyBalanceChange(account, oldBalance, model.Amount, TransactionType.Deposit, transaction.Id.ToString(), transaction.Description);

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Message = MessageConstants.TRANSACTION_DEPOSIT_SUCCESS_MESSAGE,
                Data = _mapper.Map<TransactionModel>(transaction)
            };
        }

        public async Task<BaseResultModel> WithdrawAsync(CreateWithdrawModel model)
        {
            if (model.Amount <= 0)
                throw new DefaultException(MessageConstants.TRANSACTION_INVALID_AMOUNT_MESSAGE,
                    MessageConstants.TRANSACTION_INVALID_AMOUNT_CODE);

            var account = await _unitOfWork.AccountsRepository.GetByIdAsync(model.AccountId)
                ?? throw new NotExistException(MessageConstants.ACCOUNT_NOT_EXIST_MESSAGE,
                    MessageConstants.ACCOUNT_NOT_EXIST_CODE);

            if (account.Balance < model.Amount)
                throw new DefaultException(MessageConstants.ACCOUNT_INSUFFICIENT_FUNDS_MESSAGE,
                    MessageConstants.ACCOUNT_INSUFFICIENT_FUNDS_CODE);

            var oldBalance = account.Balance;
            account.Balance -= model.Amount;

            var transaction = new Transaction
            {
                SourceAccountId = model.AccountId,
                Amount = model.Amount,
                Type = TransactionType.Withdrawal,
                Description = model.Description ?? "Withdrawal",
                TransactionDate = DateTime.UtcNow,
                Status = TransactionStatus.SUCCESS.ToString()
            };

            await _unitOfWork.TransactionsRepository.AddAsync(transaction);
            _unitOfWork.AccountsRepository.UpdateAsync(account);
            _unitOfWork.Save();

            // Notify after successful transaction
            await NotifyBalanceChange(account, oldBalance, model.Amount, TransactionType.Withdrawal, transaction.Id.ToString(), transaction.Description);

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Message = MessageConstants.TRANSACTION_WITHDRAWAL_SUCCESS_MESSAGE,
                Data = _mapper.Map<TransactionModel>(transaction)
            };
        }

        public async Task<BaseResultModel> TransferAsync(CreateTransferModel model)
        {
            if (model.Amount <= 0)
                throw new DefaultException(MessageConstants.TRANSACTION_INVALID_AMOUNT_MESSAGE,
                    MessageConstants.TRANSACTION_INVALID_AMOUNT_CODE);

            var sourceAccount = await _unitOfWork.AccountsRepository.GetByIdAsync(model.SourceAccountId)
                ?? throw new NotExistException(MessageConstants.ACCOUNT_NOT_EXIST_MESSAGE,
                    MessageConstants.ACCOUNT_NOT_EXIST_CODE);

            // check internal or external transfer
            if (model.DestinationBank == "EZB")
            {
                var destinationAccount = await _unitOfWork.AccountsRepository.GetByAccountNumberAsync(model.DestinationAccountNumber)
                    ?? throw new NotExistException(MessageConstants.ACCOUNT_DESTINATION_NOT_EXIST_MESSAGE,
                        MessageConstants.ACCOUNT_DESTINATION_NOT_EXIST_CODE);

                if (sourceAccount.Balance < model.Amount)
                    throw new DefaultException(MessageConstants.ACCOUNT_INSUFFICIENT_FUNDS_MESSAGE,
                        MessageConstants.ACCOUNT_INSUFFICIENT_FUNDS_CODE);

                var sourceOldBalance = sourceAccount.Balance;
                var destOldBalance = destinationAccount.Balance;

                sourceAccount.Balance -= model.Amount;
                destinationAccount.Balance += model.Amount;

                var transaction = new Transaction
                {
                    SourceAccountId = model.SourceAccountId,
                    DestinationAccountId = destinationAccount.Id,
                    Amount = model.Amount,
                    Type = TransactionType.Transfer,
                    Description = model.Description ?? "Transfer",
                    TransactionDate = CommonUtils.GetCurrentTime(),
                    Status = TransactionStatus.SUCCESS.ToString()
                };

                await _unitOfWork.TransactionsRepository.AddAsync(transaction);
                _unitOfWork.AccountsRepository.UpdateAsync(sourceAccount);
                _unitOfWork.AccountsRepository.UpdateAsync(destinationAccount);
                _unitOfWork.Save();

                // Notify both accounts after successful transaction
                await NotifyBalanceChange(sourceAccount, sourceOldBalance, model.Amount, TransactionType.Transfer, transaction.Id.ToString(), transaction.Description);
                await NotifyBalanceChange(destinationAccount, destOldBalance, model.Amount, TransactionType.Transfer, transaction.Id.ToString(), transaction.Description);

                return new BaseResultModel
                {
                    Status = StatusCodes.Status200OK,
                    Message = MessageConstants.TRANSACTION_TRANSFER_SUCCESS_MESSAGE,
                    Data = _mapper.Map<TransactionModel>(transaction)
                };
            } 
            else
            {
                throw new DefaultException("External transfer is not supported yet", 
                    MessageConstants.TRANSACTION_TRANSFER_EXTERNAL_NOT_SUPPORT_CODE);
            }

                
        }

        public async Task<BaseResultModel> GetTransactionsByAccountIdAsync(PaginationParameter paginationParameter, Guid accountId)
        {
            var account = await _unitOfWork.AccountsRepository.GetByIdAsync(accountId)
                ?? throw new NotExistException(MessageConstants.ACCOUNT_NOT_EXIST_MESSAGE,
                    MessageConstants.ACCOUNT_NOT_EXIST_CODE);

            var transactions = await _unitOfWork.TransactionsRepository.ToPaginationIncludeAsync(paginationParameter,
                    filter: t => t.DestinationAccountId == accountId || t.SourceAccountId == accountId,
                    include: t => t.Include(t => t.SourceAccount)
                        .Include(t => t.DestinationAccount),
                    orderBy: t => t.OrderByDescending(t => t.TransactionDate)
                );

            var transactionModels = _mapper.Map<List<TransactionModel>>(transactions);
            
            // Process transaction direction and add account information for each transaction
            foreach (var transaction in transactionModels)
            {
                // Set the transaction direction based on whether the account is source or destination
                if (transaction.Type == TransactionType.Transfer)
                {
                    // For transfers: negative for source account, positive for destination account
                    if (transaction.SourceAccountId == accountId)
                        transaction.TransactionDirection = -1 * transaction.Amount; // Outgoing transfer (negative)
                    else if (transaction.DestinationAccountId == accountId)
                        transaction.TransactionDirection = transaction.Amount; // Incoming transfer (positive)
                }
                else if (transaction.Type == TransactionType.Deposit)
                {
                    transaction.TransactionDirection = transaction.Amount; // Deposits are always positive
                }
                else if (transaction.Type == TransactionType.Withdrawal)
                {
                    transaction.TransactionDirection = -1 * transaction.Amount; // Withdrawals are always negative
                }
            }

            var result = PaginationHelper.GetPaginationResult(transactions, transactionModels);

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Data = result
            };
        }
    }
}
