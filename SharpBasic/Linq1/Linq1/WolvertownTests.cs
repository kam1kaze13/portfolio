using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Linq1
{
    public class Company
    {
        public string Name { get; set; }

        public StaffInfo[] Staff { get; set; }
    }

    public class StaffInfo
    {
        public string Name { get; set; }

        public string Position { get; set; }
    }

    public enum Sex { Male, Female }

    public class Person
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public Sex Sex { get; set; }

        public PropertyInfo[] Properties { get; set; }
    }

    public enum PropertyType { Auto, RealEstate }

    public class PropertyInfo
    {
        public PropertyType Type { get; set; }

        public string Mark { get; set; }
    }

    public class Auto
    {
        public string Number { get; set; }

        public string Model { get; set; }

        public int Year { get; set; }
    }

    public class RealEstate
    {
        public string Address { get; set; }

        public int Storeys { get; set; }

        public int Area { get; set; }
    }

    public class WolvertownTests
    {
        private Company[] Companies;
        public Person[] People;
        private Auto[] Autos;

        public RealEstate[] RealEstates { get; private set; }

        [SetUp]
        public void Setup()
        {
            this.Companies = new[]
            {
                new Company
                {
                    Name = "Widget Co",
                    Staff = new []
                    {
                        new StaffInfo
                        {
                            Name = "Jonathan Smith",
                            Position = "CEO",
                        },
                        new StaffInfo
                        {
                            Name = "Mike Brown",
                            Position = "Project Manager",
                        },
                        new StaffInfo
                        {
                            Name = "Ann Jonson",
                            Position = "Analyst",
                        },
                        new StaffInfo
                        {
                            Name = "Mary Kobb",
                            Position = "Software Developer",
                        },
                    },
                },
                new Company
                {
                    Name = "New Vision Ltd",
                    Staff = new []
                    {
                        new StaffInfo
                        {
                            Name = "Donald Brown",
                            Position = "CEO",
                        },
                        new StaffInfo
                        {
                            Name = "Matt Jonson",
                            Position = "Designer",
                        },
                        new StaffInfo
                        {
                            Name = "Dave Kobb",
                            Position = "Designer",
                        },
                    },
                },
            };

            this.People = new[]
            {
                new Person
                {
                    Name = "Matt Jonson",
                    Age = 34,
                    Sex = Sex.Male,
                    Properties = new[]
                    {
                        new PropertyInfo
                        {
                            Type = PropertyType.Auto,
                            Mark = "e123ka",
                        },
                        new PropertyInfo
                        {
                            Type = PropertyType.RealEstate,
                            Mark = "Downing st, 15",
                        },
                    },
                },
                new Person
                {
                    Name = "Ann Jonson",
                    Age = 27,
                    Sex = Sex.Female,
                    Properties = new[]
                    {
                        new PropertyInfo
                        {
                            Type = PropertyType.Auto,
                            Mark = "t994ha",
                        },
                        new PropertyInfo
                        {
                            Type = PropertyType.RealEstate,
                            Mark = "Downing st, 15",
                        },
                    },
                },
                new Person
                {
                    Name = "Mike Jonson",
                    Age = 5,
                    Sex = Sex.Male,
                    Properties = new PropertyInfo[] { },
                },
                new Person
                {
                    Name = "Jonathan Smith",
                    Age = 46,
                    Sex = Sex.Male,
                    Properties = new[]
                    {
                        new PropertyInfo
                        {
                            Type = PropertyType.Auto,
                            Mark = "DONAT",
                        },
                        new PropertyInfo
                        {
                            Type = PropertyType.Auto,
                            Mark = "PENS",
                        },
                        new PropertyInfo
                        {
                            Type = PropertyType.RealEstate,
                            Mark = "Cover st, 3",
                        },
                    },
                },
                new Person
                {
                    Name = "Dave Kobb",
                    Age = 27,
                    Sex = Sex.Male,
                    Properties = new[]
                    {
                        new PropertyInfo
                        {
                            Type = PropertyType.Auto,
                            Mark = "g481pc",
                        },
                        new PropertyInfo
                        {
                            Type = PropertyType.RealEstate,
                            Mark = "Downing st, 17",
                        },
                    },
                },
                new Person
                {
                    Name = "Mary Kobb",
                    Age = 28,
                    Sex = Sex.Female,
                    Properties = new[]
                    {
                        new PropertyInfo
                        {
                            Type = PropertyType.Auto,
                            Mark = "n833fd",
                        },
                        new PropertyInfo
                        {
                            Type = PropertyType.RealEstate,
                            Mark = "Downing st, 17",
                        },
                    },
                },
                new Person
                {
                    Name = "Mike Brown",
                    Age = 45,
                    Sex = Sex.Male,
                    Properties = new[]
                    {
                        new PropertyInfo
                        {
                            Type = PropertyType.Auto,
                            Mark = "k582yt",
                        },
                        new PropertyInfo
                        {
                            Type = PropertyType.RealEstate,
                            Mark = "Downing st, 134",
                        },
                    },
                },
                new Person
                {
                    Name = "Ann Brown",
                    Age = 42,
                    Sex = Sex.Female,
                    Properties = new[]
                    {
                        new PropertyInfo
                        {
                            Type = PropertyType.RealEstate,
                            Mark = "Downing st, 134",
                        },
                    },
                },
                new Person
                {
                    Name = "David Brown",
                    Age = 4,
                    Sex = Sex.Male,
                    Properties = new PropertyInfo[] { },
                },
                new Person
                {
                    Name = "Elli Brown",
                    Age = 12,
                    Sex = Sex.Female,
                    Properties = new PropertyInfo[] { },
                },
                new Person
                {
                    Name = "Richard Brown",
                    Age = 14,
                    Sex = Sex.Male,
                    Properties = new PropertyInfo[] { },
                },
                new Person
                {
                    Name = "Donald Brown",
                    Age = 67,
                    Sex = Sex.Male,
                    Properties = new[]
                    {
                        new PropertyInfo
                        {
                            Type = PropertyType.Auto,
                            Mark = "OPENER",
                        },
                        new PropertyInfo
                        {
                            Type = PropertyType.RealEstate,
                            Mark = "Cover st, 6",
                        },
                    },
                },
            };


            this.Autos = new[]
            {
                new Auto
                {
                    Number = "e123ka",
                    Model = "Ford",
                    Year = 2017,
                },
                new Auto
                {
                    Number = "t994ha",
                    Model = "Toyota",
                    Year = 2009,
                },
                new Auto
                {
                    Number = "DONAT",
                    Model = "Lexus",
                    Year = 2019,
                },
                new Auto
                {
                    Number = "PENS",
                    Model = "Bentley",
                    Year = 2003,
                },
                new Auto
                {
                    Number = "g481pc",
                    Model = "Volkswagen",
                    Year = 2012,
                },
                new Auto
                {
                    Number = "n833fd",
                    Model = "Ford",
                    Year = 2016,
                },
                new Auto
                {
                    Number = "k582yt",
                    Model = "Volkswagen",
                    Year = 2012,
                },
                new Auto
                {
                    Number = "OPENER",
                    Model = "Lexus",
                    Year = 2009,
                },
            };

            this.RealEstates = new[]
            {
                new RealEstate
                {
                    Address = "Cover st, 3",
                    Area = 620,
                    Storeys = 3,
                },
                new RealEstate
                {
                    Address = "Cover st, 6",
                    Area = 560,
                    Storeys = 4,
                },
                new RealEstate
                {
                    Address = "Downing st, 15",
                    Area = 190,
                    Storeys = 2,
                },
                new RealEstate
                {
                    Address = "Downing st, 17",
                    Area = 180,
                    Storeys = 2,
                },
                new RealEstate
                {
                    Address = "Downing st, 134",
                    Area = 280,
                    Storeys = 3,
                },
            };
        }

        [Test]
        public void Test1()
        {
            // calculate average count of staff in companies
            var query = Companies.Select(company => company.Staff.Count()).Average();
            Assert.AreEqual(3.5, query);
        }

        [Test]
        public void Test2()
        {
            // which number of female people have age lesser than 40
            var query = People.Where(people => people.Sex == Sex.Female)
                               .Where(people => people.Age < 40)
                               .Select(people => people).Count();
            Assert.AreEqual(3, query);
        }

        public int CalcPrice(int area, int storeys)
        {
            int price = storeys == 2 ? area * 2000 : storeys == 3 ? area * 1800 : area * 2300;
            return price;
        }

        [Test]
        public void Test3()
        {
            // caluculate price of each Real Estate if
            // 1) average price of square metre for 2-storeys building is 2000
            // 2) average price of square metre for 3-storeys building is 1800
            // 3) average price of square metre for 4-storeys building is 2300
            List<int> costs = new() { 1116000, 1288000, 380000, 360000, 504000 };

            var query = RealEstates.Select(estate => estate);
           
            int i = 0;
            foreach (var house in query)
            {
                Assert.AreEqual(costs[i], CalcPrice(house.Area, house.Storeys));
                i++;
            }
        }

        [Test]
        public void Test4()
        {
            // what car models do CEOs have
            var query = Companies.SelectMany(comp => comp.Staff)
                                       .Where(staff => staff.Position == "CEO")
                                       .Join(People,
                                             staff => staff.Name,
                                             ppl => ppl.Name,
                                             (staff, ppl) => new { Staff = staff, People = ppl })
                                       .SelectMany(ppl => ppl.People.Properties)
                                       .Where(prop => prop.Type == PropertyType.Auto)
                                       .Join(Autos,
                                            prop => prop.Mark,
                                            auto => auto.Number,
                                            (prop, auto) => new { People = prop, Car = auto })
                                       .Select(car => car.Car.Model)
                                       .Distinct();

            Assert.AreEqual(2, query.Count());
        }

        [Test]
        public void Test5()
        {
            // what streets are there
            var queryNew = People.SelectMany(ppl => ppl.Properties)
                                   .Where(p => p.Type == PropertyType.RealEstate)
                                   .Select(p => p.Mark.Substring(0, p.Mark.IndexOf(" ")))
                                   .Distinct();

            Assert.AreEqual(2, queryNew.Count());
        }

        [Test]
        public void Test6()
        {
            // sort people by price of Real Estate they are living
            var queryNew = People.SelectMany(ppl => ppl.Properties, (ppl, prop) => new { ppl, prop})
                                   .Where(p => p.prop.Type == PropertyType.RealEstate)
                                   .Join(RealEstates,
                                        p => p.prop.Mark,
                                        est => est.Address,
                                        (p, est) => new { People = p.ppl.Name, Estate = est })
                                   .Select(estate => new { cost = CalcPrice(estate.Estate.Area, estate.Estate.Storeys), name = estate.People})
                                   .OrderBy(estate => estate.cost)
                                   .Select(ppl => ppl.name);

            Assert.AreEqual(8, queryNew.Count());

            List<string> peoples = new() { "Dave Kobb", "Mary Kobb", "Matt Jonson", "Ann Jonson", "Mike Brown", "Ann Brown", "Jonathan Smith", "Donald Brown" };
        }

        [Test]
        public void Test7()
        {
            // generate family list which contains properties:
            // LastName
            // Children (count of children)
            // Autos (count of autos)
            // AverageArea (average count of Real Estate's area per family member)
            //
            // if family is pair of man and women which are living in the same real estate and have same Last Name and are adult

            var query = from male in People
                        from propertyMale in male.Properties
                        from female in People
                        from propertyFemale in female.Properties
                        where male.Sex == Sex.Male
                        where male.Age >= 18
                        where female.Sex == Sex.Female
                        where female.Age >= 18
                        where male.Name.Substring(male.Name.IndexOf(" ") + 1) == female.Name.Substring(female.Name.IndexOf(" ") + 1)
                        where propertyMale.Type == PropertyType.RealEstate
                        where propertyFemale.Type == PropertyType.RealEstate
                        where propertyMale.Mark == propertyFemale.Mark
                        let childCount = (from people in People
                                          where people.Name.Substring(people.Name.IndexOf(" ") + 1) == male.Name.Substring(male.Name.IndexOf(" ") + 1)
                                          where people.Age < 18
                                          select new { people }).Count()
                        let autoCount = (from people in People
                                         from property in people.Properties
                                         where people.Name.Substring(people.Name.IndexOf(" ") + 1) == male.Name.Substring(male.Name.IndexOf(" ") + 1)
                                         where property.Type == PropertyType.Auto
                                         select new { people }).Count()
                        let area = (from estate in RealEstates
                                    where propertyMale.Mark == estate.Address
                                    select new { area = estate.Area / (childCount + 2) })
                        select new
                        {
                            lastName = male.Name.Substring(male.Name.IndexOf(" ") + 1),
                            children = childCount,
                            cars = autoCount,
                            avgArea = area
                        };

            List<string> families = new() { "Jonson", "Kobb", "Brown" };
            int i = 0;
            foreach (var family in query)
            {
                Assert.AreEqual(families.ElementAt(i), family.lastName);
                i++;
            }
        }
    }
}