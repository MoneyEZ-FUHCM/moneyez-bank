using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Services.Constants
{
    public class MessageContants
    {
        // syntax error code: <CONSTANT_NAME> = "MessagePascalCaseCode";
        // syntax message: <CONSTANT_NAME_MESSAGE> = "Message here";

        // authen
        public const string INVALID_EMAIL_PASSWORD = "InvalidEmailOrPassword";
        public const string TOKEN_NOT_VALID = "TokenNotValid";

        // user
        public const string USER_NOT_EXIST = "UserNotExist";
        public const string USER_EXISTED = "UserExisted";
        public const string USER_BLOCKED = "UserNotActive";
        public const string USER_DUPLICATE_PHONE_NUMBER = "DuplicatePhoneNumber";
    }
}
