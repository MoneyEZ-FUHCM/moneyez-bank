using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Services.Constants
{
    public static class MessageConstants
    {
        // syntax error code: <CONSTANT_NAME> = "MessagePascalCaseCode";
        // syntax message: <CONSTANT_NAME_MESSAGE> = "Message here";

        // authen
        public const string INVALID_EMAIL_PASSWORD_CODE = "InvalidEmailOrPassword";
        public const string TOKEN_NOT_VALID_CODE = "TokenNotValid";

        // user
        public const string USER_NOT_EXIST_CODE = "UserNotExist";
        public const string USER_IS_CURRENT_CODE = "UserIsCurrent";
        public const string USER_EXISTED_CODE = "UserExisted";
        public const string USER_BANNED_CODE = "UserHasBanned";
        public const string USER_DUPLICATE_PHONE_NUMBER_CODE = "DuplicatePhoneNumber";

        // account
        public const string ACCOUNT_NOT_EXIST_CODE = "AccountNotExist";
        public const string ACCOUNT_NOT_EXIST_MESSAGE = "Account not found";
        public const string ACCOUNT_INSUFFICIENT_FUNDS_CODE = "InsufficientFunds";
        public const string ACCOUNT_INSUFFICIENT_FUNDS_MESSAGE = "Insufficient funds for this operation";
        public const string ACCOUNT_SAME_TRANSFER_CODE = "SameAccountTransfer";
        public const string ACCOUNT_SAME_TRANSFER_MESSAGE = "Cannot transfer to the same account";
        public const string ACCOUNT_DESTINATION_NOT_EXIST_CODE = "DestinationAccountNotExist";
        public const string ACCOUNT_DESTINATION_NOT_EXIST_MESSAGE = "Destination account not found";
        public const string ACCOUNT_DUPLICATE_NUMBER_CODE = "AccountNumberDupplicate";
        public const string ACCOUNT_INVALID_BALANCE_CODE = "InitialBalanceCannotBeNegative";
        public const string ACCOUNT_CREATE_SUCCESS_MESSAGE = "Account created successfully";
        public const string ACCOUNT_UPDATE_SUCCESS_MESSAGE = "Account updated successfully";
        public const string ACCOUNT_CREATE_FAIL_MESSAGE_CODE = "AccountCreatedFailed";
        public const string ACCOUNT_MISMATCH_ACCOUNT_HOLDER = "AccountMismatchAccountHolder";

        // transaction
        public const string TRANSACTION_INVALID_AMOUNT_CODE = "InvalidTransactionAmount";
        public const string TRANSACTION_INVALID_AMOUNT_MESSAGE = "Transaction amount must be positive";
        public const string TRANSACTION_DESTINATION_REQUIRED_CODE = "DestinationAccountRequired";
        public const string TRANSACTION_DESTINATION_REQUIRED_MESSAGE = "Destination account is required for transfer";
        public const string TRANSACTION_DEPOSIT_SUCCESS_MESSAGE = "Deposit completed successfully";
        public const string TRANSACTION_WITHDRAWAL_SUCCESS_MESSAGE = "Withdrawal completed successfully";
        public const string TRANSACTION_TRANSFER_SUCCESS_MESSAGE = "Transfer completed successfully";
        public const string TRANSACTION_TRANSFER_EXTERNAL_NOT_SUPPORT_CODE = "ExternalTransferNotSupport";

        // webhook
        public const string WEBHOOK_NOT_EXIST_CODE = "WebhookNotExist";
    }
}
