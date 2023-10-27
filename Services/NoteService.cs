using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using NoteGrpc.Models;

namespace NoteGrpc.Services;

public class NoteService : Note.NoteBase
{
    private readonly AppContext _dbContext;

    public NoteService(AppContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<GetAllResponse> ListNotes(GetAllRequest request, ServerCallContext context) =>
        new()
        {
            Note =
            {
                await _dbContext.Notes
                    .Select(noteEntity => new ReadNoteResponse
                    {
                        Id = noteEntity.Id,
                        Title = noteEntity.Title,
                        Description = noteEntity.Description
                    })
                    .ToListAsync()
            }
        };

    public override async Task<CreateNoteResponse> CreateNote(CreateNoteRequest request, ServerCallContext context)
    {
        if (request.Title == string.Empty || request.Description == string.Empty)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

        var noteEntity = new NoteEntity
        {
            Title = request.Title,
            Description = request.Description
        };

        _dbContext.Notes.Add(noteEntity);

        await _dbContext.SaveChangesAsync();

        return new CreateNoteResponse { Id = noteEntity.Id };
    }

    public override async Task<ReadNoteResponse> ReadNote(ReadNoteRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index must be greater than 0"));

        var noteEntity = await _dbContext.Notes.FindAsync(request.Id);

        if (noteEntity == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Note not found"));

        return new ReadNoteResponse
        {
            Id = noteEntity.Id,
            Title = noteEntity.Title,
            Description = noteEntity.Description
        };
    }

    public override async Task<UpdateNoteResponse> UpdateNotes(UpdateNoteRequest request, ServerCallContext context)
    {
        if (request.Id <= 0 || request.Title == string.Empty || request.Description == string.Empty)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

        var toDoItem = await _dbContext.Notes.FirstOrDefaultAsync(t => t.Id == request.Id);

        if (toDoItem == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"No Note with Id {request.Id}"));

        toDoItem.Title = request.Title;
        toDoItem.Description = request.Description;

        await _dbContext.SaveChangesAsync();

        return new UpdateNoteResponse { Id = toDoItem.Id };
    }

    public override async Task<DeleteNoteResponse> DeleteNote(DeleteNoteRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index must be greater than 0"));

        var noteItem = await _dbContext.Notes.FirstOrDefaultAsync(n => n.Id == request.Id);

        if (noteItem is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"No Note with Id {request.Id}"));

        _dbContext.Remove(noteItem);

        await _dbContext.SaveChangesAsync();

        return new DeleteNoteResponse { Id = noteItem.Id };
    }
}