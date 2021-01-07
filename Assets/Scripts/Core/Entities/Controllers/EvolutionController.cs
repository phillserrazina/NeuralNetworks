using UnityEngine;
using System.Collections.Generic;
using Coursework.Utils;
using System.Linq;

namespace Coursework.Core.Entities.Controllers
{
    public class EvolutionController : EntityController
    {
        // VARIABLES
        private enum States { EnRoute, Roaming, Running, Action, Waiting }

        [SerializeField] private bool drawCurrentPath = false;

        private States currentState;

        private Queue<Tile> currentPath = new Queue<Tile>();
        private List<Tile> tilesToClear = new List<Tile>();
        private Tile currentTarget = null;

        private string currentNecessity = "";

        // EXECUTION FUNCTIONS
        private void Update() {
            if (ReadyToMove()) {
                currentState = Evaluate();

                Vector2 direction = GetInput();

                Move(direction.x, direction.y);
            }
        }

        private void OnTriggerEnter(Collider other) {
            Tile tile = other.GetComponent<Tile>();
            if (tile == null) return;

            if (tile.TileType == TileTypes.WATER) {
                manager.Kill();
            }
        }

        // METHODS
        private States Evaluate() {
            string currentNecessity = manager.Stats.CurrentNecessity;

            switch (currentNecessity)
            {
                case "Hunger": return HungerEval();
                case "Thirst": return ThirstEval();
                case "Mating": return MatingEval();

                default:
                    Debug.LogError("EvolutionController::Evaluate() --- Invalid Necessity");
                    return RandomEval();
            }
        }
        
        private Vector2 GetInput() {
            switch (currentState)
            {
                case States.EnRoute: return TargetMovement();
                case States.Roaming: return RoamingMovement();
                case States.Running: return Vector2.zero;
                case States.Action: return PerformAction();
                case States.Waiting: return Vector2.zero;
                
                default:
                    return RoamingMovement();
            }
            
        }

        public override void Kill() {
            Destroy(gameObject);
        }

        public override void ResetController() {
            manager.Movement.MoveToPointInstant(startPosition);
            ClearPath();
        }

        #region Evaluation Types

        private States HungerEval() {
            currentNecessity = "Hunger";

            if (manager.Movement.CurrentPosition.TileType == TileTypes.FOOD) {
                return States.Action;
            }

            var surroundings = manager.Movement.GetNeighboursDictionary(manager.Stats.Genes.SensoryDistance);
            int numOfFood = surroundings.ContainsKey(TileTypes.FOOD) ? 
                                surroundings[TileTypes.FOOD].Count : 
                                0;

            if (numOfFood > 0) {
                ClearPath();

                Tile closest = SearchAlgorithms.GetClosestTile(manager.Movement.CurrentPosition, surroundings[TileTypes.FOOD].ToArray());
                bool successful = SetTarget(closest);

                if (successful) return States.EnRoute;

                int attempts = 0;

                while (!successful) {
                    Tile randomTile = TerrainManager.Instance.GetRandomTileOfType(TileTypes.GRASS);
                    successful = SetTarget(randomTile);
                    attempts++;
                    if (attempts > 50) return States.Roaming;
                }
            }

            return States.Roaming;
        }

        private States ThirstEval() {
            currentNecessity = "Thirst";

            if (manager.Movement.CurrentPosition.TileType == TileTypes.COAST) {
                return States.Action;
            }

            var surroundings = manager.Movement.GetNeighboursDictionary(manager.Stats.Genes.SensoryDistance);
            int numOfWater = surroundings.ContainsKey(TileTypes.COAST) ? 
                                surroundings[TileTypes.COAST].Count : 
                                0;

            if (numOfWater > 0) {
                ClearPath();

                Tile closest = SearchAlgorithms.GetClosestTile(manager.Movement.CurrentPosition, surroundings[TileTypes.COAST].ToArray());
                bool successful = SetTarget(closest);

                if (successful) return States.EnRoute;

                int attempts = 0;

                while (!successful) {
                    Tile randomTile = TerrainManager.Instance.GetRandomTileOfType(TileTypes.GRASS);
                    successful = SetTarget(randomTile);
                    attempts++;
                    if (attempts > 50) return States.Roaming;
                }
            }

            return States.Roaming;
        }

        private EntityManager matingTarget = null;
        public void SetMatingTarget(EntityManager other) => matingTarget = other;


