using AutoMapper;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
using System.IdentityModel.Tokens.Jwt;
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
        public async Task<BaseResultModel> BanUserAsync(Guid id, string currentEmail)
        {
            var existUser = await _unitOfWork.UsersRepository.GetByIdAsync(id);
            if (existUser != null)
            {
                // check current user
                if (existUser.Email == currentEmail)
                {
                    throw new DefaultException("Account is current user, can not ban", MessageConstants.USER_IS_CURRENT_CODE);
                }

                _unitOfWork.UsersRepository.UpdateAsync(existUser);
                _unitOfWork.Save();

                return new BaseResultModel
                {
                    Status = StatusCodes.Status200OK,
                    Data = _mapper.Map<UserModel>(existUser),
                    Message = "User has banned",
                };
            }
            else
            {
                throw new NotExistException("", MessageConstants.USER_BANNED_CODE);
            }
        }

        public async Task<BaseResultModel> CreateUserAsync(CreateUserModel model)
        {
            User newUser = _mapper.Map<User>(model);
            newUser.UnsignFullName = StringUtils.ConvertToUnSign(model.FullName);
            newUser.Role = model.Role;

            var existUser = await _unitOfWork.UsersRepository.GetUserByEmailAsync(model.Email);

            if (existUser != null)
            {
                throw new DefaultException("", "User has already exist");
            }

            if (CheckExistPhone(model.PhoneNumber).Result)
            {
                throw new DefaultException("", MessageConstants.USER_DUPLICATE_PHONE_NUMBER_CODE);
            }

            // generate password
            string password = PasswordUtils.GeneratePassword();

            // hash password
            newUser.PasswordHash = PasswordUtils.HashPassword(password);

            await _unitOfWork.UsersRepository.AddAsync(newUser);
            _unitOfWork.Save();

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Data = _mapper.Map<UserModel>(newUser),
                Message = "User created"
            };
        }

        public async Task<BaseResultModel> DeleteUserAsync(Guid id, string currentEmail)
        {
            var existUser = await _unitOfWork.UsersRepository.GetByIdAsync(id);
            if (existUser != null)
            {
                // check current user
                if (existUser.Email == currentEmail)
                {
                    throw new DefaultException("User is current user, can not delete", MessageConstants.USER_IS_CURRENT_CODE);
                }

                    _unitOfWork.UsersRepository.PermanentDeletedAsync(existUser);
                    _unitOfWork.Save();

                    return new BaseResultModel
                    {
                        Status = StatusCodes.Status200OK,
                        Data = _mapper.Map<UserModel>(existUser),
                        Message = "User deleted",
                    };
            }
            else
            {
                throw new NotExistException("", MessageConstants.USER_NOT_EXIST_CODE);
            }
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
                throw new NotExistException("", MessageConstants.USER_NOT_EXIST_CODE);
            }
        }

        public async Task<BaseResultModel> GetUserByIdAsync(Guid id)
        {
            var user = await _unitOfWork.UsersRepository.GetByIdAsync(id);
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
                throw new NotExistException("", MessageConstants.USER_NOT_EXIST_CODE);
            }
        }

        public async Task<BaseResultModel> GetUserPaginationAsync(PaginationParameter paginationParameter, UserFilter userFilter)
        {
            var users = await _unitOfWork.UsersRepository.GetUsersByFilter(paginationParameter, userFilter);

            var userModels = _mapper.Map<List<UserModel>>(users);
            var paginatedResult = PaginationHelper.GetPaginationResult(users, userModels);

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Data = paginatedResult
            };
        }

        public async Task<BaseResultModel> LoginWithEmailPassword(string email, string password)
        {
            var existUser = await _unitOfWork.UsersRepository.GetUserByEmailAsync(email);
            if (existUser == null)
            {
                return new BaseResultModel
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ErrorCode = MessageConstants.INVALID_EMAIL_PASSWORD_CODE
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
                        ErrorCode = MessageConstants.USER_BANNED_CODE
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
                ErrorCode = MessageConstants.INVALID_EMAIL_PASSWORD_CODE,
            };
        }

        public async Task<BaseResultModel> RefreshToken(string jwtToken)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = authSigningKey,
                ValidateIssuer = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                SecurityToken validatedToken;
                var principal = handler.ValidateToken(jwtToken, validationParameters, out validatedToken);
                var email = principal.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
                if (email != null)
                {
                    var existUser = await _unitOfWork.UsersRepository.GetUserByEmailAsync(email);
                    if (existUser != null)
                    {
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
                            Message = "Token refresh successfully"
                        };
                    }
                }
                return new BaseResultModel
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ErrorCode = MessageConstants.USER_NOT_EXIST_CODE
                };
            }
            catch
            {
                return new BaseResultModel
                {
                    Status = StatusCodes.Status401Unauthorized,
                    ErrorCode = MessageConstants.TOKEN_NOT_VALID_CODE
                };
            }
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
                throw new DefaultException("", MessageConstants.USER_EXISTED_CODE);
            }

            if (CheckExistPhone(model.PhoneNumber).Result)
            {
                throw new DefaultException("", MessageConstants.USER_DUPLICATE_PHONE_NUMBER_CODE);
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

        public async Task<BaseResultModel> UpdateUserAsync(UpdateUserModel model)
        {
            var existUser = await _unitOfWork.UsersRepository.GetByIdAsync(model.Id);

            if (existUser == null)
            {
                throw new NotExistException("", MessageConstants.USER_NOT_EXIST_CODE);
            }

            // check duplicate phone number
            if (model.PhoneNumber != existUser.PhoneNumber && CheckExistPhone(model.PhoneNumber).Result)
            {
                throw new DefaultException("", MessageConstants.USER_DUPLICATE_PHONE_NUMBER_CODE);
            }

            existUser.FullName = model.FullName;
            existUser.UnsignFullName = StringUtils.ConvertToUnSign(model.FullName);
            existUser.PhoneNumber = model.PhoneNumber;
            //if (model.Avatar != null)
            //{
            //    existUser.AvatarUrl = model.Avatar;
            //}

            _unitOfWork.UsersRepository.UpdateAsync(existUser);
            _unitOfWork.Save();

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Message = "User updated",
                Data = _mapper.Map<UserModel>(existUser)
            };
        }

        private async Task<bool> CheckExistPhone(string phoneNumber)
        {
            var users = await _unitOfWork.UsersRepository.GetUserByPhoneAsync(phoneNumber);
            return users != null;
        }
    }
}
