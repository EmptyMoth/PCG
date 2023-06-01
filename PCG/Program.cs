using System.Diagnostics;

var random = new Random();
var pcg = new PCG.PCG();
//var file = new StreamWriter(@"C:\Users\halim\RiderProjects\PCG\PCGTests\Sample.csv", false, Encoding.Default);

Console.WriteLine("Hello, World!");
Console.WriteLine("Экперимент на среднее время при 1_000 повторений, которые вызывают ф-ию рандома 100_000_000 раз.");
Console.Write("C# realisation: ");
Console.WriteLine(TimeExperiment(random, 1_000));
Console.Write("PCG realisation: ");
Console.WriteLine(TimeExperiment(pcg, 1_000));

//file.Close();
//Console.WriteLine($"{Convert.ToString(125, toBase: 10)}");

long TimeExperiment(Random testRandom, int experimentCount)
{
    var summaryTime = 0L;
    for (var i = 0; i < experimentCount; i++)
        summaryTime += TestTimeRandom(testRandom);
    return summaryTime / experimentCount;
}

long TestTimeRandom(Random testRandom)
{
    var timer = new Stopwatch();
    timer.Start();
    
    MainRandom(testRandom);
    
    timer.Stop();
    return timer.ElapsedMilliseconds;
}

void MainRandom(Random testRandom)
{
    for (var i = 0; i < 10_000_000; i++)
        testRandom.Next();
}
