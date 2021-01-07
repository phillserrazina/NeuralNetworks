using UnityEngine;
using System.Collections.Generic;
using Coursework.Core;

namespace Coursework.Utils
{
    public class SearchAlgorithms : MonoBehaviour
    {
        // Path-finding algorithm (A*) by https://www.youtube.com/watch?v=AKKpPmxx07w&t=994s
        public static Queue<Tile> GetPath(Tile startPosition, Tile targetPosition, bool drawPath=false) {
            var openList = new List<Tile>();
            var closedList = new HashSet<Tile>();

            openList.Add(startPosition);

            while (openList.Count > 0)
            {
                Tile currentTile = openList[0];

                for (int i = 1; i < openList.Count; i++) 
                {
                    if (openList[i].FCost < currentTile.FCost || openList[i].FCost == currentTile.FCost && openList[i].HCost < currentTile.HCost)
                    {
                        currentTile = openList[i];
                    }
                }

                openList.Remove(currentTile);
                closedList.Add(currentTile);

                if (currentTile == targetPosition) {
                    return GetFinalPath(startPosition, targetPosition, drawPath);
                }

                foreach (var neighbour in GetNeighbourTiles(currentTile)) {
                    if (neighbour.TileType == TileTypes.WATER || closedList.Contains(neighbour)) {
                        continue;
                    }

                    int moveCost = currentTile.GCost + GetManhattenDistance(currentTile, neighbour);
                
                    if (moveCost < neighbour.GCost || !openList.Contains(neighbour)) {
                        neighbour.GCost = moveCost;
                        neighbour.HCost = GetManhattenDistance(neighbour, targetPosition);
                        neighbour.Parent = currentTile;

                        if (!openList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                        }
                    }
                }
            }

            Debug.LogWarning($"SearchAlgorithms::GetPath() --- No Path Found to Reach {targetPosition.gameObject.name}! Ignoring Request.");
            return null;
        }

        private static Queue<Tile> GetFinalPath(Tile startTile, Tile targetTile, bool drawPath = false) {
            var finalPathList = new List<Tile>();
            var finalPathQueue = new Queue<Tile>();
            Tile currentTile = targetTile;

            while (currentTile != startTile) {
                finalPathList.Add(currentTile);
                currentTile = currentTile.Parent;
            }

            finalPathList.Reverse();
            
            foreach (var tile in finalPathList) {
                if (drawPath)
                    tile.GetComponent<Renderer>().material.color = Color.red;
                finalPathQueue.Enqueue(tile);
            }

            return finalPathQueue;
        }

        private static int GetManhattenDistance(Tile tileA, Tile tileB)
        {
            int ix = Mathf.Abs((int)tileA.Position.x - (int)tileB.Position.x);
            int iy = Mathf.Abs((int)tileA.Position.y - (int)tileB.Position.y);

            return ix + iy;
        }

        private static Tile[] GetNeighbourTiles(Tile tile) {
            return TerrainManager.Instance.GetNeighbours(tile);
        }

        public static Tile GetClosestTile(Tile CurrentPosition, Tile[] tileArray) {
            if (tileArray.Length == 0) return null;
            if (tileArray.Length == 1) return tileArray[0];

            Tile closest = tileArray[0];
            int dist = 100000;

            foreach (var tile in tileArray) {
                var path = GetPath(CurrentPosition, tile);

                if (path == null) continue;

                if (path.Count < dist) {
                    closest = tile;
                    dist = path.Count;
                }
            }

            return closest;
        }
    }
}
