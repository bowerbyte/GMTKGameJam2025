using System;
using Project.Enums;
using Project.Level;
using Project.Level.Enums;
using Unity.Mathematics;
using UnityEngine;

namespace Project.Entities
{
    public abstract class LevelEntity : MonoBehaviour
    {
        public EntityType Type { get; private set; }
        public GridDirection Direction { get; private set; }
        public TilePosition Position { get; set; }
        public LevelManager LevelManager { get; set; }

        public void SetLocalPositionImmediate(float3 position)
        {
            this.transform.localPosition = position;
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
            entity.Direction = GridDirection.Forward;
            entity.LevelManager = levelManager;

            return entity;
        }
    }
}