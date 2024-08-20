using System;
using System.Collections.Generic;
using System.Linq;

//class for activation function using non rectified linear
class ActivationReLu
{
  public List<double> outputs = new List<double>();

  public List<double> forwardPass(List<double> inputs)
  {
    outputs.Clear();

    foreach (double input in inputs)
    {
      outputs.Add(Math.Max(0,input));
    }

    return outputs;
  }
}
