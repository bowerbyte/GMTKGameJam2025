using Project.Entities.Requests;
using Project.Enums;
using Project.Level;
using UnityEngine;

namespace Project.Entities
{
    public class TreeEntity : LevelEntity
    {
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