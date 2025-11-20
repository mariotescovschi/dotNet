using System.ComponentModel.DataAnnotations;

namespace Product_Management_API.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class PriceRangeAttribute : ValidationAttribute
{
    private readonly decimal _minPrice;
    private readonly decimal _maxPrice;

    public PriceRangeAttribute(double minPrice, double maxPrice)
    {
        _minPrice = Convert.ToDecimal(minPrice);
        _maxPrice = Convert.ToDecimal(maxPrice);
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        if (value is not decimal price)
        {
            if (value is double doubleValue)
                price = Convert.ToDecimal(doubleValue);
            else if (value is int intValue)
                price = Convert.ToDecimal(intValue);
            else
                return false;
        }

        return price >= _minPrice && price <= _maxPrice;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The field {name} must be between {_minPrice:C2} and {_maxPrice:C2}.";
    }
}