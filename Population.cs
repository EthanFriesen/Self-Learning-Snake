using System;
using System.Collections.Generic;
using System.Linq;

//class for each Population of snakes
class Population
{
  public List<SingleSnake> generation = new List<SingleSnake>();
  public List<double> fitnesses = new List<double>();
  private int parentNum;
  private double mutateRate;
  private int appleSeed;
  private int showGame;
  private int genSize;

  public List<double> bestFitnesses = new List <double>();
  public List<double> avgFitnesses = new List <double>();

  //creating all the snakes with their nn's
  public Population(int size, int numOfParents, double mutationRate, int showeGame)
  {
    for (int i = 0; i < size; i++)
    {
      SingleSnake snake = new SingleSnake();
      generation.Add(snake);
    }
    parentNum = numOfParents;
    mutateRate = mutationRate;
    showGame = showeGame;
    genSize = size;
  }

  //for doing the whole thang
  public void run(int numOfCycles)
  {
    for (int i = 0; i < numOfCycles; i++)
    {
      Console.WriteLine($"Cycle Num: {i}");

      playGames(i);
      
      //getting the average fitness
      double avg = 0;
      foreach (SingleSnake snake in generation){avg+= snake.fitness; }
      avg = avg/generation.Count;
      avgFitnesses.Add(avg);
      Console.WriteLine($"average fitness: {avg}");

      crossOver(chooseParents(parentNum), genSize);
      mutation(mutateRate);

    }
  }

  //for running through games of an entire generation of snakes
  public void playGames(int cycleNum)
  {
    createBoard(true);
    foreach (SingleSnake snake in generation)
    {
      snake.runThroughCycle(createBoard(false));
      fitnesses.Add(snake.fitness);
    } 
  }

  //for creating an intitial board
  int[] createBoard(bool first)
  {
    //for creating a seed for the apple
    if (first)
    {
      Random rand = new Random();

      List<int> openSquares = new List<int>();
      
      for (int i = 0; i < 203; i++){openSquares.Add(i);}
      for (int i = 206; i < 400; i++){openSquares.Add(i);}

      appleSeed = openSquares[rand.Next(openSquares.Count)];

      return null;
    }
    
    int[] board = new int[400];

    for (int i = 0; i < 400; i++){board[i] = 0;}

    board[appleSeed] = 2;

    board[205] = 1;
    board[204] = 1;
    board[203] = 1;

    return board;
  }

  //for finding parents used for next generation
  public List<SingleSnake> chooseParents(int numOfParents)
  {

    List<SingleSnake> parents = new List<SingleSnake>();

    //finding the snakes with the highest fitnesses
    for (int i = 0; i < numOfParents; i++)
    {
      int best = (Program.indexFinder(fitnesses, Program.maxValue(fitnesses)));

      parents.Add(generation[best]);

      if (i == 0)
      {
        bestFitnesses.Add(fitnesses[best]);
        if (showGame == 1)
        {
           generation[best].showGame(); 
        }
      }
      
      
      if (i < 10)
      {
        Console.WriteLine($"Place: {i} fitness: {fitnesses[best]} score: {generation[best].score}");
      }

      fitnesses[best] = 0;
    }
    return parents;
  }

