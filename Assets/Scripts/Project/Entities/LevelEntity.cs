using System;
using JetBrains.Annotations;
using Project.Entities.Requests;
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

        [CanBeNull]
        public virtual EntityAction GetActionRequest() => null;

        public abstract void OnActionBegin(EntityAction request, bool approved);
        

        public static T Create<T>() where T : LevelEntity
        {
            string filename = typeof(T).Name.Replace("Entity", "");
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/Entities/{filename}");
            var instance = Instantiate(prefab);
            var component = instance.GetComponent<T>();
            return component;
        }

        public static LevelEntity Create(EntityType entityType, LevelManager levelManager)
        {
            LevelEntity entity;
            switch (entityType)
            {
                case EntityType.HarvestBot:
                    entity = LevelEntity.Create<HarvestBotEntity>();
                    break;
                case EntityType.Tree:
                    entity = LevelEntity.Create<TreeEntity>();
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