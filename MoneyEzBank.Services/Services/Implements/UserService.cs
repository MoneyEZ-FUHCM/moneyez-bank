using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Repositories.Commons.Filter;
using MoneyEzBank.Repositories.Entities;
using MoneyEzBank.Repositories.Enums;
using MoneyEzBank.Repositories.UnitOfWork;
using MoneyEzBank.Services.BusinessModels;
using MoneyEzBank.Services.BusinessModels.AuthenModels;
using MoneyEzBank.Services.BusinessModels.UserModels;
using MoneyEzBank.Services.Constants;
using MoneyEzBank.Services.Exceptions;
using MoneyEzBank.Services.Services.Interfaces;
using MoneyEzBank.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyEzBank.Services.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
        }
        public Task<BaseResultModel> BanUserAsync(Guid id, string currentEmail)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResultModel> DeleteUserAsync(Guid id, string currentEmail)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResultModel> GetCurrentUser(string email)
        {
            var user = await _unitOfWork.UsersRepository.GetUserByEmailAsync(email);
            if (user != null)
            {
                return new BaseResultModel
                {
                    Status = StatusCodes.Status200OK,
                    Data = _mapper.Map<UserModel>(user)
                };
            }
            else
            {
                throw new NotExistException("", MessageConstants.USER_NOT_EXIST);
            }
        }

        public Task<BaseResultModel> GetUserByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResultModel> GetUserPaginationAsync(PaginationParameter paginationParameter, UserFilter userFilter)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResultModel> LoginWithEmailPassword(string email, string password)
        {
            var existUser = await _unitOfWork.UsersRepository.GetUserByEmailAsync(email);
            if (existUser == null)
            {
                return new BaseResultModel
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ErrorCode = MessageConstants.INVALID_EMAIL_PASSWORD
                };
            }

            var verifyUser = PasswordUtils.VerifyPassword(password, existUser.PasswordHash);

            if (verifyUser)
            {
                // check status user
                if (existUser.IsDeleted == true)
                {
                    return new BaseResultModel
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        ErrorCode = MessageConstants.USER_BLOCKED
                    };
                }

                var accessToken = AuthenTokenUtils.GenerateAccessToken(email, existUser, _configuration);
                var refreshToken = AuthenTokenUtils.GenerateRefreshToken(email, _configuration);

                return new BaseResultModel
                {
                    Status = StatusCodes.Status200OK,
                    Data = new AuthenModel
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    },
                    Message = "Login successfully"
                };
            }
            return new BaseResultModel
            {
                Status = StatusCodes.Status401Unauthorized,
                ErrorCode = MessageConstants.INVALID_EMAIL_PASSWORD,
            };
        }

        public Task<BaseResultModel> RefreshToken(string jwtToken)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResultModel> RegisterAsync(SignUpModel model)
        {
            User newUser = new User
            {
                Email = model.Email,
                FullName = model.FullName,
                UnsignFullName = StringUtils.ConvertToUnSign(model.FullName),
                PhoneNumber = model.PhoneNumber,
                Role = Roles.USER
            };

            var existUser = await _unitOfWork.UsersRepository.GetUserByEmailAsync(model.Email);

            if (existUser != null)
            {
                throw new DefaultException("", MessageConstants.USER_EXISTED);
            }

            if (CheckExistPhone(model.PhoneNumber).Result)
            {
                throw new DefaultException("", MessageConstants.USER_DUPLICATE_PHONE_NUMBER);
            }

            // hash password
            newUser.PasswordHash = PasswordUtils.HashPassword(model.Password);

            await _unitOfWork.UsersRepository.AddAsync(newUser);

            _unitOfWork.Save();
            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Message = "Your account is ready. Try to login.",
            };
        }

        private async Task<bool> CheckExistPhone(string phoneNumber)
        {
            var users = await _unitOfWork.UsersRepository.GetUserByPhoneAsync(phoneNumber);
            return users != null;
        }
    }
}
