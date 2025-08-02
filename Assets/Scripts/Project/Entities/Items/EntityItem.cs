using UnityEngine;

namespace Project.Entities.Items
{
    public abstract class EntityItem : MonoBehaviour
    {
        public static T Create<T>() where T : EntityItem
        {
            string filename = typeof(T).Name.Replace("Item", "");
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/Items/{filename}");
            var instance = Instantiate(prefab);
            var component = instance.GetComponent<T>();
            return component;
        }
    }
}