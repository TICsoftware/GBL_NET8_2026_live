using Microsoft.Identity.Client;

namespace GBL_MVC;

public class Component
{
    public int CTR_Id { get; set; }
    public string? title { get; set; }
    public int? sequence { get; set; }
    public int? status { get; set; }
    public string? component_label { get; set; }
    public int? field_Id { get; set; }
    public string? group_id { get; set; }
    public int? block_field_count{get;set;}
    public int? Component_field_count{get;set;}
}

public class Component_data
{
    public int Id { get; set; }
    public int CTR_Id { get; set; }
    public string? context_group_id { get; set; }
    public string? field_Title { get; set; }
    public int? sequence { get; set; }

}

public class List_Components
{
    public List<Component>? Components { get; set; }
    public List<Component_data>? Field_details { get; set; }
}