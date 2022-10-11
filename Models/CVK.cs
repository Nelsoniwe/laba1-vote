using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace laba1_vote.Models
{
    public class CVK
    {
        UnicodeEncoding byteConverter = new UnicodeEncoding();

        public string Key { get; set; } = "7F";

        public List<Person> People { get; set; } = new List<Person>();

        private List<Vote> Votes { get; set; } = new List<Vote>();

        private string personsPath;
        private string votesPath;

        public CVK(string PersonsPath, string VotesPath)
        {
            personsPath = PersonsPath;
            votesPath = VotesPath;

        }

        public void Load()
        {
            using (StreamReader r = new StreamReader(personsPath))
            {
                string json = r.ReadToEnd();
                People = JsonSerializer.Deserialize<List<Person>>(json);
            }

            using (StreamReader r = new StreamReader(votesPath))
            {
                string json = r.ReadToEnd(); 
                Votes = JsonSerializer.Deserialize<List<Vote>>(json);
            }
        }


        public void Save()
        {
            string jsonPeople = JsonSerializer.Serialize(People);
            File.WriteAllText(personsPath, jsonPeople);

            string jsonVotes = JsonSerializer.Serialize(Votes);
            File.WriteAllText(votesPath, jsonVotes);
        }

        public bool Vote(string vote, byte[] signature, RSAParameters publicKey)
        {
            var decriptedVote = Xor.Decrypt(vote, Key);

            var decriptedVoteSplitted = Xor.Decrypt(vote, Key).Split(' ');

            var who = decriptedVoteSplitted[0];
            var forWho = decriptedVoteSplitted[1];

            RSACryptoServiceProvider RSACVK = new RSACryptoServiceProvider();

            RSACVK.ImportParameters(publicKey);

            if (RSACVK.VerifyData(byteConverter.GetBytes(decriptedVote), CryptoConfig.MapNameToOID("SHA256"), signature))
            {
                var applicant = People.FirstOrDefault(x => x.Id == forWho && x.Role == Role.applicant);
                var vater = People.FirstOrDefault(x => x.Id == who && x.Role == Role.voter);

                if (applicant != null && vater != null)
                {
                    if (vater.Voted == false && Votes.FirstOrDefault(x => x.Who == vater.Id) == null)
                    {
                        Votes.Add(new Vote(who, forWho));
                        People.FirstOrDefault(x => x.Id == vater.Id).Voted = true;
                        return true;
                    }
                }
            }

            return false;
        }

        public int GetVotesById(string id)
        {
            var applicant = People.FirstOrDefault(x => x.Id == id && x.Role == Role.applicant);
            if (applicant != null)
                return Votes.Where(x => x.ForWho == id).Count();
            return 0;
        }
    }
}
