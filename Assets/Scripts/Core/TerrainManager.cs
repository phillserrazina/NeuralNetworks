using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Coursework.Core
{
    public class TerrainManager : MonoBehaviour
    {
        // VARIABLES
        public static TerrainManager Instance { get; private set; }

        private Tile[] terrain = null;
        public Tile[] GetTilesOfType(TileTypes type) {
            return terrain.Where(t => t.TileType == type).ToArray();
        }
        public Tile GetTileAtCoord(Vector2 pos) {
            foreach (var tile in terrain) {
                if (tile.Position == pos) return tile;
            }

            return null;
        }

        private TerrainGenerator generator;

        // EXECUTION FUNCTIONS
        private void Awake() {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }

        // METHODS
        public void Initialize() {
            generator = GetComponent<TerrainGenerator>();
            terrain = generator.GenerateTerrain();

            var waterTiles = terrain.Where(tile => tile.TileType == TileTypes.WATER).ToArray();

            foreach (var tile in waterTiles) {
                foreach (var neighbour in GetNeighbours(tile)) {
                    if (neighbour.TileType == TileTypes.GRASS)
                        neighbour.TurnToCoast();
                }
            }
            
        }

        public Tile[] GetNeighbours(Tile target, int range = 1) {
            var neighbourList = new List<Tile>();

            var neighbours = new List<Tile>();

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    var currentTile = GetTileAtCoord(target.Position + new Vector2(x, y));

                    if (currentTile != target)
                        neighbours.Add(currentTile);
                }
            }

            foreach (var n in neighbours) {
                if (n != null) neighbourList.Add(n);
            }

            return neighbourList.ToArray();
        }

        public void ResetTiles() {
            foreach (var tile in terrain) {
                tile.ResetTile();
            }
        }

        public void ResetTileColors() {
            foreach (var tile in terrain) {
                tile.ResetColor();
            }
        }

        public Tile GetRandomTileOfType(TileTypes tileType) {
            var grassTiles = terrain.Where(t => t.TileType == tileType).ToArray();

            return grassTiles[Random.Range(0, grassTiles.Length)];
        }
    }
}
