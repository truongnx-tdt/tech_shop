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
using TechShop.Infrastructure.UnitOfWork;

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

        public Task<bool> EditMultiAsync(List<LanguageTranslationDTO> request)
        {
            throw new NotImplementedException();
        }

        public async Task<List<LanguageTranslation>> GetAsync(string languageCode, string? module)
        {
            var rs = await UnitOfWork.LanguageTranslation.FindAllAsync(x => x.LanguageCode == languageCode);
            return rs.ToList();
        }

        public Task<bool> Import(IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}
