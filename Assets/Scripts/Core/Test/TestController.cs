using UnityEngine;
using System.Collections.Generic;

namespace Test
{
    public class TestController : MonoBehaviour
    {
        // VARIABLES
        private Vector3 startPosition, startRotation;
        private TestNNet network;

        [Range(-1f,1f)]
        public float a,t;

        public float timeSinceStart = 0f;

        [Header("Fitness")]
        public float overallFitness;
        public float distanceMultipler = 1.4f;
        public float avgSpeedMultiplier = 0.2f;
        public float sensorMultiplier = 0.1f;
        public float checkpointsMultiplier = 0.1f;
        public float goalMultiplier = 0.5f;

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

        private float[] sensors = new float[3];
        private float[] checkpointSensors = new float[2];
        private float goalSensor;
        private float[] dangerSensors = new float[8];

        public int LapsDone { get; private set; }
        public void FinishLap() => LapsDone++;

        private float sensorAvg {
            get {
                float answer = 0f;

                foreach (float f in sensors) {
                    answer += f;
                }

                answer /= sensors.Length;
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

        [SerializeField] private Transform goal;

        // EXECUTION FUNCTIONS
        private void Awake() 
        {
            startPosition = transform.position;
            startRotation = transform.eulerAngles;
            network = new TestNNet();
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

            float[] firstCombine = CombineArrays(sensors, checkpointSensors);
            float[] finalArray = CombineArrays(firstCombine, dangerSensors);

            (a, t) = network.RunNetwork(finalArray);

            Move(a,t);

            timeSinceStart += Time.deltaTime;

            CalculateFitness();

            //a = 0;
            //t = 0;
        }

        // METHODS
        public void ResetWithNetwork (TestNNet net)
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
        }

        private void Death () {
            FindObjectOfType<TestGenManager>().Death(overallFitness, network);
        }

        private void CalculateFitness() {

            totalDistanceTravelled += Vector3.Distance(transform.position,lastPosition);
            avgSpeed = totalDistanceTravelled/timeSinceStart;

            overallFitness = (totalDistanceTravelled * distanceMultipler)+
                                (avgSpeed * avgSpeedMultiplier) +
                                (sensorAvg * sensorMultiplier) +
                                (checkpointSensorAvg * checkpointsMultiplier) +
                                (dangerSensorAvg * sensorMultiplier) +
                                (goalSensor * goalMultiplier);

            if (timeSinceStart > 20 && overallFitness < 40) {
                Death();
            }

            if (overallFitness >= 1000) {
                Death();
            }

        }

        private void InputSensors() {
            // WALL SENSORS
            Vector3[] directions = new Vector3[] {
                transform.forward + transform.right,
                transform.forward,
                transform.forward - transform.right
            };

            for (int i = 0; i < sensors.Length; i++) {
                Ray r = new Ray(transform.position, directions[i]);
                RaycastHit hit;
                Color hitColor = Color.green;

                if (Physics.Raycast(r, out hit, 300f, WallLayer)) {
                    sensors[i] = hit.distance/20;
                    hitColor = hit.distance < threshold ? Color.red : Color.green;

                    Debug.DrawLine(r.origin, hit.point, hitColor);
                }
            }

            Ray goalRay = new Ray(transform.position, goal.position - transform.position);
            RaycastHit goalHit;
            Color goalHitColor = Color.blue;

            if (Physics.Raycast(goalRay, out goalHit, 300f, GoalLayer)) {
                goalSensor = 1/goalHit.distance;

                Debug.DrawLine(goalRay.origin, goalHit.point, goalHitColor);
            }

            /*
            // CHECKPOINT SENSORS
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

            // DANGER SENSORS
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
            */
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