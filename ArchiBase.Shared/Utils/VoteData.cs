namespace ArchiBase.Utils;

public class Vote
{
    public Guid Author { get; set; }
    public int VoteValue { get; set; }
}

public class VoteData
{
    public List<Vote>? Values { get; set; } = [];

    public int Upvotes => Values?.Where(v => v.VoteValue > 0).Sum(v => v.VoteValue) ?? 0;
    public int Downvotes => -Values?.Where(v => v.VoteValue < 0).Sum(v => v.VoteValue) ?? 0;

    public int Votes => Values?.Sum(v => v.VoteValue) ?? 0;
}