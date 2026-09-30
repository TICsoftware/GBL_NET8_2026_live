using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.BAL
{
    public class Product_Packaging_Master_BAL
    {
        private readonly Product_Packaging_Master_DAL _dal;
        private readonly LanguageMaster_DAL _langDal;

        public Product_Packaging_Master_BAL(IConfiguration config)
        {
            _dal = new Product_Packaging_Master_DAL(config);
            _langDal = new LanguageMaster_DAL(config);
        }

        public (List<Product_Packaging_Master_Entity>, int) GetPaged(Product_Packaging_Master_Entity entity) =>
            _dal.GetPaged(entity.SearchText ?? string.Empty, entity.PageNumber, entity.PageSize);

        public Product_Packaging_Master_Entity GetById(int id) => _dal.GetById(id);

        public bool NameExists(string name, int? languageId, int excludeId) =>
            _dal.NameExists(name, languageId, excludeId);

        public List<LanguageMaster> GetLanguages() => _langDal.GetAllActive();

        public int Save(Product_Packaging_Master_Entity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new Exception("Name is required");

            if (!entity.Sequence.HasValue || entity.Sequence < 1)
                throw new Exception("Display order is required");

            if (NameExists(entity.Name!, entity.Language_Master_Id, entity.product_packaging_MasterId))
                throw new Exception("This name already exists for the selected language.");

            entity.Thumbnailimage_Id ??= string.Empty;
            entity.Thumbnailimage_alt ??= string.Empty;

            if (entity.product_packaging_MasterId == 0)
            {
                entity.Status ??= 1;
                return _dal.Insert(entity);
            }

            _dal.Update(entity);
            return entity.product_packaging_MasterId;
        }

        public void ChangeStatus(int id, int status, int? updateUserId) =>
            _dal.ChangeStatus(id, status, updateUserId);

        public void Delete(int id)
        {
            if (id <= 0)
                throw new Exception("Invalid packaging id.");
            _dal.Delete(id);
        }

        public void UpdateSequence(List<Product_Packaging_Master_Entity> list)
        {
            if (list == null || list.Count == 0)
                throw new Exception("Invalid sequence data.");
            _dal.UpdateSequence(list);
        }
    }
}
