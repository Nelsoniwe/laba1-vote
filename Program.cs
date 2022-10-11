using laba1_vote.Models;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace laba1_vote
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CVK cvk = new CVK(@"C:\Users\Denys\Desktop\Persons.json", @"C:\Users\Denys\Desktop\Votes.json");
            cvk.Load();

            Person currentUser = null;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Choose Person\n2. Create Person\n3. Delete Person\n4. Watch Results\n5. exit");
                string action = Console.ReadLine();

                if (action == "1")
                {
                    Console.Clear();

                    if (cvk.People.FindAll(x=>x.Role == Role.voter).Count>0)
                    {

                        var people = cvk.People.FindAll(x => x.Role == Role.voter);
                        foreach (var item in people)
                        {
                            Console.WriteLine($"{item.Id} Role: {item.Role} Name: {item.Name} Surname: {item.Surname}");
                        }

                        Console.WriteLine("Choose person");
                        var id = Console.ReadLine();
                        var result = cvk.People.FirstOrDefault(x => x.Id == id);
                        if (result != null && result.Role == Role.voter)
                        {
                            currentUser = result;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Person don't exist");
                            Console.ReadLine();
                        }

                    }
                    else
                    {
                        Console.WriteLine("People don't exist");
                        Console.ReadLine();
                    }
                }
                if (action == "2")
                {
                    Console.Clear();
                    Console.WriteLine("Write a name:");
                    var name = Console.ReadLine();
                    Console.WriteLine("Write a surname:");
                    var surname = Console.ReadLine();

                    Console.WriteLine($"Write a role: ({Role.applicant}, {Role.voter})");
                    var role = Console.ReadLine();

                    if (name != "" && surname != "" && Enum.IsDefined(typeof(Role), role))
                    {
                        var random = new Random();
                        string id = random.Next(100000000).ToString();

                        while (cvk.People.FirstOrDefault(x=>x.Id == id) != null)
                        {
                            id = random.Next(1000000).ToString();
                        }

                        cvk.People.Add(new Person(id, name, surname, (Role)Enum.Parse(typeof(Role), role)));
                        continue;
                    }
                }
                if (action == "3")
                {
                    Console.Clear();
                    var people = cvk.People;

                    if (people.Count == 0)
                    {
                        Console.WriteLine("People don't exist");
                        Console.ReadLine();
                        continue;
                    }

                    foreach (var item in people)
                    {
                        Console.WriteLine($"{item.Id} Role: {item.Role} Name: {item.Name} Surname: {item.Surname}");
                    }

                    Console.WriteLine("Choose person");

                    var id = Console.ReadLine();
                    var result = cvk.People.FirstOrDefault(x => x.Id == id);
                    if (result != null)
                    {
                        cvk.People.Remove(result);
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Person doesn't exist");
                        Console.ReadLine();
                    }
                }
                if (action == "4")
                {
                    var people = cvk.People.FindAll(x => x.Role == Role.applicant);

                    foreach (var item in people)
                    {
                        Console.WriteLine($"Id: {item.Id} Name: {item.Name} Surname: {item.Surname} Votes: {cvk.GetVotesById(item.Id)}");
                    }
                    Console.ReadLine();
                    continue;
                }
                if(action == "5")
                {
                    break;
                }
                else
                {
                    Console.Clear();
                    continue;
                }
            }

            while (true)
            {
                Console.Clear();
                if (currentUser == null)
                {
                    Console.WriteLine("Current user doesn't exist");
                    break;
                }

                if (currentUser.Voted == true && currentUser.Permission != true)
                {
                    Console.WriteLine("Current user can't vote");
                    break;
                }

                else
                {
                    Console.WriteLine("Write id of the applicant you want to vote for");

                    var people = cvk.People.FindAll(x => x.Role == Role.applicant);

                    foreach (var item in people)
                    {
                        Console.WriteLine($"Id: {item.Id} Name: {item.Name} Surname: {item.Surname}");
                    }

                    var id = Console.ReadLine();
                    var choosenApplicant = cvk.People.FirstOrDefault(x => x.Id == id);

                    if (choosenApplicant == null)
                    {
                        Console.WriteLine("Applicant doesn't exist");
                        Console.ReadLine();
                        break;
                    }
                    else
                    {
                        //cvk.Vote(currentUser.Id,choosenApplicant.Id);
                        string vote = $"{currentUser.Id} {choosenApplicant.Id}";
                        RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                        UnicodeEncoding unicodeEncoding = new UnicodeEncoding();

                        var signature = RSA.SignData(unicodeEncoding.GetBytes(vote),CryptoConfig.MapNameToOID("SHA256"));
                        vote = Xor.Encrypt(vote, cvk.Key);

                        var publicKey = RSA.ExportParameters(false);

                        var result = cvk.Vote(vote,signature, publicKey);

                        Console.WriteLine("Success");
                        Console.ReadLine();
                        break;
                    }
                }
            }

            cvk.Save();

        }
    }
}
