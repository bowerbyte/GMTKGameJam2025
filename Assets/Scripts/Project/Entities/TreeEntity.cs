using Project.Entities.Actions;
using Project.Entities.Items;
using UnityEngine;

namespace Project.Entities
{
    public class TreeEntity : LevelEntity
    {
        public override void Initialize()
        {
            base.Initialize();
            this.Harvestable = true;
            this.ItemInventory = new EntityItem[4];
            for (int i = 0; i < this.ItemInventory.Length; i++)
            {
                var fruit = EntityItem.Create<FruitItem>();
                this.ItemInventory[i] = fruit;
                
                fruit.transform.SetParent(this.transform);
                fruit.transform.localPosition = Vector3.zero + Vector3.up * (i + 3);
            }
        }


        public override EntityAction GetActionRequest()
        {
            return null; // no actions
        }

        public override void OnActionBegin(EntityAction request, bool approved)
        {
            // do nothing...
        }
        
    }
}