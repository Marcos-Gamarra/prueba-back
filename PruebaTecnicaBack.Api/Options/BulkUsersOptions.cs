using System.ComponentModel.DataAnnotations;

namespace PruebaTecnicaBack.Options;

public class BulkUsersOptions
{
    public const string SectionName = "BulkUsers";

    [Range(1, 32)]
    public int MaxDegreeOfParallelism { get; set; } = 8;

    [Range(1, 10_000)]
    public int MaxBatchSize { get; set; } = 1000;
}