using FileStorageApi.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApi.DAL.Context;

public sealed class EFContext : DbContext
{
    public DbSet<SystemFile> Files { get; set; }
    
    public EFContext(DbContextOptions<EFContext> options) : base(options) {}
    
    
}