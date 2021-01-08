using UnityEngine;
using Coursework.Objects;
using System.Collections.Generic;

namespace Coursework.Core
{
    public class NetworkController : MonoBehaviour
    {
        // VARIABLES
        private Vector3 startPosition, startRotation;
        private NeuralNetwork network;

        [Range(-1f,1f)]
        public float a,t;

        public float timeSinceStart = 0f;

        [Header("Fitness")]
        public float overallFitness;
        public float distanceMultipler = 1.4f;
        public float avgSpeedMultiplier = 0.2f;
        [Space(10)]
        public float wallSensorMultiplier = 0.2f;
        public float checkpointSensorsMultiplier = 0.1f;
        public float dangerSensorsMultiplier = 0.1f;
        public float goalSensorMultiplier = 0.1f;

        [Header("Network Options")]
        public int LAYERS = 1;
        public int NEURONS = 10;
        public LayerMask WallLayer;
        public LayerMask CheckpointLayer;
        public LayerMask EnemyLayer;
        public LayerMask GoalLayer;
        public float threshold = 1f;

        private Vector3 lastPosition;
        private float totalDistanceTravelled;
        private float avgSpeed;

        private float[] wallSensors = new float[3];
        private float[] checkpointSensors = new float[2];
        private float[] dangerSensors = new float[8];
        private float[] goalSensors = new float[1];

        public int SENSORS {
            get {
                int answer = 0;

                if (wallSensorMultiplier > 0) answer += 3;
                if (checkpointSensorsMultiplier > 0) answer += 2;
                if (dangerSensorsMultiplier > 0) answer += 8;
                if (goalSensorMultiplier > 0) answer++;

                return answer;
            }
        }

        public int LapsDone { get; private set; }
        public void FinishLap() => LapsDone++;

        private float wallSensorAvg {
            get {
                float answer = 0f;

                foreach (float f in wallSensors) {
                    answer += f;
                }

                answer /= wallSensors.Length;
                return answer;
            }
        }

        private float checkpointSensorAvg {
            get {
                float answer = 0f;

                foreach (float f in checkpointSensors) {
                    answer += f;
                }

                answer /= checkpointSensors.Length;
                return answer;
            }
        }

        private float dangerSensorAvg {
            get {
                float answer = 0f;

                foreach (float f in dangerSensors) {
                    answer += f;
                }

                answer /= dangerSensors.Length;
                return answer;
            }
        }

        [SerializeField] private Transform goal = null;

        // EXECUTION FUNCTIONS
        private void Awake() 
        {
            startPosition = transform.position;
            startRotation = transform.eulerAngles;
            network = new NeuralNetwork(SENSORS);
        }

        private void OnCollisionEnter (Collision collision) {
            if (collision.gameObject.CompareTag("Wall"))
                Death();
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Enemy"))
                Death();
        }

        private void FixedUpdate() 
        {
            InputSensors();
            lastPosition = transform.position;

            float[] finalArray = new float[0];

            if (wallSensorMultiplier > 0) finalArray = (float[])CombineArrays(finalArray, wallSensors).Clone();
            if (checkpointSensorsMultiplier > 0) finalArray = (float[])CombineArrays(finalArray, checkpointSensors).Clone();
            if (dangerSensorsMultiplier > 0) finalArray = (float[])CombineArrays(finalArray, dangerSensors).Clone();
            if (goalSensorMultiplier > 0) finalArray = (float[])CombineArrays(finalArray, goalSensors).Clone();

            if (finalArray.Length <= 0) {
                Debug.LogError("NetworkController::FixedUpdate() --- No sensors available. Please check the multipliers." );
                return;
            }

            (a, t) = network.RunNetwork(finalArray);

            Move(a,t);

            timeSinceStart += Time.deltaTime;

            CalculateFitness();
        }

        // METHODS
        public void ResetWithNetwork (NeuralNetwork net)
        {
            network = net;
            Reset();
        }

        public void Reset() {
            timeSinceStart = 0f;
            totalDistanceTravelled = 0f;
            avgSpeed = 0f;
            lastPosition = startPosition;
            overallFitness = 0f;
            transform.position = startPosition;
            transform.eulerAngles = startRotation;
            LapsDone = 0;

            try { EnemyManager.Instance.Restart(); }
            catch {}
        }

        private void Death () {
            FindObjectOfType<GeneticManager>().Kill(overallFitness, network);
        }

