using System;
using Project.Enums;
using Project.Level;
using Unity.Mathematics;
using UnityEngine;

namespace Project.Entities
{
    public abstract class LevelEntity : MonoBehaviour
    {
        public EntityType Type { get; private set; }
        public GridDirection Direction { get; protected set; }
        public TileLocation Location { get; set; }
        public LevelManager LevelManager { get; set; }

        public void SetLocalPositionImmediate(float3 position)
        {
            this.transform.localPosition = position;
        }

        public void SetDirectionImmediate(GridDirection direction)
        {
            this.Direction = direction;
            var offset = GridDirections.GridDirectionToOffset(direction);
            var worldOffset = new Vector3(offset.x, 0f, offset.y);
            this.transform.LookAt(this.transform.position + worldOffset);
        }

        public static LevelEntity CreateTest()
        {
            throw new NotImplementedException("This should be overriden in the base class (using new keyword)");
        }

        public static LevelEntity Create(EntityType entityType, LevelManager levelManager)
        {
            LevelEntity entity;
            switch (entityType)
            {
                case EntityType.HarvestBot:
                    entity = HarvestBotEntity.CreateTest();
                    break;
                default:
                    throw new NotImplementedException($"Entity creation for type {entityType} is not implemented");
            }

            entity.Type = entityType;
            // entity.Direction = GridDirection.North;
            entity.LevelManager = levelManager;
            
            entity.SetDirectionImmediate((GridDirection)UnityEngine.Random.Range(0, 3));

            return entity;
        }
    }
}