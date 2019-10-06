using System.Collections.Generic;
using Domain;


// "ASSET FINANCING", "Private equity", "Angel investment","Wire transfer","Mutual Funds","CDS",
// "CDO", "Export Financing","Debt Capital Markets","Strategic Acquisition Finance","Rating Advisory"

public class Client : EntityBase
{
    public string Name { get; set; }
    public string Level { get; set; }
    public IEnumerable<Sales> Sales { get; set; }
}

public class Sales
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Revenue { get; set; }
    public IEnumerable<string> Products { get; set; }
}

public static class ClientSeed
{
    public static readonly IEnumerable<Client> Data = new List<Client>
    {
        new Client
        {
            Name = "BNP",
            Level = "grp",
            Sales = new[]
            {
                new Sales
                {
                    Firstname = "Amber",
                    Lastname = "Duke",
                    Revenue = "1",
                    Products = new[] {"CDS", "CDO", "Export Financing"}
                }
            }
        },
        new Client
        {
            Name = "SFR",
            Level = "LE",
            Sales = new[]
            {
                new Sales
                {
                    Firstname = "Hattie",
                    Lastname = "Bond",
                    Revenue = "1",
                    Products = new[] {"ASSET FINANCING", "Private equity", "Angel investment"}
                }
            }
        },
        new Client
        {
            Name = "Bouygues Télécom",
            Level = "EC",
            Sales = new[]
            {
                new Sales
                {
                    Firstname = "Nanette",
                    Lastname = "Bates",
                    Revenue = "3",
                    Products = new[] {"Private equity", "Angel investment", "Wire transfer", "CDO"}
                }
            }
        },
        new Client
        {
            Name = "Amundi",
            Level = "grp",
            Sales = new[]
            {
                new Sales
                {
                    Firstname = "Dale",
                    Lastname = "Adams",
                    Revenue = "3",
                    Products = new[] {"CDS", "CDO", "Wire transfer"}
                }
            }
        },
        new Client
        {
            Name = "Blackrock",
            Level = "grp",
            Sales = new[]
            {
                new Sales
                {
                    Firstname = "Elinor",
                    Lastname = "Ratliff",
                    Revenue = "5",
                    Products = new[] {"CDS", "ASSET FINANCING", "Cash managment"}
                }
            }
        },
        new Client
        {
            Name = "Bloomberg",
            Level = "LE",
            Sales = new[]
            {
                new Sales
                {
                    Firstname = "P1",
                    Lastname = "L2",
                    Revenue = "4",
                    Products = new[] {"CDS", "FINANCING", "Cash managment"}
                }
            }
        }
    };
}