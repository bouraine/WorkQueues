using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Nest;


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
    public int Revenue { get; set; }
    public IEnumerable<string> Products { get; set; }
}


public static class ClientSeed
{
    public static readonly IReadOnlyCollection<Client> Data = new List<Client>
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
                    Revenue = 1,
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
                    Revenue = 1,
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
                    Revenue = 3,
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
                    Revenue = 3,
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
                    Firstname = "Elinor 2",
                    Lastname = "Ratliff 2",
                    Revenue = 5,
                    Products = new[] {"CDS", "ASSET FINANCING", "Cash managment 2"}
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
                    Revenue = 4,
                    Products = new[] {"CDS", "FINANCING", "Cash managment"}
                }
            }
        }
    };

    public static readonly Client SingleClient = new Client
    {
        Id = "1" + DateTime.Now.ToFileTime(),
        Level = "L2",
        Name = "name",
        Sales = new List<Sales>
        {
            new Sales
            {
                Firstname = "firstname" + DateTime.Now.ToFileTime()
            }
        }
    };
}