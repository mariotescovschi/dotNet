using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Product_Management_API.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ValidSkuAttribute : ValidationAttribute, IClientModelValidator
{
    private const string SkuPattern = @"^[a-zA-Z0-9\-]{5,20}$";
    private const int MinLength = 5;
    private const int MaxLength = 20;

    public ValidSkuAttribute()
    {
        ErrorMessage = "SKU must be alphanumeric with hyphens, between 5 and 20 characters";
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        string sku = value.ToString()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(sku))
            return false;

        return Regex.IsMatch(sku, SkuPattern);
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes["data-val"] = "true";
        context.Attributes["data-val-validsku"] = ErrorMessage ?? "Invalid SKU format";
        context.Attributes["data-val-validsku-pattern"] = SkuPattern;
        context.Attributes["data-val-validsku-minlength"] = MinLength.ToString();
        context.Attributes["data-val-validsku-maxlength"] = MaxLength.ToString();
    }
}