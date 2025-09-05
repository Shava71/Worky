using System.ComponentModel.DataAnnotations;

namespace Worky.DTO;

public record MakeDealRequest(
    [Range(1, 4)] ulong tarrif_id,
    int countMonth
);