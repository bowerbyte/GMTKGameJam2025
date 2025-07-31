using Project.Core.Enums;
using Unity.Mathematics;

namespace Project.Level.Data
{
    public struct LevelData
    {
        public int2 Dimensions => new int2(tiles.GetLength(0), tiles.GetLength(1));
        
        
        public TileType[,] tiles;
        public EntityType[,] entities;


        public static LevelData GetEmptyLevelData(int2 dimensions)
        {
            var data = new LevelData()
            {
                tiles = new TileType[dimensions.x, dimensions.y],
                entities = new EntityType[dimensions.x, dimensions.y]
            };

            for (int i = 0; i < dimensions.x; i++)
            {
                for (int j = 0; j < dimensions.y; j++)
                {
                    data.tiles[i, j] = TileType.Grass;
                    data.entities[i, j] = EntityType.Empty;
                }
            }

            return data;
        }
    }
}