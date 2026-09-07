using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.BAL
{
    public class GeographyMaster_BAL : IGeographyMaster_BAL
    {
        private readonly GeographyMaster_DAL _dal;
        private readonly LanguageMaster_DAL _langDal;
        public GeographyMaster_BAL(IConfiguration config)
        {
            _dal = new GeographyMaster_DAL(config);
            _langDal = new LanguageMaster_DAL(config);
        }
        public GeographyMaster_BAL(GeographyMaster_DAL dal, LanguageMaster_DAL langDal)
        {
            _dal = dal;
            _langDal = langDal;
        }

        public List<GeographyMaster> GetAll() => _dal.GetAll();

        public GeographyMaster GetById(int id) => _dal.GetById(id);
       public List<LanguageMaster> GetLanguages() => _langDal.GetAllActive();
        public int Add(GeographyMaster g) => _dal.Add(g);

        public void Update(GeographyMaster g) => _dal.Update(g);

        public void Deactivate(int id, int userId) => _dal.Deactivate(id, userId);
    }
}
