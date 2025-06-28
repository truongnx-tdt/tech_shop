// "-----------------------------------------------------------------------
//  <copyright file="IMapper.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

namespace TechShop.Data.Mapper
{
    public interface IMapper<TEntity, TDto>
    {
        TDto ToDto(TEntity entity);
        TEntity ToEntity(TDto dto);
        ICollection<TDto> ToDtoList(ICollection<TEntity> entities);
        ICollection<TEntity> ToEntityList(ICollection<TDto> dtos);
        void UpdateEntity(TEntity entity, TDto dto);
    }
}
