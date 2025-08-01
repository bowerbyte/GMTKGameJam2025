using Mono.Cecil;
using Project.Enums;
using Project.Level;
using Unity.Mathematics;
using UnityEngine;

namespace Project.Entities
{
    public class HarvestBotEntity: LevelEntity, IMovableEntity
    {
        public MoveRequest? GetMoveRequest(LevelManager level)
        {
            return new MoveRequest()
            {
                destination = this.Position + GridDirections.GridDirectionToOffset(this.Direction),
            };
        }

        public void MoveTo(float3 localPosition)
        {
            this.SetLocalPositionImmediate(localPosition);
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