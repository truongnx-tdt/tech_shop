// "-----------------------------------------------------------------------
//  <copyright file="LanguageTranslationMapper.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
using TechShop.Data.DTO;
using TechShop.Data.Entities.Languages;

namespace TechShop.Data.Mapper.LanguageMapper
{
    public class LanguageTranslationMapper : IMapper<LanguageTranslation, LanguageTranslationDTO>
    {
        public LanguageTranslationDTO ToDto(LanguageTranslation entity)
        {
            if (entity == null) return null;
            return new LanguageTranslationDTO
            {
                LanguageCode = entity.LanguageCode,
                Key = entity.Key,
                Value = entity.Value,
                Module = entity.Module
            };
        }
        public LanguageTranslation ToEntity(LanguageTranslationDTO dto)
        {
            if (dto == null) return null;
            return new LanguageTranslation
            {
                LanguageCode = dto.LanguageCode,
                Key = dto.Key,
                Value = dto.Value,
                Module = dto.Module ?? "sys"
            };
        }
        public void UpdateEntity(LanguageTranslation entity, LanguageTranslationDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
