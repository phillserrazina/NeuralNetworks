using UnityEngine;
using Coursework.Core.Entities;

namespace Coursework.Core
{
    public enum SimulationTypes { Evolution, NeuralNetwork }

    public class GameInitializer : MonoBehaviour
    {
        // VARIABLES
        [SerializeField] private SimulationTypes simulationType = SimulationTypes.Evolution; 

        [Header("Evolutional Parameters")]
        [SerializeField] private EntityManager evolutionalBunny = null;
        [SerializeField] private int initialPopulation = 4;

        [Header("Neural Network Parameters")]
        [SerializeField] private EntityManager neuralBunny = null;
        [SerializeField] private GeneticManager geneticManager = null;

        // METHODS
        public void Initialize() {
            
            switch (simulationType)
            {
                case SimulationTypes.Evolution:
                {
                    EvolutionalStart();
                    break;
                }

                case SimulationTypes.NeuralNetwork:
                {
                    NetworkStart();
                    break;
                }

                default:
                {
                    Debug.LogError("GameInitializer::Initialize() --- Invalid Simultion Type.");
                    break;
                }
            }
        }

        private void EvolutionalStart() {
            for (int i = 0; i < initialPopulation; i++) {
                PopulationManager.Instance.CreateEntity(evolutionalBunny, GetEntityStartPos());
            }
        }

        private void NetworkStart() {
            var bunny = Instantiate(neuralBunny);
            bunny.Initialize(GetEntityStartPos());

            geneticManager.Initialize(bunny.Controller as NNetController);

            bunny.GetComponent<NNetController>().GetNewTarget();          
        }

        private Tile GetEntityStartPos() {
            var grassTiles = TerrainManager.Instance.GetTilesOfType(TileTypes.GRASS);
            return grassTiles[Random.Range(0, grassTiles.Length)];
        }
    }
}
