namespace Budgetly.Models;

public class Account
{
    public int id { get; set; }
    public string name { get; set; }
    public decimal money { get; set; }
    
 }

public class Expenses
{
    public string Type { get; set; }
    public decimal Price { get; set; }
    public String Description { get; set; }
    public DateTime Date { get; set; }
    public bool Recurring{ get; set; }
}
public class Types {public string type { get; set; }}
