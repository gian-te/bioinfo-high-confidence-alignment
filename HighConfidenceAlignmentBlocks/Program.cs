﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace HighConfidenceAlignmentBlocks
{
    class Program
    {
            
        /*
         * Input:
         * Coded Alignment
         *
         * Output:
         * "High-confidence alignment blocks were identified within the multiple sequence alignment (MSA), which were defined as regions LONGER THAN  15 nt, containing less than 40% gaps IN EACH POSITION"
         */
        static void Main(string[] args)
        {
            // column with more than or equal to 40%, throw out 15 columns to the left AND to the right???????
            // approach 1: at least 16nt long regions where each column has less than 40% gaps
            Approach1();

            // approach 2: remove flanks of columns that have more than or equal to 40% gaps 15nt to the left and 15nt to the right
            //Approach2();

            // approach 3: remove flanks of columns that have less than 40% gaps 15nt to the left and 15nt to the right
            //Approach3();
        }
        private static void Approach3()
        {
            try
            {
                Dictionary<int, double> gapRatios = new Dictionary<int, double>();

                List<List<string>> genomes = new List<List<string>>();
                List<List<string>> highConfidenceBlocks = new List<List<string>>();

                // load the data into the genomes List of List
                #region LOAD DATA
                using (var reader = new StreamReader(@"C:\Users\vziex\Desktop\DLSU\BIOINFO\Bioinfo Report 2\new_dataset_final.txt"))
                {

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line.StartsWith(">"))
                        {
                            continue;
                        }
                        var values = line.Split(',').ToList();
                        values = values.Skip(1).ToList();
                        genomes.Add(values);
                    }
                }
                #endregion

                List<int> allColumns = new List<int>();
                List<int> goodColumns = new List<int>();
                for (int i = 0; i < 40059; i++) // for every column
                {
                    Console.WriteLine("Processing column {0}", i);
                    var ratio = GetColumnRatios(genomes, i, 1, gapRatios);
                    if (ratio <= 0.40)
                    {
                        goodColumns.Add(i);
                    }
                    allColumns.Add(i);
                }

                Console.WriteLine("Desired columns: {0}", goodColumns.Count);

                foreach (var goodColumn in goodColumns)
                {
                    var start = goodColumn - 15;
                    var end = goodColumn + 15;
                    // remove the flanks
                    for (int i = start; i <= end; i++)
                    {
                        allColumns.Remove(i);
                    }
                }

                //var goodColumns = allColumns;
                // identify regions, we are hoping here that we get regions that are close to the regions detected in supporting table 1
                List<Tuple<int, int>> positions = new List<Tuple<int, int>>();
                int blockStart = 0;
                for (int i = 0; i < goodColumns.Count;)
                {
                    if (i + 1 >= goodColumns.Count)
                    {
                        break;
                    }

                    if (goodColumns[i + 1] == goodColumns[i] + 1)
                    {
                        i += 1;
                    }
                    else
                    {
                        positions.Add(new Tuple<int, int>(goodColumns[blockStart], goodColumns[i]));
                        i += 1;
                        if (i < goodColumns.Count)
                        {
                            blockStart = i;
                        }
                    }
                }

                //var fileCounter = 0;
                //foreach (var ranges in positions)
                //{
                //    // 944 rows
                //    var fileText = "";
                //    for (int i = 0; i < 944; i++)
                //    {
                //        var row = string.Join(",", genomes[i].GetRange(ranges.Item1, ranges.Item2 - ranges.Item1 + 1));
                //        fileText += row;
                //        fileText += Environment.NewLine;
                //    }

                //    System.IO.File.WriteAllText(string.Format("file{0}.txt", fileCounter), fileText);
                //    // 1 file per range
                //    fileCounter++;
                //}

                Console.WriteLine("Good columns after subtacting the flanks: {0}", goodColumns.Count);
                var findRegion1 = goodColumns.Where(x => x >= 7281 && x <= 7291).Count();
                var findRegion2 = goodColumns.Where(x => x >= 10446 && x <= 10451).Count();
                var findRegion3 = goodColumns.Where(x => x >= 14249 && x <= 14249).Count();
                var findRegion4 = goodColumns.Where(x => x >= 25041 && x <= 25108).Count();
                var findRegion5 = goodColumns.Where(x => x >= 29498 && x <= 29498).Count();
                var findRegion6 = goodColumns.Where(x => x >= 32029 && x <= 32040).Count();
                var findRegion7 = goodColumns.Where(x => x >= 32906 && x <= 32927).Count();
                var findRegion8 = goodColumns.Where(x => x >= 36459 && x <= 36462).Count();
                var findRegion9 = goodColumns.Where(x => x >= 38798 && x <= 38808).Count();
                var findRegion10 = goodColumns.Where(x => x >= 38926 && x <= 38941).Count();
                var findRegion11 = goodColumns.Where(x => x >= 39356 && x <= 39360).Count();

                Console.WriteLine("Ratio of sequences in blocks over the entire MSA: {0}%", ((double)(allColumns.Count * 944)) / (944 * 40059) * 100);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private static void Approach2()
        {
            try
            {
                Dictionary<int, double> gapRatios = new Dictionary<int, double>();

                List<List<string>> genomes = new List<List<string>>();
                List<List<string>> highConfidenceBlocks = new List<List<string>>();

                // load the data into the genomes List of List
                #region LOAD DATA
                using (var reader = new StreamReader(@"C:\Users\vziex\Desktop\DLSU\BIOINFO\Bioinfo Report 2\new_dataset_final.txt"))
                {

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line.StartsWith(">"))
                        {
                            continue;
                        }
                        var values = line.Split(',').ToList();
                        values = values.Skip(1).ToList();
                        genomes.Add(values);
                    }
                }
                #endregion

                List<int> allColumns = new List<int>();
                List<int> badColumns = new List<int>();
                for (int i = 0; i < 40059; i++) // for every column
                {
                    Console.WriteLine("Processing column {0}", i);
                    var ratio = GetColumnRatios(genomes, i, 1, gapRatios);
                    if (ratio >= 0.40)
                    {
                        badColumns.Add(i);
                    }
                    allColumns.Add(i);
                }

                Console.WriteLine("Bad columns: {0}", badColumns.Count);

                foreach (var badColumn in badColumns)
                {
                    var start = badColumn - 15;
                    var end = badColumn + 15;
                    // remove the flanks
                    for (int i = start; i <= end; i++)
                    {
                        allColumns.Remove(i);
                    }
                }

                var goodColumns = allColumns;
                // identify regions
                // identify regions, we are hoping here that we get regions that are close to the regions detected in supporting table 1
                List<Tuple<int, int>> positions = new List<Tuple<int, int>>();
                int blockStart = 0;
                for (int i = 0; i < goodColumns.Count;)
                {
                    if (i + 1 >= goodColumns.Count)
                    {
                        break;
                    }

                    if (goodColumns[i + 1] == goodColumns[i] + 1)
                    {
                        i += 1;
                    }
                    else
                    {
                        positions.Add(new Tuple<int, int>(goodColumns[blockStart], goodColumns[i]));
                        i += 1;
                        if (i < goodColumns.Count)
                        {
                            blockStart = i;
                        }
                    }
                }

                //var fileCounter = 0;
                //foreach (var ranges in positions)
                //{
                //    // 944 rows
                //    var fileText = "";
                //    for (int i = 0; i < 944; i++)
                //    {
                //        var row = string.Join(",", genomes[i].GetRange(ranges.Item1, ranges.Item2 - ranges.Item1 + 1));
                //        fileText += row;
                //        fileText += Environment.NewLine;
                //    }

                //    System.IO.File.WriteAllText(string.Format("file{0}.txt", fileCounter), fileText);
                //    // 1 file per range
                //    fileCounter++;
                //}

                Console.WriteLine("Good columns after subtracting the flanks: {0}", goodColumns.Count);
                var findRegion1 = goodColumns.Where(x => x >= 7281 && x <= 7291).Count();
                var findRegion2 = goodColumns.Where(x => x >= 10446 && x <= 10451).Count();
                var findRegion3 = goodColumns.Where(x => x >= 14249 && x <= 14249).Count();
                var findRegion4 = goodColumns.Where(x => x >= 25041 && x <= 25108).Count();
                var findRegion5 = goodColumns.Where(x => x >= 29498 && x <= 29498).Count();
                var findRegion6 = goodColumns.Where(x => x >= 32029 && x <= 32040).Count();
                var findRegion7 = goodColumns.Where(x => x >= 32906 && x <= 32927).Count();
                var findRegion8 = goodColumns.Where(x => x >= 36459 && x <= 36462).Count();
                var findRegion9 = goodColumns.Where(x => x >= 38798 && x <= 38808).Count();
                var findRegion10 = goodColumns.Where(x => x >= 38926 && x <= 38941).Count();
                var findRegion11 = goodColumns.Where(x => x >= 39356 && x <= 39360).Count();
                Console.WriteLine("Ratio of sequences in blocks over the entire MSA: {0}%", ((double)(goodColumns.Count * 944)) / (944 * 40059) * 100);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Approach1()
        {
            List<Tuple<int, int>> positions = new List<Tuple<int, int>>();
            double totalSequencesInHighConfidenceBlocks = 0;
            try
            {
                Dictionary<int, double> gapRatios = new Dictionary<int, double>();
                List<int> goodColumns = new List<int>();
                List<List<string>> genomes = new List<List<string>>();
                List<List<string>> highConfidenceBlocks = new List<List<string>>();

                #region LOAD DATA
                using (var reader = new StreamReader(@"C:\Users\vziex\Desktop\DLSU\BIOINFO\Bioinfo Report 2\new_dataset_final.txt"))
                {

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line.StartsWith(">"))
                        {
                            continue;
                        }
                        var values = line.Split(',').ToList();
                        values = values.Skip(1).ToList();
                        genomes.Add(values);
                    }
                }
                #endregion

                // high-confidence alignment blocks
                for (int i = 0; i < 40059;) // for every column
                {
                    var blockStart = i;
                    var column_size = 1; // reset column size
                    i += column_size; // move counter to curent position plus increment size to begin processsing next block after the current block


                    var ratio = GetColumnRatios(genomes, blockStart, column_size, gapRatios);

                    // the the gap ratio is greater than or equal to 40%, do not add that block
                    if (ratio >= 0.4)
                    {
                        Console.WriteLine("Gap ratio is greater than or equal to 40%, discarding block starting at position {0} to position {1}, Increment Size: {2}", blockStart, blockStart + column_size, column_size);

                    }
                    else
                    {
                        // else if the gap ratio is strictly less than 40%
                        // gap ratio is less than 40% in that column,
                        while (ratio < 0.4)
                        {
                            Console.WriteLine("Continuing...");

                            column_size++; // keep incrementing column size to be added to the block as long as the gap count is less than 40%
                            Console.WriteLine("Processing block starting at position {0} to position {1}, Increment Size: {2}", blockStart, blockStart + column_size, column_size);


                            ratio = GetColumnRatios(genomes, blockStart, column_size, gapRatios);
                            Console.WriteLine("Gap ratio for column {0} is: {1}% gaps", blockStart + column_size, ratio * 100);



                            Console.WriteLine();
                        }
                        column_size--;
                        if (column_size >= 16) // longer than 15 nucleotides, 1 column of nucleotides = 1 position
                        {
                            // take note of the start and end positions of each high-confidence block
                            positions.Add(new Tuple<int, int>(blockStart, blockStart + column_size));
                            for (int j = blockStart; j <= blockStart + column_size; j++)
                            {
                                goodColumns.Add(j);
                            }
                            i = blockStart + column_size;
                            blockStart = i;
                            totalSequencesInHighConfidenceBlocks += column_size * 944; // number of sequences = number of columns * 944 rows
                        }

                    }
                }

                Console.WriteLine("Done?");
                Console.WriteLine("Total nucleotides in the blocks: {0}", totalSequencesInHighConfidenceBlocks);

                // should be 53% per paper: "...regions (spanning 53% of the total alignment)..."
                Console.WriteLine("Ratio of sequences in blocks over the entire MSA: {0}%", (totalSequencesInHighConfidenceBlocks / (944 * 40059)) * 100);
                var findRegion1 = goodColumns.Where(x => x >= 7281 && x <= 7291).Count();
                var findRegion2 = goodColumns.Where(x => x >= 10446 && x <= 10451).Count();
                var findRegion3 = goodColumns.Where(x => x >= 14249 && x <= 14249).Count();
                var findRegion4 = goodColumns.Where(x => x >= 25041 && x <= 25108).Count();
                var findRegion5 = goodColumns.Where(x => x >= 29498 && x <= 29498).Count();
                var findRegion6 = goodColumns.Where(x => x >= 32029 && x <= 32040).Count();
                var findRegion7 = goodColumns.Where(x => x >= 32906 && x <= 32927).Count();
                var findRegion8 = goodColumns.Where(x => x >= 36459 && x <= 36462).Count();
                var findRegion9 = goodColumns.Where(x => x >= 38798 && x <= 38808).Count();
                var findRegion10 = goodColumns.Where(x => x >= 38926 && x <= 38941).Count();
                var findRegion11 = goodColumns.Where(x => x >= 39356 && x <= 39360).Count();
                Console.WriteLine("{0} high-confidence blocks detected", positions.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} high-confidence blocks detected", positions.Count);

                Console.WriteLine(ex.Message);
            }
        }

        private static double GetColumnRatios(List<List<string>> genomes, int blockStart, int increment, Dictionary<int, double> gapRatios)
        {
            var genomicRegion = new List<string>();
            double retVal = 0;
            foreach (var genome in genomes) // 944 times
            {
                // assemble the region of a genome from genome[blockStart] until genome[blockStart + increment]
                var region = genome.GetRange(blockStart, increment);

                // join the columns into one substring
                var appendedRegion = string.Join("", region);

                // add the appended region for this genome in a temporary list
                genomicRegion.Add(appendedRegion);

               
            }

            // perform column-wise comparison to check if < 40% gaps IN EACH POSITION (meaning: in each column)

            // the increment value is the number of columns/positions there is in the 944 genomes
            // for each column, 
            for (int columnIndex = 0; columnIndex < increment; columnIndex++)
            {
                var columnStr = "";

                // faster computing, for instance if we are processing 16 columns, no need to re-process the first 15 columns just to process the 16th column
                if (gapRatios.ContainsKey(blockStart + columnIndex))
                {
                    retVal = gapRatios[blockStart + columnIndex];
                    continue;
                }

                // assemble the column at a specific column index by traversing all of the nucleotides in that column.
                foreach (var substring in genomicRegion)
                {
                    // assemble this specific column in this region as a string
                    columnStr += substring[columnIndex].ToString();
                }

                // get the number of sequences in the assembled column
                double totalSequencesInColumn = columnStr.Length;

                // count the number of gaps in the assembled column
                double gapCountInColumn = columnStr.Where(letter => letter.ToString().Equals("0")).Count();

                retVal = gapCountInColumn / totalSequencesInColumn;
                gapRatios.Add(blockStart + columnIndex, retVal);
            }

            return retVal;

        }

    }
}
