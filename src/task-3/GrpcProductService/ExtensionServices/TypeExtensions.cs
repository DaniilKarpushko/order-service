using Google.Type;

namespace GrpcProductService.ExtensionServices;

public static class TypeExtensions
{
    public static decimal ToDecimal(this Money value)
    {
        return value.Units + (value.Nanos / 1_000_000_000m);
    }

    public static Money ToMoney(this decimal value)
    {
        return new Money
        {
            Units = decimal.ToInt64(value),
            Nanos = decimal.ToInt32((value - decimal.ToInt64(value)) * 1_000_000_000m),
        };
    }
}