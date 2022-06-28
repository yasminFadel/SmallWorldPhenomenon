using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace project_v1
{
    class Program
    {
        static string path = ""; static Queue<string> expectedanswers = new Queue<string>();
        public static void solution()
        {
            StreamReader sr = new StreamReader(path);
            string temp = "", line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line == "")
                {
                    expectedanswers.Enqueue(temp);
                    temp = "";
                }
                else temp += line + "\n";
            }
        }
        static void Main(string[] args)
        {


            bool optimization = false;
            bool bonus = false;

            // Code for interface choices.
            string Queryfile = "", casechoice = "";
            string moviefile = "";
            Console.WriteLine("Please Choose Movies file : S for small /M for meduim /L for large /E for extreme");
            string filechoice = Console.ReadLine();
            if(filechoice == "s" || filechoice == "S")
            {
                Console.WriteLine("193 or 187");
                casechoice = Console.ReadLine();

                if (casechoice == "193")
                    moviefile = "Movies193.txt";
                
                else if (casechoice == "187")
                    moviefile = "Movies187.txt";
                
            }
            else if(filechoice == "M" || filechoice == "m")
            {
                Console.WriteLine("967 or 4736");
                casechoice = Console.ReadLine();

                if (casechoice == "967")
                    moviefile = "Movies967.txt";
                
                else if (casechoice == "4736")
                    moviefile = "Movies4736.txt";  
            } 
            else if(filechoice == "L" || filechoice == "l")
                moviefile = "Movies14129.txt";
            
            else if(filechoice == "E" || filechoice == "e")
                moviefile = "Movies122806.txt";
            

            Console.WriteLine("Choose 1 for program, 2 for Bonus: ");
            string choice = Console.ReadLine();
            if (choice == "1")
            {
                Console.WriteLine("Write query number");
                string number = Console.ReadLine();
                if (number == "110")
                {
                    if (filechoice == "s" || filechoice == "S")
                    {
                        Queryfile = "small";
                        path = "small";
                    }
                    else if (filechoice == "m" || filechoice == "M")
                    {
                        Queryfile = "medium";
                        path = "medium";
                    }
                }
                Queryfile += "queries" + number + ".txt";
                path = Path.GetFullPath(path+"queries" + number + " - Solution.txt");

                solution();
                Console.WriteLine("Do you want optimizated y/n");

                string answer = Console.ReadLine();
                if (answer == "y" || answer == "Y")
                    optimization = true;
                
                else if (answer == "n" || answer == "N")
                    optimization = false;
                
            }
            else if (choice == "2")
                bonus = true;
            


            //Start of actual program after choices.
            //clear console to start program.
            Console.Clear();
            Stopwatch sw = Stopwatch.StartNew();
            FileStream file;
            StreamReader sr;
            string line;

            file = new FileStream(moviefile, FileMode.Open, FileAccess.Read);
            sr = new StreamReader(file);
            line = null;
            Dictionary<string, List<int>> movies = new Dictionary<string, List<int>>();
            Dictionary<string, int> actorsindex = new Dictionary<string, int>();
            Dictionary<int, string> indexactors = new Dictionary<int, string>();
            Dictionary<int, HashSet<int>> neighbour = new Dictionary<int, HashSet<int>>();

            Console.WriteLine("Preparing Graph.....\n");
            int actor_counter = 0;
            while ((line = sr.ReadLine()) != null)
            {
                string[] splittline = line.Split('/');
                string key = splittline[0];
                string actor = "";
                movies.Add(key, new List<int>());

                for (int k = 1; k < splittline.Length; k++)
                {

                    actor = splittline[k];
                    if (!actorsindex.ContainsKey(actor))
                    {
                        actorsindex.Add(actor, actor_counter);
                        indexactors.Add(actor_counter, actor);
                        neighbour.Add(actor_counter, new HashSet<int>());
                        actor_counter++;
                    }
                    movies[key].Add(actorsindex[actor]);

                }
            }
            smallworldphenomenon.structures_preparation(movies, indexactors, actorsindex, neighbour);

            sr.Close();
            sr.Dispose();

            //Bonus
            if (bonus == true)
            {
                Console.Write("Query actor : ");
                string actor = Console.ReadLine();
                int start = actorsindex[actor];
                smallworldphenomenon.frequency_distribution(neighbour, start);
                return;
            }

            //program
            file = new FileStream(Queryfile, FileMode.Open, FileAccess.Read);
            sr = new StreamReader(file);
            int[] pairs = new int[2];
            int query_num = 0;
            bool congrats = true;
            while ((line = sr.ReadLine()) != null)
            {
                query_num++;

                string[] splittedline = line.Split('/');

                pairs[0] = actorsindex[splittedline[0].ToString()];
                pairs[1] = actorsindex[splittedline[1].ToString()];

                Console.WriteLine(splittedline[0].ToString() + "/ " + splittedline[1].ToString());

                string expected = expectedanswers.Dequeue();
                string[] expectedanswer = expected.Split('\n');

                string actanswer = smallworldphenomenon.findConnection(indexactors, neighbour, pairs, optimization, bonus);
                string[] actualanswer = actanswer.Split('\n');
                Console.WriteLine(actanswer);

                if (expectedanswer[1] != actualanswer[0])
                {
                    Console.WriteLine("expected " + expected);
                    Console.WriteLine("WrongAnswer at " + query_num);
                    congrats = false;
                    break;
                }


            }
            sw.Stop();
            Console.WriteLine("Total Elapsed Time is "  + sw.Elapsed.TotalSeconds + " seconds");

            if (congrats == true)
                Console.WriteLine("LOLOLOLOLOLYYYYYY!!!!!!!");

            sr.Close();
            sr.Dispose();

        }
    }
}
