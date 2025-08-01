using Project.Level;
using Unity.Mathematics;

namespace Project.Entities
{
    public interface IMovableEntity
    {
        public MoveRequest? GetMoveRequest(LevelManager level);


        public void MoveTo(float3 localPosition);
    }

    public struct MoveRequest
    {
        public int entityId;
        public TilePosition destination;
    }
}