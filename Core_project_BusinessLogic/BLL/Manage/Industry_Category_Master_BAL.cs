using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.BAL
{
    public class Industry_Category_Master_BAL
    {
        private readonly Industry_Category_Master_DAL _dal;
        private readonly LanguageMaster_DAL _langDal;

        public Industry_Category_Master_BAL(IConfiguration config)
        {
            _dal = new Industry_Category_Master_DAL(config);
            _langDal = new LanguageMaster_DAL(config);
        }

        public (List<Industry_Category_Master_Entity>, int) GetPaged(Industry_Category_Master_Entity entity)
        {
            return _dal.GetPaged(
                entity.MasterType!,
                entity.SearchText ?? string.Empty,
                entity.PageNumber,
                entity.PageSize
            );
        }

        public Industry_Category_Master_Entity GetById(int id, string type) => _dal.GetById(id, type);

        public List<LanguageMaster> GetLanguages() => _langDal.GetAllActive();

        public int Save(Industry_Category_Master_Entity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.MasterType))
                throw new Exception("Master type is required");

            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new Exception("Name is required");

            if (!entity.Sequence.HasValue || entity.Sequence < 1)
                throw new Exception("Display order is required");

            if (entity.ID == 0)
            {
                entity.Status ??= 1;
                return _dal.Insert(entity);
            }

            _dal.Update(entity);
            return entity.ID;
        }

        public void ChangeStatus(int id, string type, int status, int? updateUserId) =>
            _dal.ChangeStatus(id, type, status, updateUserId);

        public void Delete(int id, string type) => _dal.Delete(id, type);

        public void UpdateSequence(List<Industry_Category_Master_Entity> list, string type) =>
            _dal.UpdateSequence(list, type);

        public List<IndustryCategoryLookupItem> GetActiveIndustries() => _dal.GetActiveIndustries();

        public List<IndustryCategoryLookupItem> GetActiveCategories() => _dal.GetActiveCategories();

        public List<Industry_Subcategory_Mapping_Entity> GetMappings()
        {
            return _dal.GetMappings()
                .OrderBy(m => m.IndustryName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(m => m.DisplayOrder)
                .ThenBy(m => m.CategoryName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public void SaveMappings(List<int> industryIds, List<int> categoryIds, int? createUserId)
        {
            if (industryIds == null || industryIds.Count == 0)
                throw new Exception("Select at least one industry.");
            if (categoryIds == null || categoryIds.Count == 0)
                throw new Exception("Select at least one category.");

            _dal.SaveMappings(industryIds, categoryIds, createUserId);
        }

        public void DeleteMapping(int industrySubcategoryId)
        {
            if (industrySubcategoryId <= 0)
                throw new Exception("Invalid mapping id.");
            _dal.DeleteMapping(industrySubcategoryId);
        }

        public void UpdateMappingSequence(List<Industry_Subcategory_Mapping_Entity> list)
        {
            if (list == null || list.Count == 0)
                throw new Exception("Invalid mapping sequence data.");
            _dal.UpdateMappingSequence(list);
        }
    }
}
