using UnityEngine;
using System.Collections.Generic;
using Coursework.Utils;
using UnityEngine.UI;

namespace Coursework.Core.Entities
{
    public class NNetController : EntityController
    {
        // VARIABLES
        [SerializeField] private Text fitnessText = null;
        private float timeSinceStart = 0f;
        private Tile target = null;
        private float initialDistanceToTarget = 0f;

        [Header("Fitness")]
        [SerializeField] private float overallFitness;
        /*[SerializeField] private float aliveTimeMultiplier = 1.4f;
        [SerializeField] private float hungerMultiplier = 0.2f;
        [SerializeField] private float sensorMultiplier = 0.1f;
        [SerializeField] private float foodEatenMultiplier = 0.2f;
        [SerializeField] private float visitedTilesMultiplier = 0.5f;*/
        [SerializeField] private float distanceToTargetMultiplier = 0.5f;
        [SerializeField] private float visitedTilesMultiplier = 0.1f;
        [SerializeField] private float aliveTimeMultiplier = 0.6f;

        [Header("Network Options")]
        [SerializeField] private int layers = 1;
        [SerializeField] private int neurons = 10;
        public int Layers { get { return layers; } }
        public int Neurons { get { return neurons; } }

        [Header("Manual Inputs")]
        [SerializeField] [Range(-1, 1)] private float xDirection = 0;
        [SerializeField] [Range(-1, 1)] private float yDirection = 0;

        private float[] sensors = new float[8];
        public int NumOfSensors { get { return sensors.Length; } }
        private float sensorAvg 
        { 
            get 
            { 
                float answer = 0f;

                foreach (float val in sensors) {
                    answer += val;
                }

                return answer / sensors.Length;
            }
        }

        private Tile lastPosition;
        private float aliveTime;

        private int foodEaten = 0;

        private NeuralNetwork network;

        private List<Tile> visitedTiles = new List<Tile>();
        private int movesSinceNewTile = 0;

        private float distanceFitness = 0f;

        private float timeSinceNegativeFitness = 0f;
        private float lastFitness = 0f;

        [SerializeField] int availableMoves = 20;
        [SerializeField] int usedMoves = 0;

        // EXECUTION FUNCTIONS
        private void Update() {
            InputSensors();

            (xDirection, yDirection) = network.RunNetwork(sensors);

            Move(xDirection, yDirection);

            timeSinceStart += Time.deltaTime;
            timeSinceNegativeFitness += Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other) {
            Tile tile = other.GetComponent<Tile>();
            if (tile == null) return;

            if (tile.TileType == TileTypes.WATER) {
                manager.Kill();
            }

            else if (tile.TileType == TileTypes.FOOD) {
                manager.Stats.Restore("Hunger");
                tile.TurnToGrass();
                foodEaten++;
            }
        }

        // METHODS
        public override void Initialize(EntityManager manager, Tile startPosition) {
            base.Initialize(manager, startPosition);
            network = new NeuralNetwork();
        }

        private void CalculateFitness() {
            aliveTime += Time.deltaTime;
            
            /*
            overallFitness = (aliveTime * aliveTimeMultiplier) + 
                                (manager.Stats.Hunger * hungerMultiplier) +
                                (sensorAvg * sensorMultiplier) +
                                (foodEaten * foodEatenMultiplier) +
                                (visitedTiles.Count * visitedTilesMultiplier) -
                                movesSinceNewTile * movesSinceNewTile;
            */

            /*
            bool onTarget = manager.Movement.CurrentPosition == target;

            overallFitness = (aliveTime * aliveTimeMultiplier) + 
                                (distanceFitness * distanceToTargetMultiplier) -
                                (visitedTiles.Count * visitedTilesMultiplier) +
                                (onTarget ? 20f : 0f);

            fitnessText.text = overallFitness.ToString("F2");

            if (lastFitness < overallFitness) {
                fitnessText.color = Color.green;
            }
            else if (lastFitness > overallFitness) {
                fitnessText.color = Color.red;
            }
            else fitnessText.color = Color.white;

            lastFitness = overallFitness;

            if (overallFitness > 10f) {
                timeSinceNegativeFitness = 0f;
            }

            if (onTarget) {
                GetNewTarget();
            }

            if (timeSinceNegativeFitness > 20f && overallFitness < 10f) {
                manager.Kill();
            }
            */

            overallFitness = distanceFitness * distanceToTargetMultiplier;

            fitnessText.text = overallFitness.ToString("F2");

            if (lastFitness < overallFitness) {
                fitnessText.color = Color.green;
            }
            else if (lastFitness > overallFitness) {
                fitnessText.color = Color.red;
            }
            else fitnessText.color = Color.white;

            lastFitness = overallFitness;

            if (usedMoves > availableMoves) {
                manager.Kill();
            }
        }

        private void InputSensors() {
            Tile[] neighbours = manager.Movement.GetNeighbours();

            for (int i = 0; i < sensors.Length; i++) {
                sensors[i] = 0f;
            }

            for (int i = 0; i < neighbours.Length; i++) {
                sensors[i] = neighbours[i].SensorValue;
            }
        }

        protected override void Move(float x, float y) {
            if (!ReadyToMove()) return;

            int finalX = 0;
            int finalY = 0;

            if (x <= -0.5f) finalX = -1;
            else if (x >= 0.5f) finalX = 1; 

            if (y <= -0.5f) finalY = -1;
            else if (y >= 0.5f) finalY = 1;

            var tile = manager.Movement.MoveToPoint(manager.Movement.GetNeighbour(finalX, finalY));

            float lastPosDistance = Vector3.Distance(lastPosition.transform.position, target.transform.position);
            float currentPosDistance = Vector3.Distance(manager.Movement.CurrentPosition.transform.position, target.transform.position);

            float val = currentPosDistance;

            if (currentPosDistance == 0) val = 1f;

            if (lastPosDistance > currentPosDistance) {
                distanceFitness += (1 / val);
            }
            else if (lastPosDistance < currentPosDistance) {
                distanceFitness -= (1 / val);
            }

            lastPosition = manager.Movement.CurrentPosition;

            usedMoves++;

            CalculateFitness();

            if (tile == null) {
                movesSinceNewTile++;
                return;
            }

            if (!visitedTiles.Contains(tile)) {
                visitedTiles.Add(tile);
                tile.FindTile();
                movesSinceNewTile = 0;
            }
            movesSinceNewTile++;
        }

        public override void Kill() {
            manager.Stats.Reset();
            ResetController();
            FindObjectOfType<GeneticManager>().Kill(overallFitness, network);
        }

        public override void ResetController() {
            timeSinceStart = 0f;
            timeSinceNegativeFitness = 0f;
            aliveTime = 0f;
            lastPosition = startPosition;
            overallFitness = 0f;
            foodEaten = 0;
            visitedTiles.Clear();
            usedMoves = 0;
            distanceFitness = 0;

            manager.Movement.MoveToPointInstant(startPosition);
        }

        public void ResetWithNetwork(NeuralNetwork net) {
            network = net;
            ResetController();
        }

        public void GetNewTarget() {
            Tile target = TerrainManager.Instance.GetRandomTileOfType(TileTypes.GRASS);
            this.target = target;
            initialDistanceToTarget = Vector3.Distance(transform.position, target.Position);

            SearchAlgorithms.GetPath(manager.Movement.CurrentPosition, target, true);
            target.GetComponent<Renderer>().material.color = Color.yellow;
        }
    }
}
