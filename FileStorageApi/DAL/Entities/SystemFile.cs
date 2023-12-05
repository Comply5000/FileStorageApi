namespace FileStorageApi.DAL.Entities;

public sealed class SystemFile
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
    public byte[] Data { get; set; }
    public long TotalBytes { get; set; }
}