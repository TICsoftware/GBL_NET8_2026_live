using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.BAL
{
    public class mainBlockmasterFields_BAL
    {
        private readonly MainBlockmasterFields_DAL dal;

        public mainBlockmasterFields_BAL(IConfiguration config)
        {
            dal = new MainBlockmasterFields_DAL(config);
        }

        public BlockMasterFieldAssign Load(int componentMasterId)
        {
            var selected = dal.GetSelectedFields(componentMasterId) ?? new List<int>();
            var all = dal.GetAllFields() ?? new List<BlockMasterField>();

            return new BlockMasterFieldAssign
            {
                block_master_id = componentMasterId,
                SelectedFieldIds = selected,
                AllFields = all
            };
        }

        public void Save(int main_block_master_id, List<int> fieldIds, int userId)
        {
            string csv = string.Join(",", fieldIds);
            dal.Save(main_block_master_id, csv, userId);
        }
    }

}