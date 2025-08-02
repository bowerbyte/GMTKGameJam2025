using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Project.Entities.Actions;
using Project.Entities.Items;
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
        public EntityItem[] ItemInventory { get; set; }
        public LevelManager LevelManager { get; set; }

        public bool Harvestable => EntityTypes.HasFlag(this.Type, EntityFlags.Harvestable);
        public bool Depositable => EntityTypes.HasFlag(this.Type, EntityFlags.Depositable);
        
        public bool HasInventory => ItemInventory != null && ItemInventory.Length > 0;

        public virtual void Initialize()
        {
            
        }

        public int AvailableItemCount()
        {
            if (!this.HasInventory) { return 0; }

            int count = 0;
            
            for (int i = 0; i < ItemInventory.Length; i++)
            {
                if (ItemInventory[i] != null) count++;
            }
            return count;
        }
        
        public int AvailableItemSlotCount()
        {
            var maxCount = HasInventory ? ItemInventory.Length : 0;
            return maxCount - AvailableItemCount();
        }
        
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
        
        public bool TryCast<T>(out T result) where T : LevelEntity
        {
            result = this as T;
            return result != null;
        }
        
        public static EntityItem TransferItemBetweenInventories(LevelEntity sourceEntity, LevelEntity destinationEntity)
        {
            EntityItem item = null;
            for (int i = 0; i < sourceEntity.ItemInventory.Length; i++)
            {
                if (sourceEntity.ItemInventory[i] != null)
                {
                    item = sourceEntity.ItemInventory[i];
                    sourceEntity.ItemInventory[i] = null;
                    break;
                }
            }
            
            for (int i = 0; i < destinationEntity.ItemInventory.Length; i++)
            {
                if (destinationEntity.ItemInventory[i] == null)
                {
                    destinationEntity.ItemInventory[i] = item;
                    break;
                }
            }

            return item;
        }
        

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
                case EntityType.Stockpile:
                    entity = LevelEntity.Create<StockpileEntity>();
                    break;
                default:
                    throw new NotImplementedException($"Entity creation for type {entityType} is not implemented");
            }

            entity.Type = entityType;
            // entity.Direction = GridDirection.North;
            entity.LevelManager = levelManager;
            entity.ItemInventory = null;
            
            entity.Initialize();
            
            entity.SetDirectionImmediate((GridDirection)UnityEngine.Random.Range(0, 3));

            return entity;
        }
    }
}