using System;
using Project.Entities.Actions;
using Project.Entities.Items;
using Project.Enums;
using Project.Level;
using Unity.VisualScripting;
using UnityEngine;

namespace Project.Entities
{
    public class HarvestBotEntity: LevelEntity
    {
        public override void Initialize()
        {
            base.Initialize();
            this.Harvestable = true;
            this.ItemInventory = new EntityItem[1];
        }
        
        public override EntityAction GetActionRequest()
        {
            var frontLocation = this.Location + GridDirections.GridDirectionToOffset(this.Direction);
            
            if (this.AvailableItemSlotCount() > 0 && this.LevelManager.TryGetEntityAt(frontLocation, out LevelEntity target))
            {
                if (target.AvailableItemCount() > 0)
                {
                    return new HarvestAction()
                    {
                        Actor = this,
                        targetLocation = frontLocation,
                    };
                }
            }
            
            return new MoveAction()
            {
                Actor = this,
                source = this.Location,
                destination = frontLocation,
            };
        }

        public override void OnActionBegin(EntityAction request, bool approved)
        {
            if (request.TryCast<MoveAction>(out var moveAction))
            {
                if (approved)
                {
                    MoveTo(moveAction.destination);
                    this.LevelManager.OnEntityMoved(moveAction.source, moveAction.destination);
                }
                else
                {
                    SetDirectionImmediate(GridDirections.FlipDirection(this.Direction));
                }
            }
            else if (request.TryCast<HarvestAction>(out var harvestAction))
            {
                if (approved)
                {
                    HarvestItem(harvestAction.targetLocation);
                }
            }
            else
            {
                throw new NotImplementedException($"Action not implemented for {this.GetType()}");
            }
        }
        
        public void MoveTo(TileLocation location)
        {
            var localPosition = this.LevelManager.LocationToBasePosition(location);
            this.SetLocalPositionImmediate(localPosition);
            this.Location = location;
        }

        public void HarvestItem(TileLocation targetLocation)
        {
            if (!this.LevelManager.TryGetEntityAt(targetLocation, out LevelEntity targetEntity))
            {
                throw new Exception($"Entity {targetLocation} not found");
            }
            
            var item = LevelEntity.TransferItemBetweenInventories(targetEntity, this);
            
            item.transform.SetParent(this.transform);
            item.transform.localPosition = Vector3.zero + Vector3.up * 1;

            Debug.Log("Harvesting Item");
        }

    }
}