using UnityEngine;
using UnityEngine.Pool;

namespace Creotly_Studios
{
    public class ObjectHolder : MonoBehaviour
    {
        public ObjectPool<Tiles>[] tilePools;
        public ObjectPool<Tiles> backupTilePool;

        [Header("Objects To Spawn")]
        [SerializeField] private Tiles backup;
        [SerializeField] private Tiles[] tilesArray;
        
        public void Initialize(EnvironmentSetter environmentSetter)
        {
            tilePools = new ObjectPool<Tiles>[tilesArray.Length];
            backupTilePool = ObjectPooler.TilesPool(backup, backupTilePool);

            for(int i = 0; i < tilesArray.Length; i++)
            {
                Tiles newTile = tilesArray[i];
                ObjectPool<Tiles> tilePool = tilePools[i];
                tilePool = ObjectPooler.TilesPool(newTile, tilePool);

                for(int j = 0; j < environmentSetter.cellsList.Count; j++)
                {
                    Tiles tile = tilePool.Get();
                    tile.gameObject.SetActive(false);
                    Cells newCell = environmentSetter.cellsList[j];
                    newCell.InitializeTile(tile);
                }
            }
        }
    }
}
