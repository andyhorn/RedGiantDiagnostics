namespace API.Models
{
    public interface IIsvStatisticsTable : IServerStatisticsTable
    {
        int[] Checkouts { get; set; }
        int[] Denials { get; set; }
        int[] LicenseRemovals { get; set; }
    }
}