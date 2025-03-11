using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Services.Constants
{
    public class MessageConstants
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
    }
}
