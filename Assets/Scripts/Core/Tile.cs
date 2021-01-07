using UnityEngine;

namespace Coursework.Core
{
    public enum TileTypes { GRASS, WATER, COAST, FOOD }

    public class Tile : MonoBehaviour
    {
        // VARIABLES
        private int x;
        private int y;

        public TileTypes TileType;
        [SerializeField] private float sensorValue = 0f;
        public void FindTile() => sensorValue /= 3;
        public float SensorValue { get { return sensorValue; } }

        public Vector2 Position { get { return new Vector2(x, y); } }

        private TileTypes initialTileType;
        private GameObject foodMesh;

        public int GCost;
        public int HCost;

        public float FCost { get { return GCost + HCost; } }

        public Tile Parent;

        private Color initialColor;

        // METHODS
        public void Initialize(int x, int y) {
            this.x = x;
            this.y = y;

            initialTileType = TileType;
            initialColor = GetComponent<Renderer>().material.color;

            if (transform.childCount > 0)
                foodMesh = transform.GetChild(0).gameObject;
        }

        public void TurnToGrass() {
            SetTileType(TileTypes.GRASS);
            foodMesh.SetActive(false);
        }

        public void TurnToCoast() {
            SetTileType(TileTypes.COAST);
            GetComponent<Renderer>().material.color = Color.yellow;
            initialColor = Color.yellow;
        }

        public void ResetTile() {
            TileType = initialTileType;

            if (TileType == TileTypes.FOOD) {
                foodMesh.SetActive(true);
            }
        }

        public void ResetColor() {
            GetComponent<Renderer>().material.color = initialColor;
        }

        public void SetTileType(TileTypes t) {
            TileType = t;
            initialTileType = t;
        }
    }
}

