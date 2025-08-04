// "-----------------------------------------------------------------------
//  <copyright file="LanguageMapper.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
using TechShop.Data.Entities.Languages;
using TechShop.Data.Request;

namespace TechShop.Data.Mapper.LanguageMapper
{
    public class LanguageMapper : IMapper<Language, LanguageRequest>
    {
        public LanguageRequest ToDto(Language entity)
        {
            if (entity == null) return null;
            return new LanguageRequest
            {
                Key = entity.Id,
                Name = entity.Name,
                Flag = entity.Flag,
                IsActive = entity.IsActive,
            };
        }

        public ICollection<LanguageRequest> ToDtoList(ICollection<Language> entities)
        {
            if (entities == null || !entities.Any()) return new List<LanguageRequest>();
            return entities.Select(ToDto).ToList();
        }

        public Language ToEntity(LanguageRequest dto)
        {
            if (dto == null) return null;
            return new Language
            {
                Id = dto.Key,
                Name = dto.Name,
                Flag = dto.Flag,
                IsActive = dto.IsActive,
            };
        }

        public ICollection<Language> ToEntityList(ICollection<LanguageRequest> dtos)
        {
            if (dtos == null || !dtos.Any()) return new List<Language>();
            return dtos.Select(ToEntity).ToList();
        }

        public void UpdateEntity(Language entity, LanguageRequest dto)
        {
            throw new NotImplementedException();
        }
    }
}
