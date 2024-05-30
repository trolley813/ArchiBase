using Microsoft.EntityFrameworkCore;

namespace ArchiBase.Models;

public enum DatePrecision
{
    None = 0,
    Full = 1,
    YearAndMonthOnly = 2,
    YearOnly = 3,
    Circa = 4,
    Decade = 5,
    DecadeOnly = 6,
    Century = 7,
    CenturyOnly = 8
}

public static class DateExtensions
{
    public static int GetCentury(this DateTime date)
    {
        return (date.Year - 1) / 100 + 1;
    }

    public static string GetEarlyMidLate(this DateTime date, bool useCentury)
    {
        if (useCentury)
        {
            return (date.Year % 100) switch
            {
                >= 1 and <= 29 => "early ",
                >= 30 and <= 69 => "mid-",
                0 or >= 70 and <= 99 => "late ",
                _ => ""
            };
        }
        else
        {
            return (date.Year % 10) switch
            {
                >= 0 and <= 2 => "early ",
                >= 3 and <= 6 => "mid-",
                >= 7 and <= 9 => "late ",
                _ => ""
            };
        }
    }
}

[Owned]
public class ImpreciseDate
{
    public DateTime Date { get; set; }
    public DatePrecision Precision { get; set; }

    public ImpreciseDate Clone()
    {
        return new ImpreciseDate { Date = Date, Precision = Precision };
    }

    public override string ToString() =>
    Precision switch
    {
        DatePrecision.None => Date.ToString(),
        DatePrecision.Full => Date.ToString("dd.MM.yyyy"),
        DatePrecision.YearAndMonthOnly => Date.ToString("MM.yyyy"),
        DatePrecision.YearOnly => Date.ToString("yyyy"),
        DatePrecision.Circa => "ca. " + Date.ToString("yyyy"),
        DatePrecision.Decade => $"{Date.GetEarlyMidLate(useCentury: false)}{Date.Year / 10 * 10}s",
        DatePrecision.DecadeOnly => $"{Date.Year / 10 * 10}s",
        DatePrecision.Century => $"{Date.GetEarlyMidLate(useCentury: true)}{Date.GetCentury()} century",
        DatePrecision.CenturyOnly => $"{Date.GetCentury()} century",
        _ => throw new NotImplementedException()
    };


}