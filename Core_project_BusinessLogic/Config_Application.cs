using System;
namespace Core_project_BusinessLogic;

class Config_Application
{
}

public static class Content_Types
{
    public static int Section = 1;
    public static int Common_Article = 2;
    public static int Related_Article = 3;
}


public static class Content_Status
{
    public static int Draft = 1;
    public static int Publish = 2;
    public static int InActive = 0;
    public static int Delete = -1;
}

public static class Template_type
{
    public static int type_Static = 1;

    public static int type_Dynamic = 2;
}