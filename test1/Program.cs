using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Util.Profiling;

namespace test1
{
    class Program
    {

        static JC[] jclist = new JC[]
        {
//            new JC("navn", 1),
//            new JC("navn-igen", 2)

new JC("Ishøj",18300),
new JC("Jammerbugt",84900),
new JC("Kalundborg",32600),
new JC("Kerteminde",44000),
new JC("Kolding",62100),
new JC("København",10100),
new JC("Køge",25900),
new JC("Langeland",48200),
new JC("Lejre",35000),
new JC("Lemvig",66500),
new JC("Lolland",36000),
new JC("Lyngby-Taarbæk",17300),
new JC("Læsø",82500),
new JC("Mariagerfjord",84600),
new JC("Middelfart",41000),
new JC("Morsø",77300),
new JC("Norddjurs",70700),
new JC("Nordfyn",48000),
new JC("Nyborg",45000),
new JC("Næstved (Pilot)",37000),
new JC("Odder",72700),
new JC("Odense (Pilot)",46100),
new JC("Odsherred",30600),
new JC("Randers",73000),
/*
new JC("Rebild (Pilot)",84000),
new JC("Ringkøbing-Skjern",76000),
new JC("Ringsted",32900),
new JC("Roskilde",26500),
new JC("Rudersdal (Pilot)",23000),
new JC("Rødovre",17500),
new JC("Samsø",74100),
new JC("Silkeborg",74000),
new JC("Skanderborg (Pilot)",74600),
new JC("Skive",77900),
new JC("Slagelse",33000),
new JC("Solrød",26900),
new JC("Sorø",34000),
new JC("Stevns",33600),
new JC("Struer",67100),
new JC("Svendborg",47900),
new JC("Syddjurs",70600),
new JC("Sønderborg (Pilot)",54000),
new JC("Thisted (Pilot)",78700),
new JC("Tønder",55000),
new JC("Tårnby",18500),
new JC("Vallensbæk",18700),
new JC("Varde",57300),
new JC("Vejen",57500),
new JC("Vejle (Pilot)",63000),
new JC("Vesthimmerland",82000),
new JC("Viborg",79100),
new JC("Vordingborg",39000),
new JC("Ærø",49200),
new JC("Aabenraa",58000),
new JC("Aalborg",85100),
new JC("Århus",75100)
*/
        };
        
        
        class JC
        {
            public string name { get; set; }
            public int code { get; set; }

            public JC(string name, int code)
            {
                this.name = name;
                this.code = code;
            }
        }

        static void Main(string[] args)
        {

            using (StreamWriter file = new StreamWriter(@"C:\Users\Torben\Downloads\sql.txt", false, Encoding.UTF8))
            {
                file.WriteLine("use Brokerservices");
                file.WriteLine("go");

                file.WriteLine("if (@@SERVERNAME = 'BITOLTPCL201')");
                file.WriteLine("begin");

                foreach (JC j in jclist)
                {

                    file.WriteLine("\tif not exists(select * from tblAllowedJobplanJobcenters where JobcenterCode = " + j.code + ")");
                    file.WriteLine("\tbegin");
                    file.WriteLine("\t\tinsert into tblAllowedJobplanJobcenters	values ('" + j.name + "'," + j.code + ")");
                    file.WriteLine("\tend");
                }
                file.WriteLine("end");
            }

            Console.ReadKey();
        }

        public static void Out(string s) {
           Console.WriteLine(s);
        }
    }
}
