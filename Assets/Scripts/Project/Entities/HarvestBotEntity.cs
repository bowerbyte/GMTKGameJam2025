using Project.Enums;
using Project.Level;
using UnityEngine;

namespace Project.Entities
{
    public class HarvestBotEntity: LevelEntity, IMovableEntity
    {
        public MoveRequest? GetMoveRequest()
        {
            var destination = this.Location + GridDirections.GridDirectionToOffset(this.Direction);
            return new MoveRequest()
            {
                source = this.Location,
                destination = destination,
            };
        }

        public void OnMoveFailed()
        {
            SetDirectionImmediate(GridDirections.FlipDirection(this.Direction));
        }

        public void MoveTo(TileLocation location)
        {
            var localPosition = this.LevelManager.LocationToBasePosition(location);
            this.SetLocalPositionImmediate(localPosition);
            this.Location = location;
        }

        public new static LevelEntity CreateTest()
        {
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/Entities/Bots/Harvest Bot");
            var instance = Object.Instantiate(prefab);
            var component = instance.GetComponent<HarvestBotEntity>();
            return component;
        }

    }
}