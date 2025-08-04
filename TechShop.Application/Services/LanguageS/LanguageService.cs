// "-----------------------------------------------------------------------
//  <copyright file="LanguageService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TechShop.Data.Entities.Languages;
using TechShop.Data.Mapper;
using TechShop.Data.Request;
using TechShop.Data.Response;
using TechShop.Infrastructure.UnitOfWork;
using TechShop.Manufacture.CommonConst;

namespace TechShop.Application.Services.LanguageS
{
    public class LanguageService : BaseService, ILanguageService
    {
        private readonly IMapper<Language, LanguageRequest> _mapper;
        private readonly ILogger<LanguageService> _logger;
        public LanguageService(IUnitOfWork unitOfWork, ILogger<LanguageService> logger, IMapper<Language, LanguageRequest> mapper) : base(unitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<string> AddLanguage(List<LanguageRequest> request)
        {
            try
            {
                List<Language> language = _mapper.ToEntityList(request).ToList();
                var ids = language.Select(x => x.Id).ToList();
                // Check if exists in database before adding 
                var data = await UnitOfWork.Language.AnyAsync(x => ids.Contains(x.Id));
                if (!data)
                {
                    await UnitOfWork.BulkInsertAsync(language).ConfigureAwait(false);
                    return StringConst.AddDone;
                }
                else
                {
                    return ResponseStatusName.Exists;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StringConst.Exception;
            }
        }

        public async Task<List<Language>> GetLanguages()
        {
            var rs = await UnitOfWork.Language.GetAllAsyn().ConfigureAwait(false);
            return rs.ToList();
        }
        /// <summary>
        /// Get Language For Client
        /// </summary>
        /// <returns></returns>
        public List<LanguageResponse> LanguageResponses()
        {
            var data = UnitOfWork.Language.FindBy(x => x.IsActive == true);
            var rs = data.Select(x => new LanguageResponse
            {
                Id = x.Id,
                Name = x.Name,
                Flag = x.Flag,
            });
            return rs.ToList();
        }

        public async Task<bool> UpdateLanguage(List<LanguageRequest> request)
        {
            bool isUpdate = false;
            try
            {
                _logger?.LogInformation("Updating languages..." + JsonConvert.SerializeObject(request));
                if (request == null || !request.Any())
                {
                    _logger?.LogError("No languages to update.");
                    return false;
                }
                // transaction
                isUpdate = await UnitOfWork.ExecuteWithStrategyAsync(async () =>
                {
                    using (var transaction = await UnitOfWork.BeginTransactionAsync())
                    {
                        try
                        {
                            // Update languages in bulk
                            await UnitOfWork.BulkUpdateAsync(_mapper.ToEntityList(request).ToList());
                            transaction.Commit();
                            _logger?.LogInformation("Languages updated successfully.");
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            _logger?.LogError($"Error updating languages: {ex.Message}");
                            return false;
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.Message);
            }
            return isUpdate;
        }
    }
}
