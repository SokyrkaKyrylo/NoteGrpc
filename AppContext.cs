using Microsoft.EntityFrameworkCore;
using NoteGrpc.Models;

namespace NoteGrpc;

public class AppContext : DbContext
{
    public DbSet<NoteEntity> Notes => Set<NoteEntity>();
    
    public AppContext(DbContextOptions<AppContext> options) : base(options)
    { }
}