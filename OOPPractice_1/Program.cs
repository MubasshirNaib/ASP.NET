// See https://aka.ms/new-console-template for more information



//string v =Console.ReadLine();
//string p = v;
//int number= Convert.ToInt32(Console.ReadLine());
//string k;
//k = "Mubasshir";

//Console.WriteLine(p +" "+ number);

public abstract class Vehicle
{
    protected string vehicle_type;//encapsulation;
    public Vehicle(string _type)
    {
        vehicle_type = _type;
    }
    public abstract void details();//absrtract
    public void owner_name(string _name)
    {
        Console.WriteLine("Owner of the "+ vehicle_type +" is "+_name);
    }
}
public class Car : Vehicle//inheritance
{
    public Car(string _type) : base(_type)
    {
    }

    public override void details()//polimorphism
    {
        Console.WriteLine("This is " + vehicle_type);
    }
}
public class Bike : Vehicle//inheritance
{
    public Bike(string _type) : base(_type)
    {
    }

    public override void details()//polimorphism
    {
        Console.WriteLine("This is " + vehicle_type);
    }
}

public class practice
{
    public static void Main(string[] args)
    {
        Vehicle vehicle_1 = new Car("Car");//object
        vehicle_1.details();
        string name=Console.ReadLine();
        vehicle_1.owner_name(name);
        Vehicle vehicle_2 = new Bike("Bike");//object
        vehicle_2.details();
        name = Console.ReadLine();
        vehicle_1.owner_name(name);
    }
}
