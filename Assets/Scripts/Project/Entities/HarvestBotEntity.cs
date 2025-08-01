using Project.Entities.Requests;
using Project.Enums;
using Project.Level;
using UnityEngine;

namespace Project.Entities
{
    public class HarvestBotEntity: LevelEntity
    {
        public override EntityAction GetActionRequest()
        {
            var destination = this.Location + GridDirections.GridDirectionToOffset(this.Direction);
            return new MoveAction()
            {
                Actor = this,
                source = this.Location,
                destination = destination,
            };
        }

        public override void OnActionBegin(EntityAction request, bool approved)
        {
            if (request.TryCast<MoveAction>(out var moveAction))
            {
                if (approved)
                {
                    MoveTo(moveAction.destination);
                }
                else
                {
                    SetDirectionImmediate(GridDirections.FlipDirection(this.Direction));
                }
            }
        }
        
        public void MoveTo(TileLocation location)
        {
            var localPosition = this.LevelManager.LocationToBasePosition(location);
            this.SetLocalPositionImmediate(localPosition);
            this.Location = location;
        }

    }
}