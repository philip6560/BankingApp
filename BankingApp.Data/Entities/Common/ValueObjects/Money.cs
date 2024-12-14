using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Data.Entities.Common.ValueObjects;

/// <summary>
/// Value object for money
/// </summary>

[ComplexType]
public record Money(
    [property: Precision(18,2)]
    decimal Amount,
    [property: StringLength(3)]
    string Currency = "NGN")
{
    public Money() : this(0.00M) { }

    public static Money operator +(Money a, Money b)
    {
        ValidateCurrency(a, b, "Cannot add two different currencies");
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        ValidateCurrency(a, b, "Cannot subtract two different currencies");
        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public static Money operator /(Money a, Money b)
    {
        ValidateCurrency(a, b, "Cannot divide two different currencies");
        return new Money(a.Amount / b.Amount, a.Currency);
    }

    public static Money operator /(Money a, int b)
    {
        return new Money(a.Amount / b, a.Currency);
    }

    public static Money operator *(Money a, Money b)
    {
        ValidateCurrency(a, b, "Cannot multiply two different currencies");
        return new Money(a.Amount * b.Amount, a.Currency);
    }

    public static Money operator *(Money a, int b)
    {
        return new Money(a.Amount * b, a.Currency);
    }

    public static Money operator *(Money a, decimal b)
    {
        return new Money(a.Amount * b, a.Currency);
    }

    public static Money operator %(Money a, Money b)
    {
        ValidateCurrency(a, b, "Cannot divide two different currencies");
        return new Money(a.Amount % b.Amount, a.Currency);
    }

    public static bool operator >(Money a, Money b)
    {
        ValidateCurrency(a, b, "Cannot compare two different currencies");
        return a.Amount > b.Amount;
    }

    public static bool operator >(Money a, int b)
    {
        return a.Amount > b;
    }

    public static bool operator <(Money a, int b)
    {
        return a.Amount < b;
    }

    public static bool operator <(Money a, Money b)
    {
        ValidateCurrency(a, b, "Cannot compare two different currencies");
        return a.Amount < b.Amount;
    }

    public static bool operator >=(Money a, Money b)
    {
        ValidateCurrency(a, b, "Cannot compare two different currencies");
        return a.Amount >= b.Amount;
    }

    public static bool operator <=(Money a, Money b)
    {
        ValidateCurrency(a, b, "Cannot compare two different currencies");
        return a.Amount <= b.Amount;
    }

    private static void ValidateCurrency(Money a, Money b, string message)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException(message);
        }
    }

    public override string ToString()
        => $"{Currency} {Amount}";

}

