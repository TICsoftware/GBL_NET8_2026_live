using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;
using System.Data;
namespace Core_project_BusinessLogic.BAL
{
    public class Main_Spot_Template_Details_BAL
    {
        private readonly Main_Spot_Template_Details_DAL dal;
        private readonly GeographyMaster_DAL _geogDal;
        private readonly LanguageMaster_DAL _langDal;
        private readonly SpotTemplateTypeMaster_dal _spotref;
        private readonly MainSpotTemplateReference_DAL _mainsoptrefDal;

        public Main_Spot_Template_Details_BAL(IConfiguration config)
        {
            dal = new Main_Spot_Template_Details_DAL(config);
            _langDal = new LanguageMaster_DAL(config);
            _geogDal = new GeographyMaster_DAL(config);
            _spotref = new SpotTemplateTypeMaster_dal(config);
            _mainsoptrefDal = new MainSpotTemplateReference_DAL(config);
        }

        public List<Main_spot_template_details> GetAll() => dal.GetAll();

        public Main_spot_template_details GetById(int id) => dal.GetById(id);

        public int Add(Main_spot_template_details t) => dal.Insert(t);

        public void Update(Main_spot_template_details t) => dal.Update(t);

        public void Deactivate(int id, int userId) => dal.Deactivate(id, userId);

        public DataSet GetSpotLayoutById(int id) => dal.GetSpotLayoutById(id);

        public List<SpotReference> GetAllSpotReference() => _spotref.GetAllActive();
        public List<LanguageMaster> GetLanguages() => _langDal.GetAllActive();
        public List<GeographyMaster> GetGeography() => _geogDal.GetAll();

        public List<MainSpotTemplateReference> GetAllMainStopReferenceTemplate() => _mainsoptrefDal.GetAll();


    }
}
