using System;
using System.Collections.Generic;
using System.Linq;

//class for activation function using softmax
class ActivationSoMax
{
    private List<double> expValues = new List<double>();
    public List<double> outputs = new List<double>();
    

    public void forwardPass(List<double> inputs)
    {
      expValues.Clear();
      outputs.Clear();
      
      //expenentiating the values and subtracting the max so the 
      //numbers don't get out of control
      foreach (double input in inputs)
      {
        expValues.Add(Math.Exp(input)-Program.maxValue(inputs));
      }

      //normalizing the values
      foreach (double value in expValues)
      {
        outputs.Add(value/expValues.Sum());
      }
    }
}