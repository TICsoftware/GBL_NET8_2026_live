using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System;

namespace  GBL_MVC.Models.Manage_Model
{
    public enum FieldType
    {
        Text,
        Image,
        CkEditor
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DynamicFieldAttribute : Attribute
    {
        public FieldType FieldType { get; set; }

        public DynamicFieldAttribute(FieldType fieldType)
        {
            FieldType = fieldType;
        }
    }

    public class DynamicFormViewModel
    {
        [DynamicField(FieldType.Text)]
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100)]
        public string Title { get; set; }

        [DynamicField(FieldType.Image)]
        [Required(ErrorMessage = "Please upload an image.")]
        public IFormFile ImageFile { get; set; }

        [DynamicField(FieldType.CkEditor)]
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
    }
}