        private States MatingEval() {
            currentNecessity = "Mating";

            if (matingTarget != null) {
                if ((matingTarget.Movement.CurrentPosition.Position 
                        - matingTarget.Movement.CurrentPosition.Position).magnitude < Mathf.Sqrt(2f))
                {
                    return States.Action;
                }

                else return States.EnRoute;
            }

            if (manager.EntitiesInRange.Count <= 0) {
                return CheckNext("Mating");
            }
            
            else {
                var bestOption = GetBestMatingOption();
                if (bestOption == null) return CheckNext("Mating");

                bool accepted = bestOption.RequestMate(manager);

                if (accepted) {
                    matingTarget = bestOption;
                    SetTarget(matingTarget.Movement.CurrentPosition);
                    currentPath.Dequeue();

                    return States.EnRoute;
                }
                
                return CheckNext("Mating");
            }
        }

        private States RandomEval() {
            bool successfull = false;
            int attempts = 0;

            while (!successfull) {
                Tile randomTile = TerrainManager.Instance.GetRandomTileOfType(TileTypes.GRASS);
                successfull = SetTarget(randomTile);
                attempts++;
                if (attempts > 50)
                    return States.Roaming;
            }

            return States.EnRoute;
        }

        #endregion

        #region Movement Types

        private Vector2 RoamingMovement() {
            var neighbours = manager.Movement.GetNeighboursDictionary(1);

            if (neighbours.ContainsKey(TileTypes.GRASS)) {
                var grassTiles = neighbours[TileTypes.GRASS];

                var chosen = grassTiles[Random.Range(0, grassTiles.Count)];
                var pos = manager.Movement.CurrentPosition.Position - chosen.Position;

                return pos;
            }

            return Vector2.zero;
        }

        private Vector2 TargetMovement() {
            if (currentPath.Count > 0) {
                Tile next = currentPath.Dequeue();
                
                if (drawCurrentPath) {
                    next.ResetColor();
                    tilesToClear.Remove(next);
                }

                return manager.Movement.CurrentPosition.Position - next.Position;
            }
                
            ClearPath();
            return Vector2.zero;
        }

        private Vector2 PerformAction() {
            if (currentNecessity == "Thirst" && 
                manager.Movement.CurrentPosition.TileType == TileTypes.COAST) {
                manager.Stats.Restore("Thirst", 20);
            }

            if (currentNecessity == "Hunger" && 
                manager.Movement.CurrentPosition.TileType == TileTypes.FOOD) {
                manager.Stats.Restore("Hunger", 10);
                manager.Movement.CurrentPosition.TurnToGrass();
            }

            if (currentNecessity == "Mating" && 
                manager.EntitiesInRange.Count > 0) {
                    // TODO: This
            }

            return Vector2.zero;
        }

        #endregion

        private bool SetTarget(Tile target) {
            var path = SearchAlgorithms.GetPath(manager.Movement.CurrentPosition, target, drawCurrentPath);
            if (path == null) return false;

            tilesToClear = new List<Tile>(path);
            currentPath = new Queue<Tile>(path);
            return true;
        }

        private void ClearPath() {
            if (!drawCurrentPath) return;

            foreach (Tile tile in tilesToClear) {
                tile.ResetColor();
            }

            tilesToClear.Clear();
            currentPath.Clear();
        }

        private EntityManager GetBestMatingOption() {
            var oppositeGenderEntities = manager.EntitiesInRange.Where(ent => 
                ent.Stats.Gender == (manager.Stats.Gender == "Male" ? "Female" : "Male")).ToArray();

            if (oppositeGenderEntities.Length <= 0) return null;

            EntityManager bestOption = oppositeGenderEntities[0];

            foreach (var entity in oppositeGenderEntities) {
                if (entity.Stats.Genes.Desirability > bestOption.Stats.Genes.Desirability) {
                    bestOption = entity;
                }
            }

            return bestOption;
        }

        private States CheckNext(string current) {
            float hunger = manager.Stats.Percentage("Hunger");
            float thirst = manager.Stats.Percentage("Thirst");
            float mating = manager.Stats.Percentage("Mating");

            switch (current)
            {
                case "Hunger":
                    if (mating > thirst) {
                        return MatingEval();
                    }
                    else return ThirstEval();

                case "Thirst":
                    if (mating > hunger) {
                        return MatingEval();
                    }
                    else return HungerEval();
                
                case "Mating":
                    if (thirst > hunger) {
                        return ThirstEval();
                    }
                    else return HungerEval();
                
                default:
                    Debug.Log("Invalid State");
                    return States.Roaming;
            }
        }

    }
}
