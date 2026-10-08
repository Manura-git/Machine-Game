// program.cs
using Spectre.Console;

var Halle1 = new Halle();


bool running = true;
while (running)
{
    var wahl = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
    .Title("Hauptmenü")
    .AddChoices("Maschinenpark anzeigen","Maschine hinzufügen","Simulieren","Wartung durchführen","Beenden")
    );

    switch(wahl)
    {
        case "Maschinenpark anzeigen":
            Console.WriteLine("Maschinenpark anzeigen...");
            Halle1.ShowAll();
            break;

        case "Maschine hinzufügen":
            Console.WriteLine("Maschine hinzufügen...");
            // input
            var wahl_machine = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Maschinenauswahl").AddChoices("Fraesmaschine","Drehmaschine","Roboter","zurück"));
            if(wahl_machine == "zurück"){break;};

            var name = AnsiConsole.Ask<string>("Wie heißt die Maschine?");
            // logic
            switch(wahl_machine)
            {
                case "Fraesmaschine": Halle1.AddMachine(new Fraesmaschine(name));break;
                case "Drehmaschine": Halle1.AddMachine(new Drehmaschine(name));break;
                case "Roboter": Halle1.AddMachine(new Roboter(name));break;
                case "zurück": running=false;break;
            }
            break;
            
        case "Beenden":
            Console.WriteLine("beenden...");
            running = false;
            break;
        case "Simulieren":
            var anzahlticks = AnsiConsole.Ask<int>("Anzahl ticks");
            Halle1.SimTick(anzahlticks);
            break;
        case "Wartung durchführen":
            List<string> choices = Halle1.GetAllMachineNames();
            var wahl_wartung = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Maschinenwartung").AddChoices(choices)); 
            foreach (var item in Halle1._maschinen)
            {
                if (item.Name == wahl_wartung){item.Warte();}
            }
            break;
    }
}


// Functions
enum MaschinenStatus
{
    Bereit,
    Laeuft,
    Wartung,
    Defekt
}
interface IWartbar
{
    void Warte();
}
abstract class Maschine :IWartbar
{
    // Attribute
    public abstract double VerschleissProTick {get; } //Das abstrakte Attribut MUSS von einer erbenden Klasse overrided werden
    public string Name {get;set; }
    public double Verschleiss {get; set; }
    public MaschinenStatus Status {get; set;}
    
    public Maschine(string name)
    {
        Name = name;
        Verschleiss = 0;
        Status = MaschinenStatus.Bereit;
    }
    public void ZeigeStatus()
    {
        Console.WriteLine($"{Name} | Status: {Status} | Verschleiß: {Verschleiss}");
    }
    public void Arbeite()
    {
        // Wenn die Maschine defekt ist, kann sie nicht mehr arbeiten:
        if (Status == MaschinenStatus.Defekt)
        {
            return;
        }
        else
        {
            Status = MaschinenStatus.Laeuft;
        }
        // Verschleiss berechnen
        var _Verschleiss = Verschleiss + VerschleissProTick;
        // Defektbedingung prüfen
        if (_Verschleiss >= 100)
        {
            Status = MaschinenStatus.Defekt;
            Verschleiss = 100;
        }
        else
        {
            Verschleiss = _Verschleiss;
        }
    }
    public void Warte()
    {
        Verschleiss = 0;
        Status = MaschinenStatus.Bereit;
    }
}
// Klassen die von der abstrakten Maschine Klasse erben
class Fraesmaschine : Maschine
{
    public Fraesmaschine(string name): base(name) { }
    public override double VerschleissProTick => 1.5;
}

class Drehmaschine : Maschine
{
    public override double VerschleissProTick => 2.5;
    public Drehmaschine(string name) : base(name) {}
}
class Roboter : Maschine
{
    public override double VerschleissProTick => 0.5;
    public Roboter(string name) : base(name) {}
}
class Halle
{
    public List<Maschine> _maschinen = [];
    
    // Methoden
    public void AddMachine(Maschine machine)
    {
        _maschinen.Add(machine);
    }
    public void ShowAll()
    {
        foreach (var item in _maschinen)
        {
            // Console.WriteLine($"{item.Name},{item.Status},{item.Verschleiss}");
            item.ZeigeStatus();
        }
    }
    public void SimTick(int n_ticks = 1)
    {
        for (int i = 1; i <= n_ticks; i++)
        {
            foreach (var item in _maschinen)
            {
                item.Arbeite();    
            }    
        }
    } 
    public List<string> GetAllMachineNames()
    {
        List<string> machineNames = [];
        foreach (var item in _maschinen)
        {
            machineNames.Add(item.Name);
        }
        return machineNames;
    }
}