using Project.Level;

namespace Project.Entities.Requests
{
    public class MoveAction : EntityAction
    {
        public TileLocation source;
        public TileLocation destination;
    }
}