// See https://aka.ms/new-console-template for more information
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

public class Name
{
    public string name;
}
public class Batch:Name
{
    public int batch;
}
public class Details : Batch
{
    public string detpartment;
    public Details(string _detpartment,string _name,int _batch)
    {
        name = _name;
        batch = _batch;
        detpartment = _detpartment;
    }
    public void print_details()
    {
        Console.WriteLine(name + " " + detpartment+" "+ batch);
    }
}
interface IName
{
    public void Name(string _name);
}
interface IBatch
{
    public void Batch(int _batch);
}
public class Info : IName, IBatch
{
    public void Batch(int _batch)
    {
        Console.WriteLine(_batch);
    }

    public void Name(string _name)
    {
        Console.WriteLine(_name);
    }
}
public class BasicOOP
{
    public static void Main(string[] args)
    {
        //Details a = new Details("Cse", "Mubasshir", 19);
        //a.print_details();
        Info i = new Info();
        i.Name("Naib");
        i.Batch(19);
    }
}