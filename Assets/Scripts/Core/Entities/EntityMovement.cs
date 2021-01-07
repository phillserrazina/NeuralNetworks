using UnityEngine;
using System.Collections.Generic;

namespace Coursework.Core.Entities
{
    public class EntityMovement : MonoBehaviour
    {
        // VARIABLES
        [SerializeField] [Range(0f, 10f)] private float animationSpeed = 1f;

        public Tile CurrentPosition { get; private set; }
        private Vector3 currentWorldPos = Vector3.zero;

        private EntityManager manager;

        [SerializeField] private bool debug = false;

        // EXECUTION FUNCTIONS
        private void Update() {
            if (debug) {
                transform.position = currentWorldPos;
                return;
            }

            UpdateAnimations();
        }

        // METHODS
        public void Initialize(EntityManager manager, Tile startPosition) {
            this.manager = manager;
            CurrentPosition = startPosition;
            currentWorldPos = GetWorldPosition(CurrentPosition.Position);
            transform.position = currentWorldPos;
        }

        public Tile MoveToPoint(Tile t) {
            if (t == null) return null;

            CurrentPosition = t;
            currentWorldPos = GetWorldPosition(CurrentPosition.Position);
            return CurrentPosition;
        }

        public void MoveToPointInstant(Tile t) {
            CurrentPosition = t;
            currentWorldPos = GetWorldPosition(CurrentPosition.Position);
            transform.position = currentWorldPos;
        }

        private Vector3 GetWorldPosition(Vector2 tilePos) {
            return new Vector3(tilePos.x, 0.7f, tilePos.y);
        }

        private void UpdateAnimations() {
            if (Vector3.Distance(transform.position, currentWorldPos) < 0.1) return;

            transform.position = Vector3.Slerp(transform.position, currentWorldPos, Time.deltaTime * animationSpeed);

            Quaternion targetRotation = Quaternion.LookRotation(currentWorldPos - transform.position);
       
            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, animationSpeed * 5f * Time.deltaTime);
        }

        public Tile[] GetNeighbours(int range = 1) { return TerrainManager.Instance.GetNeighbours(CurrentPosition, range); }
        public Tile GetNeighbour(int x, int y) {
            Vector2 pos = CurrentPosition.Position - new Vector2(x, y);
            return TerrainManager.Instance.GetTileAtCoord(pos);
        }

        public Dictionary<TileTypes, List<Tile>> GetNeighboursDictionary(int range) { 
            var neighbours = TerrainManager.Instance.GetNeighbours(CurrentPosition, range);
            var answer = new Dictionary<TileTypes, List<Tile>>();

            foreach (var n in neighbours) {
                if (!answer.ContainsKey(n.TileType))
                    answer[n.TileType] = new List<Tile>();
                
                answer[n.TileType].Add(n);
            }

            return answer;
        }


        public void Reset() {
            
        }
    }
}
