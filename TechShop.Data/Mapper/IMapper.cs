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
        void UpdateEntity(TEntity entity, TDto dto);
    }
}