        private void CalculateFitness() {

            totalDistanceTravelled += Vector3.Distance(transform.position,lastPosition);
            avgSpeed = totalDistanceTravelled/timeSinceStart;

            overallFitness = (totalDistanceTravelled * distanceMultipler)+
                                (avgSpeed * avgSpeedMultiplier) +
                                (wallSensorAvg * wallSensorMultiplier) +
                                (checkpointSensorAvg * checkpointSensorsMultiplier) +
                                (dangerSensorAvg * wallSensorMultiplier) +
                                (goalSensors[0] * goalSensorMultiplier);

            if (timeSinceStart > 20 && overallFitness < 40) {
                Death();
            }

            if (overallFitness >= 1000) {
                Death();
            }

        }

        private void InputSensors() {
            // WALL SENSORS
            if (wallSensorMultiplier > 0) {
                Vector3[] directions = new Vector3[] {
                    transform.forward + transform.right,
                    transform.forward,
                    transform.forward - transform.right
                };

                for (int i = 0; i < wallSensors.Length; i++) {
                    Ray r = new Ray(transform.position, directions[i]);
                    RaycastHit hit;
                    Color hitColor = Color.green;

                    if (Physics.Raycast(r, out hit, 300f, WallLayer)) {
                        wallSensors[i] = hit.distance/20;
                        hitColor = hit.distance < threshold ? Color.red : Color.green;

                        Debug.DrawLine(r.origin, hit.point, hitColor);
                    }
                }
            }

            // GOAL SENSOR
            if (goalSensorMultiplier > 0)
            {
                Ray goalRay = new Ray(transform.position, goal.position - transform.position);
                RaycastHit goalHit;
                Color goalHitColor = Color.blue;

                if (Physics.Raycast(goalRay, out goalHit, 300f, GoalLayer)) {
                    goalSensors[0] = 1/goalHit.distance;

                    Debug.DrawLine(goalRay.origin, goalHit.point, goalHitColor);
                }
            }

            
            // CHECKPOINT SENSORS
            if (checkpointSensorsMultiplier > 0)
            {
                Vector3[] checkpointDirections = new Vector3[] {
                    transform.forward + transform.right * 0.05f,
                    transform.forward - transform.right * 0.05f
                };

                for (int i = 0; i < checkpointSensors.Length; i++) {
                    Ray r = new Ray(transform.position, checkpointDirections[i]);
                    RaycastHit hit;

                    if (Physics.Raycast(r, out hit, 10f, CheckpointLayer)) {
                        checkpointSensors[i] = 1/hit.distance;

                        Debug.DrawLine(r.origin, hit.point, Color.blue);
                    }
                    else checkpointSensors[i] = 0f;
                }
            }

            
            // DANGER SENSORS
            if (dangerSensorsMultiplier > 0)
            {
                Vector3[] dangerDirections = new Vector3[] {
                    transform.forward + transform.right,
                    transform.forward,
                    transform.forward - transform.right,

                    transform.right,
                    -transform.right,

                    -transform.forward + transform.right,
                    -transform.forward,
                    -transform.forward - transform.right,
                };

                for (int i = 0; i < dangerSensors.Length; i++) {
                    Ray r = new Ray(transform.position, dangerDirections[i]);
                    RaycastHit hit;

                    if (Physics.Raycast(r, out hit, 20f, EnemyLayer)) {
                        dangerSensors[i] = hit.distance/20;

                        Debug.DrawLine(r.origin, hit.point, Color.yellow);
                    }
                    else {
                        dangerSensors[i] = 1f;
                        Debug.DrawLine(r.origin, r.origin + dangerDirections[i].normalized * 20f, Color.yellow);
                    }
                }
            }
        }

        private Vector3 inp;
        public void Move(float v, float h) {
            inp = Vector3.Lerp(Vector3.zero,new Vector3(0,0,v*11.4f),0.02f);
            inp = transform.TransformDirection(inp);
            transform.position += inp;

            transform.eulerAngles += new Vector3(0, (h*90)*0.02f,0);
        }

        private float[] CombineArrays(float[] arr1, float[] arr2) {
            var answer = new List<float>();

            foreach (float f in arr1) {
                answer.Add(f);
            }

            foreach (float f in arr2) {
                answer.Add(f);
            }

            return answer.ToArray();
        }

    }
}