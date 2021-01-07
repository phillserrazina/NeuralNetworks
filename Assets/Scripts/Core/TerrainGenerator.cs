using UnityEngine;
using System.Collections.Generic;

namespace Coursework.Core
{
    public class TerrainGenerator : MonoBehaviour
    {
        // VARIABLES
        [Header("Object References")]
        // 0 = Grass
        // 1 = Water
        // 2 = Food
        [SerializeField] private Tile[] tiles = null;

        [Header("Settings")]
        [SerializeField] private int width = 256;
        [SerializeField] private int height = 256;

        [SerializeField] [Range(0f, 1f)] private float waterFrequency = 0.5f;
        [SerializeField] [Range(0f, 1f)] private float foodFrequency = 0.5f;

        // METHODS
        public Tile[] GenerateTerrain() {
            var finalTiles = new List<Tile>();

            for (int x = -width / 2; x < width / 2; x++)
            {
                for (int y = -height / 2; y < height / 2; y++)
                {
                    Tile tile = tiles[0];
                    if (Random.value < waterFrequency)
                        tile = tiles[1];
                    else if (Random.value < foodFrequency)
                        tile = tiles[2];
                    
                    tile = Instantiate(tile, new Vector3(x, 0f, y), Quaternion.identity, transform);
                    tile.Initialize(x, y);
                    finalTiles.Add(tile);
                }
            }

            StaticBatchingUtility.Combine(gameObject);
            return finalTiles.ToArray();
        }

        public Tile[] ResetTerrain() {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }

            return GenerateTerrain();
        }
    }
}
