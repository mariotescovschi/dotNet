using System.ComponentModel.DataAnnotations;
using Product_Management_API.Enums;

namespace Product_Management_API.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ProductCategoryAttribute : ValidationAttribute
{
    private readonly ProductCategory[] _allowedCategories;

    public ProductCategoryAttribute(params ProductCategory[] allowedCategories)
    {
        _allowedCategories = allowedCategories.Length > 0 ? allowedCategories : [];
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        if (value is not ProductCategory category)
            return false;

        if (_allowedCategories.Length == 0)
            return Enum.IsDefined(typeof(ProductCategory), category);

        return _allowedCategories.Contains(category);
    }

    public override string FormatErrorMessage(string name)
    {
        if (_allowedCategories.Length == 0)
            return $"The field {name} must be a valid ProductCategory.";

        string allowedValues = string.Join(", ", _allowedCategories.Select(c => c.ToString()));
        return $"The field {name} must be one of the following categories: {allowedValues}.";
    }
}
