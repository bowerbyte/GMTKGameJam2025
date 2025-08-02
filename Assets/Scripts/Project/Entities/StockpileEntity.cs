using Project.Entities.Actions;
using Project.Entities.Items;
using UnityEngine;

namespace Project.Entities
{
    public class StockpileEntity : LevelEntity
    {
        public override void Initialize()
        {
            base.Initialize();
            this.ItemInventory = new EntityItem[4];
            
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