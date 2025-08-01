using Project.Level;
using Unity.Mathematics;

namespace Project.Entities
{
    public interface IMovableEntity
    {
        public MoveRequest? GetMoveRequest();
        
        public void OnMoveFailed();


        public void MoveTo(TileLocation location);
    }

    public struct MoveRequest
    {
        public TileLocation source;
        public TileLocation destination;
    }
}