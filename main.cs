using System;
using System.Collections.Generic;
using System.Linq;

class Program {
  public static void Main (string[] args) {

    Console.WriteLine("How many snakes do you want in each generation (reccomended 1000)");
    int numOfSnakes = Convert.ToInt32(Console.ReadLine());
    Console.WriteLine("How many generations do you want (reccomended 50)");
    int numOfGens = Convert.ToInt32(Console.ReadLine());
    Console.WriteLine("How many parents do you want each generation(reccomended 100)");
    int numOfParents = Convert.ToInt32(Console.ReadLine());
    Console.WriteLine("What do you want the mutation rate to be(recommended 0.05)");
    double mutationRate = Convert.ToDouble(Console.ReadLine());
    Console.WriteLine("Do you want to show the game of the best snake?(0/1)");
    int showGame = Convert.ToInt32(Console.ReadLine());

    
    DateTime start = DateTime.Now;
    
    Population pop = new Population(numOfSnakes, numOfParents, mutationRate, showGame);

    pop.run(numOfGens);

    Console.WriteLine("Best fitnesses: ");
    foreach (double bestFit in pop.bestFitnesses){Console.WriteLine(bestFit);}
    Console.WriteLine("Average fitnesses: ");
    foreach (double avg in pop.avgFitnesses){Console.WriteLine(avg);}

    Console.WriteLine("time taken: " + ((start-DateTime.Now).TotalSeconds));

  }

  //for finding the largest value in a double list
  public static double maxValue(List<double> list)
  {
    double maxValue = -1000;
    foreach (double value in list)
    {
        if (value > maxValue)
        {
            maxValue = value;
        }
    }
    return maxValue;
  }

  //for finding the index of a value in a list
  public static int indexFinder(List<double> list, double value)
  {
    for (int i = 0; i < list.Count; i++)
    {
      if (list[i] == value)
      {
        return i;
      }
    }
    return -1;
  }
  
}