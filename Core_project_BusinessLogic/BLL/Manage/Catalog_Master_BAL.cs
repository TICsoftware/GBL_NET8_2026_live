using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.BAL
{
    public class Catalog_Master_BAL
    {
        private readonly Catalog_Master_DAL _dal;

        public Catalog_Master_BAL(IConfiguration config)
        {
            _dal = new Catalog_Master_DAL(config);
        }

        public (List<CatalogMasterEntity>, int) GetPaged(CatalogMasterEntity entity)
        {
            return _dal.GetPaged(
                entity.MasterType!,
                entity.SearchText ?? string.Empty,
                entity.PageNumber,
                entity.PageSize
            );
        }

        public CatalogMasterEntity GetById(int id, string type) => _dal.GetById(id, type);

        public List<CatalogLookupItem> GetLookup(string lookupType) => _dal.GetLookup(lookupType);

        public int Save(CatalogMasterEntity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.MasterType))
                throw new Exception("Master type is required");

            if (IsMapping(entity.MasterType))
            {
                if (!entity.IndustryId.HasValue || entity.IndustryId <= 0)
                    throw new Exception("Industry is required");
                if (!entity.Category_Master_Id.HasValue || entity.Category_Master_Id <= 0)
                    throw new Exception("Subcategory is required");
            }
            else if (string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new Exception("Name is required");
            }

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

        public void Deactivate(int id, string type, int? updateUserId) =>
            _dal.Deactivate(id, type, updateUserId);

        public void Delete(int id, string type) => _dal.Delete(id, type);

        public void UpdateSequence(List<CatalogMasterEntity> list, string type) =>
            _dal.UpdateSequence(list, type);

        private static bool IsMapping(string type) =>
            string.Equals(type, "Mapping", StringComparison.OrdinalIgnoreCase);
    }
}
