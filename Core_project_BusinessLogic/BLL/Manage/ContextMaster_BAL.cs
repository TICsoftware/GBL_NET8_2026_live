using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.BAL
{
    public class ContextMaster_BAL
    {
        private readonly ContextMaster_DAL _dal;
        private readonly LanguageMaster_DAL _langDal;

        public ContextMaster_BAL(IConfiguration config)
        {
            _dal = new ContextMaster_DAL(config);
            _langDal = new LanguageMaster_DAL(config);
        }

        public List<ContextMaster> GetAll() => _dal.GetAll();
        public (List<ContextMasterListVM> Data, int Total) GetPaged(string search, int page, int pageSize)
        {
            return _dal.GetPaged(search, page, pageSize);
        }

        public ContextMaster GetById(int id) => _dal.GetById(id);

        public ContextMaster GetById_layout(int id)
        {
            return _dal.GetById_layout(id);
        }

        public int Add(ContextMaster m) => _dal.Insert(m);
        public void Update(ContextMaster m) => _dal.Update(m);
        public void Deactivate(int id, int userId) => _dal.Deactivate(id, userId);

        public List<LanguageMaster> GetLanguages() => _langDal.GetAllActive();
    }
}
