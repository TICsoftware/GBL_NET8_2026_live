using System.Collections.Generic;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.BAL
{
    public interface IGeographyMaster_BAL
    {
        List<GeographyMaster> GetAll();
        GeographyMaster GetById(int id);
        List<LanguageMaster> GetLanguages();
        int Add(GeographyMaster g);
        void Update(GeographyMaster g);
        void Deactivate(int id, int userId);
    }
}
