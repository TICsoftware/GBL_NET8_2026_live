using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;
namespace Core_project_BusinessLogic.BAL
{
    public class MainSpotTemplateReference_BAL
    {
        private readonly MainSpotTemplateReference_DAL dal;
        private readonly MainSpotTemplateMaster_DAL _mainsoptDal;
        private readonly TemplateMaster_BAL _templatemasterbal;

        private readonly LanguageMaster_DAL _langDal;
        public MainSpotTemplateReference_BAL(IConfiguration config)
        {
            dal = new MainSpotTemplateReference_DAL(config);
            _langDal = new LanguageMaster_DAL(config);
            _mainsoptDal = new MainSpotTemplateMaster_DAL(config);
            _templatemasterbal = new TemplateMaster_BAL(config);
        }

        public List<LanguageMaster> GetLanguages() => _langDal.GetAllActive();

        public List<MainSpotTemplateMaster> GetAllMainStopTemplate() => _mainsoptDal.GetAll();
        public List<TemplateMaster> GetAllTemplates() => _templatemasterbal.GetAllTemplates();

        public List<MainSpotTemplateReference> GetAll() => dal.GetAll();

        public MainSpotTemplateReference GetById(int id) => dal.GetById(id);

        public int Add(MainSpotTemplateReference t) => dal.Insert(t);

        public void Update(MainSpotTemplateReference t) => dal.Update(t);

        public void Deactivate(int id, int userId) => dal.Deactivate(id, userId);
    }
}
