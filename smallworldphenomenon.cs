using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace project_v1
{
    public static class smallworldphenomenon
    {

        static int start = -1, end = -1, current_level = -1, newLvlNodes = 0, currLvlNodes = 0, current_strength, maxstren = 0;
        static string moviespath = "";


        static List<string> movies_chain = new List<string>();
        static List<string> current_mov_chain = new List<string>();


        static Queue<int> BFSQueue = new Queue<int>();
        static int[] level;
        static List<int>[] parents;

        static public HashSet<string>[] actormovies;

        //bonus
        static int[] frequency;

        public static void find_degree(Dictionary<int, HashSet<int>> neighbour_indices, bool optimization)
        {
            int count = neighbour_indices.Count;
            int start_index = start;
            BFSQueue.Enqueue(start);
            currLvlNodes = 1;
            while (BFSQueue.Count > 0)
            {
                start_index = BFSQueue.Dequeue();
                currLvlNodes--;

                foreach (var adj in neighbour_indices[start_index])
                {
                    //check if the child(adj) is not in the same level or a higher level in the tree than parent(start_index) 
                    if (level[adj] > level[start_index])
                    {
                        parents[adj].Add(start_index);

                        if (adj == end)
                        {
                            level[adj] = current_level + 1;
                            break;
                        }

                        if (level[adj] == count + 1)
                        {
                            BFSQueue.Enqueue(adj);
                            newLvlNodes++;
                        }
                        //set level of children to the next level from parent(increase degree)(Ex: Childern of A(level1) have level(1+1) ).
                        level[adj] = current_level + 1;

                    }
                }

                level[start_index] = current_level;
                // lw khls nodes el current level ynzl el level el ba3do.
                if (currLvlNodes == 0)
                {
                    current_level++;
                    if (optimization)
                    {
                        if (current_level > level[end])
                            break;
                    }
                    currLvlNodes = newLvlNodes;
                    newLvlNodes = 0;
                }
            }
        }
        //backtrack using DFS.
        public static void backtrack(int index)
        {
            if (index == start)
                return;

            foreach (var p in parents[index])
            {

                List<string> commons = actormovies[index].Intersect(actormovies[p]).ToList();

                int count = commons.Count;
                current_mov_chain.Add(commons[0]);

                current_strength += count;
                backtrack(p);

                if (current_strength > maxstren)
                {
                    maxstren = current_strength;
                    movies_chain.Clear();
                    movies_chain.AddRange(current_mov_chain);

                }
                current_mov_chain.RemoveAt(current_mov_chain.Count - 1);
                current_strength -= count;
            }
        }
        public static string findConnection(Dictionary<int, string> indexactors, Dictionary<int, HashSet<int>> neighbour_indices, int[] pairs, bool optimization, bool bonus)
        {

            //get consecutive pairs
            start = pairs[0];
            end = pairs[1];

            //start level of first node is level 0 and reintialize variables and clear lists
            level[start] = 0;
            current_level = 0;
            newLvlNodes = 0;
            currLvlNodes = 0;
            moviespath = "";
            BFSQueue.Clear();
            current_strength = 0;
            maxstren = 0;

            //degree and path.
            find_degree(neighbour_indices, optimization);
            backtrack(end);
          
            for (int i = movies_chain.Count - 1; i >= 0; i--)
            {
                moviespath += movies_chain[i] + "=>";
            }
            string answer = "";
            answer +=  "DoS = " + level[end]
            + ", RS = " + maxstren + "\n" + "CHAIN OF MOVIES: => " + moviespath + "\n";

            for (int row = 0; row < indexactors.Count; row++)
            {
                parents[row] = new List<int>();
                level[row] = indexactors.Count + 1;
            }
            return answer;

        }

        public static void structures_preparation(Dictionary<string, List<int>> movies, Dictionary<int, string> indexactors, Dictionary<string, int> actorsindex, Dictionary<int, HashSet<int>> neighbour_indices)
        {
            int size = indexactors.Count;
            parents = new List<int>[size];
            level = new int[size];
            actormovies = new HashSet<string>[size];
            for (int row = 0; row < size; row++)
            {
                parents[row] = new List<int>();
                actormovies[row] = new HashSet<string>();
                level[row] = size + 1;
            }
            string moviename = "";
            foreach (var movie in movies)
            {
                moviename = movie.Key;
                if (movies[moviename].Count > 1)
                    for (int i = 0, j = 0, k = movies[movie.Key].Count - 1; i < movies[movie.Key].Count;)//loop with two pointers on only half the matrix//k = movies[movie.Key].Count - 1
                    {
                        //movies[movie.key(nameelmovie)][i,k,j(actor)]
                        int row = movies[moviename][i];
                        if (i != j && j < movies[movie.Key].Count)
                        {
                            int col1 = movies[moviename][j];
                            neighbour_indices[col1].Add(row);
                            neighbour_indices[row].Add(col1);
                            actormovies[col1].Add(moviename);
                        }
                        if (i != k && k < movies[movie.Key].Count)
                        {
                            int col2 = movies[moviename][k];
                            neighbour_indices[row].Add(col2);
                            neighbour_indices[col2].Add(row);
                            actormovies[col2].Add(moviename);
                        }
                        j++;
                        k--;
                        //if they pass each other
                        if (j > k)
                        {
                            actormovies[row].Add(moviename);
                            i++;
                            j = i + 1;
                            k = movies[moviename].Count - 1;
                        }
                    }
            }

        }
        //Bonus
        public static void frequency_distribution(Dictionary<int, HashSet<int>> neighbour_indices, int query)
        {

            int count = neighbour_indices.Count;
            frequency = new int[count];
            frequency[0] = 1;
            for (int row = 0; row < count; row++)
            {
                level[row] = count + 1;
            }
            level[query] = 0;
            current_level = 0;
            frequency[current_level] = 1;
            newLvlNodes = 0;
            currLvlNodes = 1;
            BFSQueue.Clear();
            int start_index = query;
            BFSQueue.Enqueue(query);
            while (BFSQueue.Count > 0)
            {
                start_index = BFSQueue.Dequeue();
                currLvlNodes--;
                foreach (var adj in neighbour_indices[start_index])
                {
                    //check if the child(adj) is not in the same level or a higher level in the tree than parent(start_index) 
                    if (level[adj] > level[start_index])
                    {
                        if (level[adj] == count + 1)
                        {
                            BFSQueue.Enqueue(adj);
                            newLvlNodes++;
                        }
                        level[adj] = current_level + 1;
                    }
                }
                level[start_index] = current_level;
                // lw khls nodes el current level ynzl el level el ba3do.
                if (currLvlNodes == 0)
                {
                    current_level++;
                    currLvlNodes = newLvlNodes;
                    frequency[current_level] += newLvlNodes;
                    newLvlNodes = 0;
                }
            }
            int sum = 0;
            for (int i = 0; i < current_level + 1; i++)
            {
                if (frequency[i] == 0)
                {
                    Console.WriteLine(">" + (i - 1) + "\t " + (neighbour_indices.Count - sum));
                    break;
                }
                Console.WriteLine(i + "\t " + frequency[i]);
                sum += frequency[i];
            }
        }

    }

}





