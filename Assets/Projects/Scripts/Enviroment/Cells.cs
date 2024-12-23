using UnityEngine;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class Cells : MonoBehaviour
    {
        [Header("Status")]
        public bool hasCollapsed;
        public Vector2 cellDimensions;

        [Header("Tiles")]
        public Tiles selectedTile;
        private List<Tiles> permanentTileList = new();
        public List<Tiles> temporaryTilesList = new();
        [SerializeField] private Transform spawnPoint;

        [field: Header("Neighbors")]
        public Cells UpCell {get; private set;}
        public Cells DownCell {get; private set;}
        public Cells LeftCell {get; private set;}
        public Cells RightCell {get; private set;}
        public List<Cells> neighboringCells = new();
        public int totalNeighborEntropies {get; private set;}

        public void InitializeTile(Tiles tile)
        {
            tile.canEnableObjects = false;

            tile.cellHolder = this;
            tile.transform.SetParent(spawnPoint);
            tile.transform.localPosition = Vector3.zero;

            temporaryTilesList.Add(tile);
        }

        public void ResetCell()
        {
            hasCollapsed = false;
            temporaryTilesList.Clear();
            temporaryTilesList.AddRange(permanentTileList);
            selectedTile.ReleaseFromPool();
        }

        public void AddTile(Tiles tiles)
        {
            temporaryTilesList.Clear();
            temporaryTilesList.Add(tiles);
        }

        public void CreateCell(float x, float y, bool collapsed, List<Tiles> tilesOptions)
        {
            cellDimensions.x = x;
            cellDimensions.y = y;
            hasCollapsed = collapsed;
            temporaryTilesList.Clear();
            temporaryTilesList.AddRange(tilesOptions);
        }

        public void GetNeighboringCells(List<Cells> cellsList)
        {
            UpCell = GetCell(cellDimensions.y + 1, cellDimensions.x, cellsList, TileNeighbors.Top);
            LeftCell = GetCell(cellDimensions.x - 1, cellDimensions.y, cellsList, TileNeighbors.Left);
            RightCell = GetCell(cellDimensions.x + 1, cellDimensions.y, cellsList, TileNeighbors.Right);
            DownCell = GetCell(cellDimensions.y - 1, cellDimensions.x, cellsList, TileNeighbors.Bottom);
        }

        private Cells GetCell(float searchIndex, float cellDimensionToCheck, List<Cells> cellsList, TileNeighbors tn)
        {
            int index = Mathf.RoundToInt(searchIndex);
            int cellIndex = Mathf.RoundToInt(cellDimensionToCheck);

            Cells neighbor = cellsList.Find(cell => cell.CellDimension(tn) == cellDimensionToCheck &&
                 cell.OppositeCellDimension(tn) == searchIndex);
            
            if(neighbor != null)
            {
                neighboringCells.Add(neighbor);
            }
            return neighbor;
        }

        public int CellDimension(TileNeighbors tn)
        {
            if(tn == TileNeighbors.Left || tn == TileNeighbors.Right)
            {
                return Mathf.RoundToInt(cellDimensions.y);
            }
            return Mathf.RoundToInt(cellDimensions.x);
        }

        public int OppositeCellDimension(TileNeighbors tn)
        {
            if(tn == TileNeighbors.Left || tn == TileNeighbors.Right)
            {
                return Mathf.RoundToInt(cellDimensions.x);
            }
            return Mathf.RoundToInt(cellDimensions.y);
        }

        public void RecreateCell(List<Tiles> tilesOptions)
        {
            temporaryTilesList.Clear();
            temporaryTilesList.AddRange(tilesOptions);

            totalNeighborEntropies = 0;
            foreach(Cells neighbor in neighboringCells)
            {
                totalNeighborEntropies += neighbor.temporaryTilesList.Count;
            }
        }

        public void EnableTile()
        {
            GameObjectTools.InitializeObject(Time.deltaTime, selectedTile, spawnPoint, Vector3.zero);
        }
    }
}
