using Humanizer;
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
    CenturyOnly = 8,
    YearOrEarlier = 9,
    YearOrLater = 10,
}

public static class DateExtensions
{
    public static int GetCentury(this DateTime date)
    {
        return (date.Year - 1) / 100 + 1;
    }

    public enum EarlyMidLate { Early, Mid, Late }

    public static string GetRepresentation(this EarlyMidLate earlyMidLate) => earlyMidLate switch
    {
        EarlyMidLate.Early => "early ",
        EarlyMidLate.Mid => "mid-",
        EarlyMidLate.Late => "late ",
        _ => "",
    };

    public static EarlyMidLate GetEarlyMidLate(this DateTime date, bool useCentury)
    {
        if (useCentury)
        {
            return (date.Year % 100) switch
            {
                >= 1 and <= 29 => EarlyMidLate.Early,
                >= 30 and <= 69 => EarlyMidLate.Mid,
                0 or >= 70 and <= 99 => EarlyMidLate.Late,
                _ => EarlyMidLate.Mid
            };
        }
        else
        {
            return (date.Year % 10) switch
            {
                >= 0 and <= 2 => EarlyMidLate.Early,
                >= 3 and <= 6 => EarlyMidLate.Mid,
                >= 7 and <= 9 => EarlyMidLate.Late,
                _ => EarlyMidLate.Mid
            };
        }
    }
}

[Owned]
public class ImpreciseDate : IComparable<ImpreciseDate>
{
    public DateTime Date { get; set; }
    public DatePrecision Precision { get; set; }

    public ImpreciseDate Clone()
    {
        return new ImpreciseDate { Date = Date, Precision = Precision };
    }

    public int CompareTo(ImpreciseDate? other)
    {
        return Date.CompareTo(other?.Date);
    }

    public override string ToString() =>
    Precision switch
    {
        DatePrecision.None => Date.ToString(),
        DatePrecision.Full => Date.ToString("dd.MM.yyyy"),
        DatePrecision.YearAndMonthOnly => Date.ToString("MM.yyyy"),
        DatePrecision.YearOnly => Date.ToString("yyyy"),
        DatePrecision.Circa => "ca. " + Date.ToString("yyyy"),
        DatePrecision.Decade =>
            $"{Date.GetEarlyMidLate(useCentury: false).GetRepresentation()}{Date.Year / 10 * 10}s",
        DatePrecision.DecadeOnly => $"{Date.Year / 10 * 10}s",
        DatePrecision.Century =>
            $"{Date.GetEarlyMidLate(useCentury: true).GetRepresentation()}{Date.GetCentury().Ordinalize()} century",
        DatePrecision.CenturyOnly => $"{Date.GetCentury().Ordinalize()} century",
        DatePrecision.YearOrEarlier => Date.ToString("yyyy") + " or earlier",
        DatePrecision.YearOrLater => Date.ToString("yyyy") + " or later",
        _ => throw new NotImplementedException()
    };


}