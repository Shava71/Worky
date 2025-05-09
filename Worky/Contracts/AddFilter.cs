namespace Worky.Contracts;

public record AddFilter(
    ulong id,
    List<ulong> typeOfActivity_id
    );