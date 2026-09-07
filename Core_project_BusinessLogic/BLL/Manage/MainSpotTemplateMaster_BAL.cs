using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;
namespace Core_project_BusinessLogic.BAL
{
    public class MainSpotTemplateMaster_BAL
    {
        private readonly MainSpotTemplateMaster_DAL dal;
        //private readonly TemplateTypeMaster_dal typeDal;
        private readonly LanguageMaster_DAL _langDal;
        public MainSpotTemplateMaster_BAL(IConfiguration config)
        {
            dal = new MainSpotTemplateMaster_DAL(config);
            // typeDal = new TemplateTypeMaster_dal(config);
            _langDal = new LanguageMaster_DAL(config);
        }

        public List<MainSpotTemplateMaster> GetAll() => dal.GetAll();

        public MainSpotTemplateMaster GetById(int id) => dal.GetById(id);

        //   public List<TemplateTypeMaster> GetTemplateTypes() => typeDal.GetAll();
        public List<LanguageMaster> GetLanguages() => _langDal.GetAllActive();
        public int Add(MainSpotTemplateMaster t) => dal.Insert(t);

        public void Update(MainSpotTemplateMaster t) => dal.Update(t);

        public void Deactivate(int id, int userId) => dal.Deactivate(id, userId);



        public List<int> GetComponentIds(int mainSpotTemplateId)
        {
            return dal.GetTemplatesByMainSpot(mainSpotTemplateId);
        }

        public void SaveMappings(int mainSpotTemplateId, List<int> templateIds)
        {
            // delete old mappings
            dal.DeleteByMainSpot(mainSpotTemplateId);

            // insert new
            foreach (var tid in templateIds)
            {
                dal.Insert(mainSpotTemplateId, tid);
            }
        }
        public (List<MainSpotTemplateMaster> data, int total)
        GetPaged(string search, int page, int size)
        {
            int total;
            var data = dal.GetPaged(search, page, size, out total);
            return (data, total);
        }
        public List<int> GetTemplateIds(int mainSpotTemplateId)
        {
            return dal.GetTemplatesByMainSpot(mainSpotTemplateId);
        }
    }
}
