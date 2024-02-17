using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    // Node in Neural Network
    public class Node
    {
        public float value;
        public float bias;
        public float[] weights; // Weights for every node this node is connected to

        public Node(int numConnections, bool isInput)
        {
            this.value = 0f;
            this.bias = (isInput ? 0f : Random.Range(-5f, 5f)); // Random bias between 0 and 10 (if the layer isn't the input layer)
            this.weights = new float[numConnections];
            for(int i = 0; i < this.weights.Length; i++)
            {
                this.weights[i] = Random.Range(-5f, 5f);
            }
        }

        public void AddBias()
        {
            this.value += bias;
        }

        // Sigmoid activation function (run after calculating weighted sum + bias)
        public void Sigmoid()
        {
            this.value /= (1f + Mathf.Abs(this.value));
        }
    }

    // Neural Network
    public class Network
    {
        public Node[][] network;

        public int INPUT_LAYER = 0;
        public int OUTPUT_LAYER;
        
        public Network(int inputLayer, int[] hiddenLayers, int outputLayer)
        {
            this.network = new Node[hiddenLayers.Length + 2][];
            this.network[0] = new Node[inputLayer];

            for(int i = 0; i < hiddenLayers.Length; i++)
            {
                this.network[i + 1] = new Node[hiddenLayers[i]];
            }
            this.network[hiddenLayers.Length + 1] = new Node[outputLayer];

            // Initialize input layer
            for(int i = 0; i < this.network[0].Length; i++)
            {
                this.network[0][i] = new Node(this.network[1].Length, true);
            }

            // Initialize hidden layers
            for(int i = 1; i < (this.network.Length - 1); i++)
            {
                for(int j = 0; j < this.network[i].Length; j++)
                {
                    this.network[i][j] = new Node(this.network[i + 1].Length, false);
                }
            }

            // Initialize output layers
            for(int i = 0; i < this.network[this.network.Length - 1].Length; i++)
            {
                this.network[this.network.Length - 1][i] = new Node(0, false);
            }

            this.OUTPUT_LAYER = this.network.Length - 1;
        }

        // Updates the state of all neurons
        public void Run()
        {
            // For each layer ...
            for(int i = 0; i < this.network.Length; i++)
            {
                // For each node in layer ...
                for(int j = 0; j < this.network[i].Length; j++)
                {
                    // Add this node's value multiplied by the weight to the value of each node it's connected to
                    for(int k = 0; k < this.network[i][j].weights.Length; k++)
                    {
                        this.network[i + 1][k].value += (this.network[i][j].value * this.network[i][j].weights[k]); // value * weight
                    }
                }
                if(i < (this.network.Length - 1))
                {
                    // Add bias and run activation function on every node in the next layer once their weighted sums are calculated by the above loop
                    for(int j = 0; j < this.network[i + 1].Length; j++)
                    {
                        this.network[i + 1][j].AddBias();
                        this.network[i + 1][j].Sigmoid();
                    }
                }
            }
        }

        public void Print()
        {
            for(int i = 0; i < this.network.Length; i++)
            {
                string e = "";
                for(int j = 0; j < this.network[i].Length; j++)
                {
                    e += this.network[i][j].value + " ";
                }
                Debug.Log(e);
            }
        }
    }

    public int[] hiddenLayerNodes; // Numbers of nodes to use for every hidden layer (each element = 1 layer)

    CarController controller;
    DepthSensor sensor;
    Rigidbody2D body;

    Network network = null; // Neural Network

    const int ACCELERATE = 0;
    const int DECELERATE = 1;
    const int TURN_RIGHT = 2;
    const int TURN_LEFT = 3;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        controller = GetComponent<CarController>();
        sensor = GetComponent<DepthSensor>();
    }

    void Update()
    {
        if (network == null)
        {
            // Initialize network once distance sensor data is initialized
            if (sensor.distanceSensorData.Length > 0)
            {
                network = new Network(sensor.distanceSensorData.Length + 1, hiddenLayerNodes, 4);

            }
        }
        else if (controller.alive)
        {
            // Update input
            for(int i = 0; i < sensor.distanceSensorData.Length; i++)
            {
                network.network[0][i].value = Sigmoid(sensor.distanceSensorData[i]);
            }
            network.network[network.INPUT_LAYER][sensor.distanceSensorData.Length - 1].value = Sigmoid(controller.speed);

            network.Run();

            /*
            Makes a decision on what to do next based on which output nodes are more active
            */

            // Accelerate or decelerate?
            if(network.network[network.OUTPUT_LAYER][ACCELERATE].value >= network.network[network.OUTPUT_LAYER][DECELERATE].value)
            {
                if(controller.speed < controller.maxSpeed)
                {
                    controller.speed += controller.acceleration * Time.deltaTime;
                }
            }
            else
            {
                if(controller.speed > -controller.maxSpeed)
                {
                    controller.speed -= controller.acceleration * Time.deltaTime;
                }
            }

            // Turn left or right?
            if(network.network[network.OUTPUT_LAYER][TURN_LEFT].value >= network.network[network.OUTPUT_LAYER][TURN_RIGHT].value)
            {
                transform.Rotate(Vector3.forward * Time.deltaTime * controller.turnSpeed);
            }
            else
            {
                transform.Rotate(-Vector3.forward * Time.deltaTime * controller.turnSpeed);
            }

            float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            body.velocity = new Vector2(controller.speed * Mathf.Cos(angle), controller.speed * Mathf.Sin(angle));
        }
    }

    // Returns a number representing how successful this car is (larger value = more successful)
    float CalculateSuccess()
    {
        return controller.place * 100f - Vector2.Distance(
            controller.lastPassedCheckpoint.transform.position,
            controller.lastPassedCheckpoint.GetComponent<Checkpoint>().next.transform.position);
    }

    float Sigmoid(float x)
    {
        return x / (1f + Mathf.Abs(x));
    }
}
