using System;
using System.Collections.Generic;
using System.Linq;

//class for a single snake
class SingleSnake
{
  public double fitness;
  public int numOfMoves = 0;
  public int score;

  public List<List<int>> game = new List<List<int>>();

  private int bodyLength;
  private int headPos;
  private bool dead;
  private int curDirection = 1;
  private int movesSinceScore = 0;
  
  public Layer layer1 = new Layer (6,8);
  public ActivationReLu ReLu = new ActivationReLu();
  public Layer layer2 = new Layer (8,8);
  public ActivationReLu ReLu2 = new ActivationReLu();
  public Layer outPutLayer = new Layer (8,3);
  public ActivationSoMax softMax = new ActivationSoMax();

  public List<Layer> layers = new List<Layer>();

  //adding the layers to the layer list
  public SingleSnake()
  {
    layers.Add(layer1);
    layers.Add(layer2);
    layers.Add(outPutLayer);
  }

  //for getting outputs from the snakes nn
  public int forwardPass(List<double> inputs)
  {
    layer1.forwardPass(inputs);
    ReLu.forwardPass(layer1.outputs);
    layer2.forwardPass(ReLu.outputs);
    ReLu2.forwardPass(layer2.outputs);
    outPutLayer.forwardPass(ReLu2.outputs);
    softMax.forwardPass(outPutLayer.outputs);

    //the output is the index of the largest output(0 = left, 1 = forward, 2 = right)
    int chosenDir = Program.indexFinder(softMax.outputs, Program.maxValue(softMax.outputs));

    //for converting left, forward, right to 4 directions by using the last direction
    var dirChange = new Dictionary<int, int[]>(){
    {0,new int[]{3,0,1}},
    {1,new int[]{0,1,2}},
    {2,new int[]{1,2,3}},
    {3,new int[]{2,3,0}}
    };

    int trueDir = dirChange[curDirection][chosenDir];

    return trueDir;
  }

  //for running though a whole game
  public void runThroughCycle(int[] thisBoard)
  {
  
    List<int> instructions = new List<int>(); 
    headPos = 205;
    dead = false;
    bodyLength = 3;
    curDirection = 1; //0 = N,  1 = E,  2 = S,  3 = W

    instructions.Add(203);
    instructions.Add(204);
    instructions.Add(205);

    int lastDis = 100;

    //if its alive keep going
    while (dead == false)
    {
      

      //finding the direction the snake wants to go
      curDirection = forwardPass(inputFinder(thisBoard));

      //updating the board
      thisBoard = updateBoard(thisBoard, instructions, curDirection);
      instructions.Add(headPos); 

      game.Add(thisBoard.ToList());

      numOfMoves++;
      movesSinceScore++;
      
      //updating the fitness
      int disFromApple = appleDis(thisBoard);
      
      if (disFromApple < lastDis)
      {
        fitness+= 1;
      }
      else
      {
     //   fitness-=1;
      }

      lastDis = disFromApple;
    }
    score = bodyLength-3;
    fitness += score * 50;

  }
  //for finding the distance from the head to the apple
  int appleDis(int[] board)
  {
    int applePos = 0;

    for (int i = 0; i < 400; i++)
    {
      if (board[i] == 2)
      {
        applePos = i;
      }
    }

    int headY = Convert.ToInt32(headPos/20);
    int headX = headPos-headY*20;
    int appleY = Convert.ToInt32(applePos/20);
    int appleX = applePos-appleY*20;

    return (Math.Abs(headY-appleY) + Math.Abs(headX-appleX));
  }

