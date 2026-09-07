
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core_project_BusinessLogic.Entity
{
    
public class ContentSpotMappingTemp
{
    public int cont_spot_id { get; set; }
    public int cont_id { get; set; }
    public int spot_id { get; set; }
    public int spot_type { get; set; }
    public int cont_spot_sequence { get; set; }
}

public class SpotOrderVM
{
    public int id { get; set; }
    public int sequence { get; set; }
}
public class AddSpotVM
{
    public int SpotId { get; set; }
    public int SpotType { get; set; }
}
public class SpotPreviewVM
{
    public int SpotId { get; set; }
    public int SpotType { get; set; } // 1 = Context, 2 = Template
}

public class AddSpotMappingVM
{
    public string contId { get; set; }     // encrypted
    public string tempId { get; set; }     // encrypted
    public List<SpotItemVM> spots { get; set; }
}

public class SpotItemVM
{
    public int SpotId { get; set; }
    public int SpotType { get; set; }
}
}
