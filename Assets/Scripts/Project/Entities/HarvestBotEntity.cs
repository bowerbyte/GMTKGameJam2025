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
            this.ItemInventory = new EntityItem[1];
        }
        
        public override EntityAction GetActionRequest()
        {
            var frontLocation = this.Location + GridDirections.GridDirectionToOffset(this.Direction);

            LevelEntity target;
            
            if (this.AvailableItemSlotCount() > 0 && this.LevelManager.TryGetEntityAt(frontLocation, out target))
            {
                if (target.Harvestable && target.AvailableItemCount() > 0)
                {
                    return new HarvestAction()
                    {
                        Actor = this,
                        targetLocation = frontLocation,
                    };
                }
            }
            else if (this.AvailableItemCount() > 0 && this.LevelManager.TryGetEntityAt(frontLocation, out target))
            {
                if (target.Depositable && target.AvailableItemSlotCount() > 0)
                {
                    return new DepositAction()
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
            else if (request.TryCast<DepositAction>(out var depositAction))
            {
                if (approved)
                {
                    DepositItem(depositAction.targetLocation);
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
            item.transform.localPosition = Vector3.zero + Vector3.up * targetEntity.AvailableItemCount() * 0.5f;
        }
        
        public void DepositItem(TileLocation targetLocation)
        {
            if (!this.LevelManager.TryGetEntityAt(targetLocation, out LevelEntity targetEntity))
            {
                throw new Exception($"Entity {targetLocation} not found");
            }
            
            var item = LevelEntity.TransferItemBetweenInventories(this, targetEntity);
            
            item.transform.SetParent(targetEntity.transform);
            item.transform.localPosition = Vector3.zero + Vector3.up * targetEntity.AvailableItemCount() * 0.5f;

            Debug.Log("Depositing Item");
        }

    }
}