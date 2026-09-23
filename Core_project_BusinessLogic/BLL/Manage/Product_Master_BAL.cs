using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.BAL
{
    public class Product_Master_BAL
    {
        private readonly Product_Master_DAL _dal;
        private readonly LanguageMaster_DAL _langDal;

        public Product_Master_BAL(IConfiguration config)
        {
            _dal = new Product_Master_DAL(config);
            _langDal = new LanguageMaster_DAL(config);
        }

        public (List<Product_Master_Entity>, int) GetPaged(Product_Master_Entity entity) =>
            _dal.GetPaged(entity.SearchText ?? string.Empty, entity.PageNumber, entity.PageSize);

        public Product_Master_Entity GetById(int id) => _dal.GetById(id);

        public bool NameExists(string name, int? languageId, int excludeId) =>
            _dal.NameExists(name, languageId, excludeId);

        public List<LanguageMaster> GetLanguages() => _langDal.GetAllActive();

        public List<ProductLookupItem> GetActiveIndustries() => _dal.GetActiveIndustries();

        public List<ProductLookupItem> GetActiveApplications() => _dal.GetActiveApplications();

        public List<ProductLookupItem> GetActivePackaging() => _dal.GetActivePackaging();

        public List<ProductLookupItem> GetSubcategoriesByIndustries(List<int> industryIds) =>
            _dal.GetSubcategoriesByIndustries(industryIds ?? new List<int>());

        public int Save(Product_Master_Entity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.ProductName))
                throw new Exception("Name is required");

            if (string.IsNullOrWhiteSpace(entity.Product_pagename))
                throw new Exception("Page name is required");

            if (!entity.Sequence.HasValue || entity.Sequence < 1)
                throw new Exception("Display order is required");

            if (NameExists(entity.ProductName!, entity.Language_Master_Id, entity.ProductId))
                throw new Exception("This name already exists for the selected language.");

            int productId;
            if (entity.ProductId == 0)
            {
                entity.Status ??= 1;
                productId = _dal.Insert(entity);
            }
            else
            {
                _dal.Update(entity);
                productId = entity.ProductId;
            }

            _dal.SaveMappings(productId, entity, entity.Update_UserId ?? entity.Create_UserId);
            return productId;
        }

        public void ChangeStatus(int id, int status, int? updateUserId) =>
            _dal.ChangeStatus(id, status, updateUserId);

        public void Delete(int id)
        {
            if (id <= 0)
                throw new Exception("Invalid product id.");
            _dal.Delete(id);
        }

        public void UpdateSequence(List<Product_Master_Entity> list)
        {
            if (list == null || list.Count == 0)
                throw new Exception("Invalid sequence data.");
            _dal.UpdateSequence(list);
        }
    }
}
