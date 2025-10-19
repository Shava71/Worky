namespace WorkerService.DAL.Contracts;

public record AddFilter(
    ulong id,
    List<ulong> typeOfActivity_id
);