  //for breeding parents and creating next generation
  void crossOver(List<SingleSnake> parents, int popSize)
  {
    foreach (SingleSnake snake in generation)
    {
      snake.game.Clear();
      snake.game.TrimExcess();
    }
    //killing all the old snakes in the generation
    generation.Clear();
    fitnesses.Clear();
    generation.TrimExcess();
    fitnesses.TrimExcess();

    Random rand = new Random();

    //making new snakes
    for (int i = 0; i < popSize; i++)
    {
      var availableSnakes = Enumerable.Range(0, parents.Count).ToList();
      int choseSnake = (rand.Next(0,parents.Count));
      availableSnakes.Remove(choseSnake);

      //finding the parents
      SingleSnake[] chosenParents = twoParents(parents);
      SingleSnake dad = chosenParents[0];
      SingleSnake mom = chosenParents[1];

      //creating the child
      SingleSnake child = new SingleSnake();

      //foreach layer in the snake
      for (int layerNum = 0; layerNum < child.layers.Count; layerNum++)
      {
        //foreach neuron in the layer
        for (int neuronNum = 0; neuronNum < child.layers[layerNum].weights.Count; neuronNum++)
        {
          int pointOfCrossover = rand.Next(0,child.layers[layerNum].weights[neuronNum].Count);

          //changing the weights
          for (int weightNum = 0; weightNum < child.layers[layerNum].weights[neuronNum].Count; weightNum++)
          {
            if (weightNum < pointOfCrossover)
            {
              child.layers[layerNum].weights[neuronNum][weightNum] = dad.layers[layerNum].weights[neuronNum][weightNum];
            }
            else
            {
              child.layers[layerNum].weights[neuronNum][weightNum] = mom.layers[layerNum].weights[neuronNum][weightNum];
            }
          }
          //changing the bias
          double[] parentBiases = {mom.layers[layerNum].biases[neuronNum], dad.layers[layerNum].biases[neuronNum]};
          child.layers[layerNum].biases[neuronNum] = parentBiases[rand.Next(0,2)];
        }
      }
      //adding to the new generation
      generation.Add(child);
    }
    parents.Clear();
    parents.TrimExcess();
  }

  //for adding random mutations to the newly created Population
  void mutation(double mutationRate)
  {
    Random rand = new Random();
    double mutateChance = 0;

    //making new snakes
    foreach (SingleSnake child in generation)
    {
      //foreach layer in the snake
      for (int layerNum = 0; layerNum < child.layers.Count; layerNum++)
      {
        //foreach neuron in the layer
        for (int neuronNum = 0; neuronNum < child.layers[layerNum].weights.Count; neuronNum++)
        {
          int pointOfCrossover = rand.Next(0,child.layers[layerNum].weights[neuronNum].Count);

          //changing the weights
          for (int weightNum = 0; weightNum < child.layers[layerNum].weights[neuronNum].Count; weightNum++)
          {
            mutateChance = rand.NextDouble();

            if (mutateChance < mutationRate)
            {
              child.layers[layerNum].weights[neuronNum][weightNum] = (rand.NextDouble() * (1 - -1) + -1);
            } 
          }
          //changing bias
          mutateChance = rand.NextDouble();
          if (mutateChance < mutationRate)
          {
            child.layers[layerNum].biases[neuronNum] = (rand.NextDouble() * (1 - -1) + -1);
          }
        }
      }
    }
  }

  //for choosing 2 parents out of the parent generation
  SingleSnake[] twoParents(List<SingleSnake> someParents)
  {
    Random rand = new Random();

    List<SingleSnake> parents = new List<SingleSnake>();
    foreach (SingleSnake parent in someParents){parents.Add(parent);}

    //finding all the fitnesses
    List<double> parentFitnesses = new List<double>();
    foreach (SingleSnake snake in parents){parentFitnesses.Add(snake.fitness);}

    //making all the fitnesses positive and on a positive relative scale
    for (int i = 0; i < parentFitnesses.Count; i++)
    {
      parentFitnesses[i]+= Math.Abs(parentFitnesses[parentFitnesses.Count-1]);
    }

    //choosing parents by having the better parents have higher chances
    SingleSnake[] chosenParents = new SingleSnake[2];
    for (int n = 0; n < 2; n++)
    {
      double randNum = rand.Next(0, Convert.ToInt32(parentFitnesses.Sum()));
      double partialSum = 0;

      for (int i = 0; i < parentFitnesses.Count; i++)
      {
        partialSum+=parentFitnesses[i];
        if (partialSum >= randNum)
        {
          chosenParents[n] = parents[i];
          parents.RemoveAt(i);
          parentFitnesses.RemoveAt(i);
          break;
        }
      }
    }
    
    return chosenParents;
  }
}