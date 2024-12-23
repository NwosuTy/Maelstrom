using UnityEngine;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class EnvironmentSetter : MonoBehaviour
    {
        Vector3 spacing;
        Tiles selectedTile;
        private Sentient sentient;
        private ObjectHolder objectHolder;

        //Private Lists
        private List<Tiles> validTilesList = new();
        private List<Cells> temporaryCellList = new();
        private List<Cells> temporaryNeighborCellList = new();

        [Header("Parameters")]
        [SerializeField] private int dimensions;
        [SerializeField] private Vector2 spacingParameter;

        [Header("Cells")]
        public Cells[,] cellArray;
        [SerializeField] private Cells cellObject;
        public List<Cells> cellsList = new List<Cells>();

        [Header("Spawn Points")]
        [SerializeField] private Transform cellsSpawnPoint;

        public void ResetEnvironment()
        {
            foreach(Cells cells in cellsList)
            {
                cells.ResetCell();
            }
        }

        public void InitializeGrid(Sentient snt)
        {
            if(sentient == null)
            {
                sentient = snt;
            }
            objectHolder = GetComponent<ObjectHolder>();

            for(int x = 0; x < dimensions; x++)
            {
                for(int y = 0; y < dimensions; y++)
                {
                    spacing = new Vector3(x * spacingParameter.x, 0.0f, y * spacingParameter.y);

                    Cells newCell = Instantiate(cellObject, cellsSpawnPoint);

                    newCell.CreateCell(x, y, false, new());
                    newCell.transform.SetLocalPositionAndRotation(spacing, Quaternion.identity);
                    cellsList.Add(newCell);
                    newCell.gameObject.name = $"New Cell {cellsList.IndexOf(newCell)}";
                }
            }
            objectHolder.Initialize(this);

            for(int i = 0; i < cellsList.Count; i++)
            {
                cellsList[i].GetNeighboringCells(cellsList);
            }
        }

        public void EnvironmentGeneration(Cells cellToCollapse)
        {
            temporaryCellList.Clear();
            temporaryCellList.Add(cellToCollapse);

            CollapseCell();
        }

        private void CollapseCell()
        {
            int random = Random.Range(0, temporaryCellList.Count);
            Cells cellToCollapse = temporaryCellList[random];
            try
            {
                int randomTile = Random.Range(0, cellToCollapse.temporaryTilesList.Count);
                selectedTile = cellToCollapse.temporaryTilesList[randomTile];
            }
            catch
            {
                selectedTile = objectHolder.backupTilePool.Get();
            }

            cellToCollapse.selectedTile = selectedTile;
            cellToCollapse.EnableTile();
            
            cellToCollapse.hasCollapsed = true;
            UpdateGeneratedCells(cellToCollapse);
        }

        private void UpdateGeneratedCells(Cells collapsedCell)
        {
            temporaryCellList.Clear();
            temporaryCellList.AddRange(collapsedCell.neighboringCells);

            foreach(Cells cell in temporaryCellList)
            {
                if(cell.hasCollapsed)
                {
                    continue;
                }

                if(cell.UpCell != null && cell.UpCell.hasCollapsed)
                {
                    float oppositeBoundary = cell.UpCell.selectedTile.bottomBoundary;
                    ValidityCheck(cell, oppositeBoundary, TileNeighbors.Top);
                }

                if(cell.DownCell != null && cell.DownCell.hasCollapsed)
                {
                    float oppositeBoundary = cell.DownCell.selectedTile.topBoundary;
                    ValidityCheck(cell, oppositeBoundary, TileNeighbors.Bottom);
                }

                if(cell.LeftCell != null && cell.LeftCell.hasCollapsed)
                {
                    float oppositeBoundary = cell.LeftCell.selectedTile.rightBoundary;
                    ValidityCheck(cell, oppositeBoundary, TileNeighbors.Left);
                }

                if(cell.RightCell != null && cell.RightCell.hasCollapsed)
                {
                    float oppositeBoundary = cell.RightCell.selectedTile.leftBoundary;
                    ValidityCheck(cell, oppositeBoundary, TileNeighbors.Right);
                }
            }
            CheckEntropy(collapsedCell);
        }

        private void CheckEntropy(Cells cellToCollapse)
        {
            temporaryCellList.Clear();
            temporaryCellList.AddRange(cellToCollapse.neighboringCells);
            temporaryCellList.RemoveAll(cell => cell.hasCollapsed == true);

            if(temporaryCellList.Count == 0)
            {
                MoveToNextNeighboringCell(cellToCollapse);
            }

            if(temporaryCellList.Count != 0)
            {
                temporaryCellList.Sort((cellA,cellB) => cellA.temporaryTilesList.Count - cellB.temporaryTilesList.Count);
                temporaryCellList.RemoveAll(cell => cell.temporaryTilesList.Count != temporaryCellList[0].temporaryTilesList.Count);
                Invoke(nameof(CollapseCell), 0.125f);
            }
        }

        private void MoveToNextNeighboringCell(Cells cellToCollapse)
        {
            temporaryCellList.AddRange(cellToCollapse.neighboringCells);
            temporaryCellList.Sort((cellA, cellB) => cellA.totalNeighborEntropies - cellB.totalNeighborEntropies);

            int random = Random.Range(0, temporaryCellList.Count);

            Cells newCellToCollapse = temporaryCellList[random];
            temporaryCellList.Clear();
            temporaryCellList.AddRange(newCellToCollapse.neighboringCells);
            temporaryCellList.RemoveAll(cells => cells.hasCollapsed == true);

            if(temporaryCellList.Count == 0)
            {
                FindNewCellToCollapse();
            }
        }

        private void FindNewCellToCollapse()
        {
            temporaryCellList.Clear();
            temporaryCellList.AddRange(cellsList);
            temporaryCellList.RemoveAll(cells => cells.hasCollapsed);
        }

        private void ValidityCheck(Cells cell, float oppositeBoundary, TileNeighbors tn)
        {
            validTilesList.Clear();
            foreach(Tiles tiles in cell.temporaryTilesList)
            {
                float boundaryCheck = ReturnBoundaryCheck(tiles, tn);

                if(boundaryCheck == oppositeBoundary)
                {
                    validTilesList.Add(tiles);
                }
            }
            cell.RecreateCell(validTilesList);
        }

        private float ReturnBoundaryCheck(Tiles tiles, TileNeighbors tn)
        {
            if(tn == TileNeighbors.Top)
            {
                return tiles.topBoundary;
            }
            if(tn == TileNeighbors.Left)
            {
                return tiles.leftBoundary;
            }
            if(tn == TileNeighbors.Right)
            {
                return tiles.rightBoundary;
            }
            return tiles.bottomBoundary;
        }
    }
}
