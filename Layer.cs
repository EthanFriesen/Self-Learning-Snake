using System;
using System.Collections.Generic;
using System.Linq;

//class for layers
class Layer
{
  Random random = new Random();
  public List<List<double>> weights = new List<List<double>>();
  public List<double> biases = new List<double>();
  public List<double> outputs = new List<double>();
  private int inputCount;
  private int neuronCount;

  //creating the initial weights and biases
  public Layer(int numOfInputs, int numOfNeurons)
  {
    inputCount = numOfInputs;
    neuronCount = numOfNeurons;

    //for each neuron in the layer
    for (int i = 0; i < numOfNeurons; i++)
    {
      weights.Add(new List<double>());
      //for each weight in the neuron
      for (int n = 0; n < numOfInputs; n++)
      {
        //adding a random weight between 0 and 1
        weights[i].Add(random.NextDouble() * (1 - -1) + -1);
      }
      biases.Add(random.NextDouble() * (1 - -1) + -1);
    }
  }

  //for passing inputs through and getting an output 
  public List<double> forwardPass(List<double> inputs)
  {
    outputs.Clear();
    double neuronOutput = 0;

    //foreach neuron in the layer
    for (int i = 0; i < neuronCount; i++)
    {
      neuronOutput = 0;

      //foreach weight in each neuron
      for (int n = 0; n < inputCount; n++)
      {
          neuronOutput+= inputs[n] * weights[i][n];
      }
      outputs.Add(neuronOutput+biases[i]);
    }
    return outputs;
  }
}