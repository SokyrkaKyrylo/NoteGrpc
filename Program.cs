using Microsoft.EntityFrameworkCore;
using NoteGrpc.Services;
using AppContext = NoteGrpc.AppContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppContext>(options => options.UseSqlite("DataSource=note.dbo"));
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<NoteService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. " +
                      "To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();