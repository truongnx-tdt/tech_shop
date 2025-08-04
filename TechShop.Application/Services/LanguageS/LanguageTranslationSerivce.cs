// "-----------------------------------------------------------------------
//  <copyright file="LanguageTranslationSerivce.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TechShop.Data.DTO;
using TechShop.Data.Entities.Languages;
using TechShop.Data.Mapper;
using TechShop.Data.Response;
using TechShop.Infrastructure.UnitOfWork;
using TechShop.Manufacture.CommonConst;

namespace TechShop.Application.Services.LanguageS
{
    public class LanguageTranslationSerivce : BaseService, ILanguageTranslationService
    {
        private readonly IMapper<LanguageTranslation, LanguageTranslationDTO> _mapper;
        private readonly ILogger<LanguageTranslationSerivce> _logger;
        public LanguageTranslationSerivce(IUnitOfWork unitOfWork, IMapper<LanguageTranslation, LanguageTranslationDTO> mapper, ILogger<LanguageTranslationSerivce> logger) : base(unitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> AddMultiAsync(List<LanguageTranslationDTO> request)
        {
            if (request == null || !request.Any())
            {
                return false;
            }
            var rs = await UnitOfWork.ExecuteWithStrategyAsync(async () =>
            {
                using (var transaction = await UnitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var translations = _mapper.ToEntityList(request).ToList();
                        await UnitOfWork.BulkInsertAsync(translations);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex.Message);
                        return false;
                    }
                }
            });
            return rs;
        }

        public Task<bool> DeleteByIds(List<int> id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByLCode(string languageCode)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<object>> EditMultiAsync(List<LanguageTranslationDTO> request)
        {
            var res = new ApiResponse<object>
            {
                Status = ResponseStatusCode.UnSuccess,
                Message = StringConst.UpdateFailed
            };

            if (request == null || !request.Any())
            {
                return res;
            }

            if (request.Any(x => x.Id <= 0))
            {
                res.Message = StringConst.InvalidId;
                return res;
            }
            if (request.Any(x => string.IsNullOrEmpty(x.LanguageCode)))
            {
                res.Message = StringConst.InvalidLanguageCode;
                return res;
            }
            var rs = await UnitOfWork.ExecuteWithStrategyAsync(async () =>
            {
                using (var transaction = await UnitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var translations = _mapper.ToEntityList(request).ToList();
                        await UnitOfWork.BulkUpdateAsync(translations);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        res.Message = StringConst.Exception;
                        transaction.Rollback();
                        _logger.LogError(ex.Message);
                        return false;
                    }
                }
            });
            if (rs)
            {
                return new ApiResponse<object>
                {
                    Status = ResponseStatusCode.Success,
                    Message = StringConst.UpdateDone,
                };
            }
            return res;
        }

        public async Task<List<LanguageTranslation>> GetAsync(string languageCode, string? module)
        {
            var rs = await UnitOfWork.LanguageTranslation.FindAllAsync(x => String.IsNullOrEmpty(languageCode) || x.LanguageCode == languageCode);
            return rs.ToList();
        }

        public Task<bool> Import(IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}
