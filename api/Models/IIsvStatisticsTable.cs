namespace API.Models
{
    public interface IIsvStatisticsTable : IStatisticsTable
    {
        int[] Checkouts { get; set; }
        int[] Denials { get; set; }
        int[] LicenseRemovals { get; set; }
    }
}