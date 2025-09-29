namespace Budgetly.Models;
using System;

using System.ComponentModel.DataAnnotations;

public class Account
{
    public int id { get; set; }
    public string name { get; set; }
    public decimal money { get; set; }
}
public class HubViewModel
{
    public Account Account { get; set; } = new Account();
    public List<Expenses> Expenses { get; set; } = new List<Expenses>();
}
public class Expenses
{
    [Required(ErrorMessage = "Type is required")]
    public string Type { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    public string? Description { get; set; } // opcional

    [Required(ErrorMessage = "Date is required")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Recurring field is required")]
    public bool Recurring { get; set; }

    [Required(ErrorMessage = "User ID is required")]
    public int id_user { get; set; }

    public DateTime? NextDueDate { get; set; } // opcional
}