  //for finding the inputs for the nn
  List<double> inputFinder (int[] board)
  {
    //inputs check if there is a boundry left, right and forward 
    //and the same for apples (6 inputs)
    List<double> inputs = new List<double>();
  
    //for converting left, forward, right to 4 directions by using the last direction
    var dirChange = new Dictionary<int, int[]>(){
    {0,new int[]{-1,-20,1}},
    {1,new int[]{-20,1,20}},
    {2,new int[]{1,20,-1}},
    {3,new int[]{20,-1,-20}}
    };
    int pos = 0;
    //for boundries and apples 
    for (int p = 0; p < 2; p++)
    {
      //foreach direction
      for (int n = 0; n < 3; n++)
      {
        bool found = false;
        int lastHeadX = headPos - (Convert.ToInt32(headPos/20)) * 20;
    
        //looking all the way out in a direction to see whats there
        for (int i = 1; i < 21; i++)
        {
          pos = headPos + (dirChange[curDirection][n])*i;
          int headX = pos - (Convert.ToInt32(pos/20)) * 20;

          //checking for boundries
          if ( pos > 399 || pos < 0)
          {
            break;
          }
          else if (Math.Abs(headX-lastHeadX) > 18)
          {
            break;
          }
          //if its looking for snake
          else if (p == 0)
          {
            if (board[pos] == 1)
            {
              found = true;
              inputs.Add(1);
              break;
            }
            inputs.Add(0);
            break;
          }
          //apples
          else
          {
            if (board[pos] == 2)
            {
              found = true;
              inputs.Add(1);
              break;
            }
          }

          lastHeadX = headX;
        }
        //if it didnt't find anything
        if (found == false)
        {
          if (p == 0)
          {
            inputs.Add(1);
          }
          else
          {
            inputs.Add(0);
          }
        }
        
      }
    }
    return inputs;
  }

  //for updating the board (takes the current baord and spits out the new one)
  int[] updateBoard (int[] board, List<int> instructions, int direction)
  {
    Random rand = new Random();
    var positions = new Dictionary<int, int>(){
    {0,-20},
    {1,1},
    {2,20},
    {3,-1}
    };

    //finding the new headpos
    int newHeadPos = headPos + positions[direction];

    //checking for death
    if (deathChecker(board,headPos,newHeadPos, direction) == true)
    {
      dead = true;
      return board;
    }

    //if theres an apple add another one otherwise get rid of tail
    if (board[newHeadPos] == 2)
    {
      board = newApple(board);
      bodyLength++;
      movesSinceScore = 0;
    }
    else
    {
      board[instructions[instructions.Count-bodyLength]] = 0;
    }
    //moving the head one up
    board[newHeadPos] = 1;
    
    headPos = newHeadPos;

    return board;
  }

  //checks if the snake died
  bool deathChecker(int[] board, int headPos, int newHeadPos, int direction)
  {
    //up and down boundries
    if (newHeadPos < 0 || newHeadPos > 399)
    {
      return true;
    }

    //snake boundries
    if (board[newHeadPos] == 1)
    {
      return true;
    }

    //side boundries
    if (headPos % 20 == 0 && direction == 3)
    {
      return true;
    }
    if ((headPos+1) % 20 == 0 && direction == 1)
    {
      return true;
    }

    //failsafe for no infinite loop
    if (movesSinceScore > 100)
    {
      return true;
    }
    return false;
  }

  //creates a board with a new apple on it
  int[] newApple (int[] board)
  {
    Random rand = new Random();

    //finding open positions and picking a random one
    List<int> openPos = new List<int>();

    for (int i = 0; i < 400; i++)
    {
      if (board[i] != 1)
      {
        openPos.Add(i);
      }
    }

    board[openPos[rand.Next(openPos.Count)]] = 2;

    return board;
  }

  

  //for displaying games the snake played
  public void showGame()
  {
    var changeNum = new Dictionary<int, string>(){
      {0,"-"},
      {1,"#"},
      {2,"&"},
      };

    for (int n = 0; n < game.Count; n++)
    {
      for (int i = 0; i < 400; i++)
      {
        if (i% 20 == 0)
        {
          Console.WriteLine();
        }
        Console.Write(changeNum[game[n][i]] + " ");
      }
      Console.WriteLine();
    }
  }
}