using System.ComponentModel.DataAnnotations;

namespace GBL_BusinessLogic.Entity;

public class Search
{
    public List<Content_Search_Display> objsearchdisplay { get; set; }
    public int count { get; set; }
    public int currentpage { get; set; }
    public int totalrecords { get; set; }
    public string keyword { get; set; }
    public string txttitle { get; set; }
}


public class Content_Search_Display
{

    public string Keyword { get; set; }
    public string intro { get; set; }
    public string hmpg_intro { get; set; }

    public string cont_title { get; set; }
    public string content { get; set; }
    public string cont_pagename { get; set; }
    public string cont_external_url { get; set; }
    public string cont_pdf { get; set; }
    public int cont_status { get; set; }
    public string PageLink { get; set; }
    public int Row { get; set; }
    public string URL { get; set; }
    public string search_URL { get; set; }
    public string cont_loc { get; set; }
    public string cont_publishdate { get; set; }
    public string section_name { get; set; }


}
