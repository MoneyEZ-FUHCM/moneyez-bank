using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoneyEzBank.Repositories.Commons;
using MoneyEzBank.Repositories.Entities;
using MoneyEzBank.Repositories.Enums;
using MoneyEzBank.Repositories.UnitOfWork;
using MoneyEzBank.Repositories.Utils;
using MoneyEzBank.Services.BusinessModels;
using MoneyEzBank.Services.BusinessModels.WebhookModels;
using MoneyEzBank.Services.Constants;
using MoneyEzBank.Services.Exceptions;
using MoneyEzBank.Services.Services.Interfaces;

namespace MoneyEzBank.Services.Services.Implements
{
    public class WebhookService : IWebhookService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public WebhookService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _httpClient = httpClientFactory.CreateClient("WebhookClient");
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task NotifyBalanceChangeAsync(WebhookPayload payload)
        {
            var account = await _unitOfWork.AccountsRepository.GetByAccountNumberAsync(payload.AccountNumber);
            var webhooks = await _unitOfWork.WebhookConfigRepository
                .GetByConditionAsync(
                filter: w => 
                    w.Type == WebhookType.TransactionNotification && 
                    w.IsEnabled && w.AccountId == account.Id);

            foreach (var webhook in webhooks)
            {
                await TriggerWebhookAsync(webhook, payload);
            }
        }

        private async Task TriggerWebhookAsync(WebhookConfig webhook, WebhookPayload payload)
        {
            try
            {
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, webhook.ContentType);

                // Clear and set new headers
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Webhook-Secret", webhook.Secret);

                var response = await _httpClient.PostAsync(webhook.Url, content);
                response.EnsureSuccessStatusCode();

                // Update success metrics
                webhook.LastTriggeredAt = CommonUtils.GetCurrentTime();
                webhook.FailureCount = 0;
                
                _unitOfWork.WebhookConfigRepository.UpdateAsync(webhook);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception)
            {
                // Update failure metrics
                webhook.LastFailureAt = CommonUtils.GetCurrentTime();
                webhook.FailureCount++;

                // Disable webhook if it exceeds max retries
                if (webhook.FailureCount >= webhook.MaxRetries)
                {
                    webhook.IsEnabled = false;
                }

                _unitOfWork.WebhookConfigRepository.UpdateAsync(webhook);
                await _unitOfWork.SaveAsync();

                // TODO: Implement retry mechanism using RetryIntervalSeconds
                // Could use a background job service like Hangfire or Quartz.NET
            }
        }

        public async Task<BaseResultModel> RegisterWebhookAsync(WebhookRequestModel model)
        {

            var account = await _unitOfWork.AccountsRepository.GetByAccountNumberAsync(model.AccountNumber);
            if (account == null)
            {
                throw new NotExistException("", MessageConstants.ACCOUNT_NOT_EXIST_CODE);
            }

            if (account.AccountHolder != model.AccountHolder)
            {
                throw new DefaultException("", MessageConstants.ACCOUNT_MISMATCH_ACCOUNT_HOLDER);
            }

            var webhook = new WebhookConfig
            {
                Url = model.Url,
                Type = WebhookType.TransactionNotification,
                Secret = model.Secret,
                AccountId = account.Id,
                MaxRetries = 3,
                RetryIntervalSeconds = 30,
                ContentType = "application/json",
            };

            await _unitOfWork.WebhookConfigRepository.AddAsync(webhook);
            await _unitOfWork.SaveAsync();

            return new BaseResultModel
            {
                Status = StatusCodes.Status201Created,
                Message = "Webhook registered successfully"
            };
        }

        public async Task<BaseResultModel> GetWebhookByIdAsync(Guid id)
        {
            var webhook = await _unitOfWork.WebhookConfigRepository.GetByIdAsync(id);
            if (webhook == null || webhook.IsDeleted)
            {
                throw new NotExistException("", MessageConstants.WEBHOOK_NOT_EXIST_CODE);
            }

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Data = webhook
            };
        }

        public async Task<BaseResultModel> UpdateWebhookAsync(Guid id, WebhookRequestModel model)
        {
            var webhook = await _unitOfWork.WebhookConfigRepository.GetByIdAsync(id);
            if (webhook == null || webhook.IsDeleted)
            {
                throw new NotExistException("", MessageConstants.WEBHOOK_NOT_EXIST_CODE);
            }

            webhook.Url = model.Url;
            webhook.Secret = model.Secret;

            _unitOfWork.WebhookConfigRepository.UpdateAsync(webhook);
            await _unitOfWork.SaveAsync();

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Message = "Webhook updated successfully",
                Data = webhook
            };
        }

        public async Task<BaseResultModel> DeleteWebhookAsync(Guid id)
        {
            var webhook = await _unitOfWork.WebhookConfigRepository.GetByIdAsync(id);
            if (webhook == null || webhook.IsDeleted)
            {
                throw new NotExistException("", MessageConstants.WEBHOOK_NOT_EXIST_CODE);
            }

            _unitOfWork.WebhookConfigRepository.PermanentDeletedAsync(webhook);
            await _unitOfWork.SaveAsync();

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Message = "Webhook deleted successfully"
            };
        }

        public async Task<BaseResultModel> GetWebhooksByAccountIdAsync(Guid accountId, PaginationParameter paginationParameter)
        {
            var webhooks = await _unitOfWork.WebhookConfigRepository.ToPaginationIncludeAsync(
                paginationParameter,
                filter: w => w.AccountId == accountId && !w.IsDeleted
            );

            return new BaseResultModel
            {
                Status = StatusCodes.Status200OK,
                Data = webhooks
            };
        }
    }
}
