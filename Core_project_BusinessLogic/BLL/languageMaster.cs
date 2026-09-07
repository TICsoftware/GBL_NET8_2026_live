using System.Collections.Generic;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic.BAL
{
    public class LanguageMaster_BAL
    {
        private readonly LanguageMaster_DAL _dal;

        public LanguageMaster_BAL(IConfiguration config)
        {
            _dal = new LanguageMaster_DAL(config);
        }

        public List<LanguageMaster> GetAll()
        {
            return _dal.GetAllLanguages();
        }

        public LanguageMaster GetById(int id)
        {
            return _dal.GetLanguageById(id);
        }

        public int Add(LanguageMaster lm)
        {
            return _dal.AddLanguage(lm);
        }

        public void Update(LanguageMaster lm)
        {
            _dal.UpdateLanguage(lm);
        }

        public void Deactivate(int id, int userId)
        {
            _dal.DeactivateLanguage(id, userId);
        }

         public List<LanguageMaster> GetLanguages() => _dal.GetAllLanguages();
    }
}
