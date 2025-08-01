namespace Project.Entities.Requests
{
    public abstract class EntityAction
    {
        public LevelEntity Actor { get; set; }


        
        public bool TryCast<T>(out T result) where T : EntityAction
        {
            result = this as T;
            return result != null;
        }
    }
}