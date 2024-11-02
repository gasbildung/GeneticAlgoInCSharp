//Customise the below to tune the algorithm, note: obstacleOptions and moveOptions must be of equal length
//and the Nth element of each should directly correspond to each other.
List<string> obstacleOptions = new List<string>(){"road","water","hole","tunnel","mountain","forest","air","river","city"};
List<string> moveOptions = new List<string>(){"walk","swim","jump","crawl","climb","navigate","fly","paddle","parkour"};
List<string> testPath = new List<string>(){"road","road","water","city","road","water","tunnel","hole","hole","forest","air","river"};
int populationSize = 2000;
int survivalSize = 5;
int generations = 40;

//Function: Fitness function to test strategy.
static int Evaluate(Dictionary<string, string> strategy, List<string> obstacleOptions, List<string> moveOptions, List<string> testPath)
{
    int score = 0;
    foreach (string tp in testPath)
    {
        if (obstacleOptions.IndexOf(tp)==moveOptions.IndexOf(strategy[tp]))
        {
            score++;
        }
        else
        {
            return score;
        }

    }

    return score;
}

//Function: Generate random strategy.
static Dictionary<string, string> GenerateRandomStrategy(List<string> obstacleOptions, List<string> moveOptions)
{
    Random rnd = new Random();
    List<string> temp= moveOptions.ToList();
    Dictionary<string, string> strategy = new Dictionary<string, string>();
    foreach (string obstacleOption in obstacleOptions)
    {
        string stratagem = temp[rnd.Next(temp.Count)];
        strategy.Add(obstacleOption,stratagem);
        temp.Remove(stratagem);
    }
    return strategy;
    
}

//Function: Print strategy.
static void PrintStrategy(Dictionary<string, string> strategy)
{
    foreach (string key in strategy.Keys)
    {
        Console.WriteLine("{0}: {1}", key, strategy[key]);
    }
}

//Function: Marry 2 strategies.
static Dictionary<string, string> Marry(Dictionary<string, string> strategy1, Dictionary<string, string> strategy2)
{
    Dictionary<string, string> child = new Dictionary<string, string>();
    Random rnd = new Random();
    int rand = rnd.Next(0, strategy1.Count);
    for (int i = 0; i < strategy1.Count; i++)
    {
        if (i < rand)
        {
            child.Add(strategy1.ElementAt(i).Key, strategy1.ElementAt(i).Value);
        }
        else
        {
            child.Add(strategy2.ElementAt(i).Key, strategy2.ElementAt(i).Value);
        }
    }
    return child;
}

//Function: Mutate a strategy.
static Dictionary<string, string> Mutate(Dictionary<string, string> strategy,List<string> moveOptions)
{
    Dictionary<string, string> mutant = new Dictionary<string, string>();
    Random rnd = new Random();
    int rand = rnd.Next(0, strategy.Count);
    int rand2 = rnd.Next(0, moveOptions.Count);
    foreach (KeyValuePair<string, string> kvp in strategy)
    {
        mutant.Add(kvp.Key, kvp.Value);
    }
    string keyToMutate = strategy.ElementAt(rand).Key;
    mutant[keyToMutate] = moveOptions.ElementAt(rand2);
    return mutant;
}

//Function: Generate initial population, based on populationSize.
static List<Dictionary<string, string>> GeneratePopulation(List<string> obstacleOptions, List<string> moveOptions,
    int populationSize)
{
    List<Dictionary<string, string>> population = new List<Dictionary<string, string>>();
    for (int i = 0; i < populationSize; i++)
    {
        Dictionary<string, string> tempStrategy = GenerateRandomStrategy(obstacleOptions, moveOptions);
        population.Add(tempStrategy);
    }
    return population;
}

//Function: Return sorted population by descending order of fitness.
static List<Dictionary<string, string>> SortPopulation(List<Dictionary<string, string>> population, List<string> obstacleOptions, List<string> moveOptions, List<string> testPath)
{
    List<Dictionary<string, string>>sortedPopulation = population.OrderByDescending(strategy =>Evaluate(strategy, obstacleOptions, moveOptions, testPath)).ToList();
    return sortedPopulation;
}

//Function: Run simulation.
Dictionary<string, string> SimulateEvolution(List<string> obstacleOptions, List<string> moveOptions, List<string> testPath, int populationSize, int survivalSize, int generations)
{
    List<Dictionary<string, string>> population = GeneratePopulation(obstacleOptions, moveOptions, populationSize);
    for (int i = 0; i < generations; i++)
    {
        Console.WriteLine($"Generation:{i}");
        Console.WriteLine($"Evaluation:{Evaluate(population.ElementAt(0), obstacleOptions, moveOptions, testPath)}");
        List<Dictionary<string, string>>tempReducedPopulation = SortPopulation(population, obstacleOptions, moveOptions, testPath).GetRange(0,survivalSize);
        Dictionary<string, string> marriedStrategy = Marry(tempReducedPopulation.ElementAt(1),tempReducedPopulation.ElementAt(2));
        Dictionary<string, string> mutatedStrategy = Mutate(tempReducedPopulation.ElementAt(3), moveOptions);
        //Removing 2, 1 for mutated strategy 1 for child of married.
        List<Dictionary<string, string>> tempAddedPopulation=GeneratePopulation(obstacleOptions, moveOptions, populationSize-survivalSize-2);
        tempAddedPopulation.AddRange(tempReducedPopulation);
        tempAddedPopulation.Add(marriedStrategy);
        tempAddedPopulation.Add(mutatedStrategy);
        population.Clear();
        population.AddRange(SortPopulation(tempAddedPopulation, obstacleOptions, moveOptions, testPath));
        tempAddedPopulation.Clear();
        tempReducedPopulation.Clear();
        
        
    }
    population=SortPopulation(population, obstacleOptions, moveOptions, testPath);
    return population.ElementAt(0);
}

Dictionary<string, string>solution = SimulateEvolution(obstacleOptions, moveOptions, testPath, populationSize, survivalSize, generations);
PrintStrategy(solution);
Console.WriteLine($"Final Score:{Evaluate(solution, obstacleOptions, moveOptions, testPath)}/{testPath.Count}");
