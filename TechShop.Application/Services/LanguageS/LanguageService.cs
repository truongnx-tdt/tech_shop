// "-----------------------------------------------------------------------
//  <copyright file="LanguageService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using Microsoft.Extensions.Logging;
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

        public async Task<string> AddLanguage(LanguageRequest request)
        {
            try
            {
                Language language = _mapper.ToEntity(request);
                var data = await UnitOfWork.Language.FindAsync(x => x.Id == language.Id);
                if (data == null)
                {
                    await UnitOfWork.Language.AddAsyn(language).ConfigureAwait(false);
                    if (await UnitOfWork.SaveChangesAsync())
                    {
                        return StringConst.AddDone;
                    }
                    else
                    {
                        return StringConst.AddFailed;
                    }
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
        public async Task<List<LanguageResponse>> LanguageResponses()
        {
            var data = UnitOfWork.Language.FindBy(x => x.IsActive == true);
            var rs = data.Select(x => new LanguageResponse
            {
                Key = x.Id,
                Name = x.Name,
                Flag = x.Flag,
            });
            return rs.ToList();
        }

        public async Task<bool> UpdateLanguage(LanguageRequest request)
        {
            try
            {
                Language language = _mapper.ToEntity(request);
                await UnitOfWork.Language.UpdateAsyn(language, language.Id);
                return await UnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.Message);
                return false;
            }

        }
    }
}
