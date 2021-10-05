using System;

namespace Converter
{
    class Program
    {
        static void Main(string[] args)
        {

            System.Collections.Generic.List<string> arguments = new System.Collections.Generic.List<string>(args);
            if (arguments.Contains("-koef"))
            {

                //convert koef file
                Console.WriteLine("Trying to find koef file...");


                Console.OutputEncoding = System.Text.Encoding.UTF8;
                //read koef values
                if (System.IO.File.Exists(arguments[arguments.IndexOf("-koef") + 1]))
                {
                    Console.Write("found!");
                    Console.WriteLine();
                    Console.WriteLine("Reading Machine emition data: ");
                    Console.CursorLeft = ("Reading Machine emition data: ").Length;
                    Console.Write("0%");
                    System.Text.StringBuilder csv = new System.Text.StringBuilder();
                    var strings = System.IO.File.ReadAllText(arguments[arguments.IndexOf("-koef") + 1], System.Text.Encoding.UTF8);

                    System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<string>> data = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<string>>();
                    for (int i = 12; i < strings.Length; i += 51)
                    {
                        //read category
                        //category is stored as 2 symblol decimal integer number written 3 symbols before the value
                        //categories
                        /*
                         * 7 - Пыль абразивная
                         * 11 - пыль металлическая
                         * 10 - пыль войлока
                         * 9 - текстильная
                         * 2 - полировальной пасты
                         */
                        //different categories are important cause we will be reading them in that order


                        int category = 7;
                        string cat = "";
                        string res = "";
                        string id = "";
                        for (int j = 0; j < 2; j++)
                        {
                            id += strings[i + j - 12];
                            csv.Append($"{strings[i + j - 12]}");
                        }
                        csv.Append($";");
                        for (int j = 0; j < 2; j++)
                        {
                            cat += strings[i + j - 3];
                            csv.Append($"{strings[i + j - 3]}");
                            //Console.Write($"{strings[i + j - 3]}");
                        }
                        category = Convert.ToInt32(cat);
                        csv.Append($";");
                        //Console.Write($";");
                        //all number values are stored as float values with 9 symbol lenght
                        for (int j = 0; j < 9; j++)
                        {
                            res += strings[i + j];
                            csv.Append($"{strings[i + j]}");
                            // Console.Write($"{strings[i + j]}");
                        }
                        try
                        {
                            data[category].Add(res);
                        }
                        catch (System.Collections.Generic.KeyNotFoundException e)
                        {
                            data.Add(category, new System.Collections.Generic.List<string>());
                            data[category].Add(res);
                        }
                        csv.Append($"\n");
                        Console.CursorLeft = ("Reading Machine emition data: ").Length;
                        Console.Write($"{(int)(((float)(i + 51) / (float)strings.Length) * 100f)}%");
                        //Console.Write($"\n");

                    }

                    foreach (var dat in data)
                    {
                        System.IO.File.Delete($"./koef_{dat.Key}.csv");
                        System.IO.File.AppendAllText($"./koef_{dat.Key}.csv", $"\n Key id : {dat.Key} \n");
                        System.IO.File.AppendAllLines($"./koef_{dat.Key}.csv", dat.Value);
                        Console.WriteLine($"Made file for cateogory {dat.Key}");
                    }
                }
                else
                {
                    System.Console.Write("failed! No file found");
                }
            }
            Console.WriteLine();
            if (arguments.Contains("-tp"))
            {
                //convert koef file
                Console.WriteLine("Trying to find tp file...");
                //read machine names
                if (System.IO.File.Exists(arguments[arguments.IndexOf("-tp") + 1]))
                {

                    Console.Write("found!");
                    Console.WriteLine();
                    System.Console.Write("Reading machines : ");
                    System.Text.StringBuilder csv = new System.Text.StringBuilder();
                    byte[] strings = System.IO.File.ReadAllBytes(arguments[arguments.IndexOf("-tp") + 1]);
                    System.Collections.Generic.Dictionary<double, System.Collections.Generic.List<byte>> res = new System.Collections.Generic.Dictionary<double, System.Collections.Generic.List<byte>>(4);
                    try
                    {
                        for (int i = 0; i < strings.Length; i += 526)
                        {
                            int id = Convert.ToInt32(strings[i]);

                            //read till we hit more then one space in the row
                            int spacecount = 0;
                            int charCount = -16;//because 0 is the machine id
                            System.Collections.Generic.List<byte> tableId = new System.Collections.Generic.List<byte>();
                            try
                            {
                                while (spacecount < 2)
                                {
                                    tableId.Add(strings[i + charCount]);
                                    if (strings[i + charCount] == 32)
                                    {
                                        spacecount += 1;
                                    }
                                    else
                                    {
                                        spacecount = 0;
                                    }
                                    charCount++;
                                }
                            }
                            catch (System.IndexOutOfRangeException)
                            {
                                tableId.Add(49);
                            }
                            double group = Convert.ToDouble(System.Text.Encoding.Default.GetString(tableId.ToArray()), System.Globalization.CultureInfo.InvariantCulture);
                            bool need = true;

                            foreach (var key in res.Keys)
                            {
                                if (group == key)
                                {
                                    need = false;
                                }
                            }
                            if (need)
                            {
                                res.Add(group, new System.Collections.Generic.List<byte>());
                                Console.WriteLine($"Added group : {group}");
                            }

                            res[group].Add(strings[i]);

                            res[group].Add(59);
                            csv.Append($"{id};");
                            spacecount = 0;
                            charCount = 1;//because 0 is the machine id
                            while (spacecount < 2)
                            {
                                res[group].Add(strings[i + charCount]);
                                csv.Append(strings[i + charCount]);
                                if (strings[i + charCount] == 32)
                                {
                                    spacecount += 1;
                                }
                                else
                                {
                                    spacecount = 0;
                                }
                                charCount++;
                            }
                            res[group].Add(59);
                            csv.Append(";");
                            //read the special value
                            spacecount = 0;
                            charCount = 254;
                            while (spacecount < 2)
                            {
                                res[group].Add(strings[i + charCount]);
                                csv.Append(strings[i + charCount]);
                                if (strings[i + charCount] == 32)
                                {
                                    spacecount += 1;
                                }
                                else
                                {
                                    spacecount = 0;
                                }
                                charCount++;
                            }
                            res[group].Add(10);
                            csv.Append("\n");
                            System.Console.CursorLeft = "Reading machines : ".Length;
                            System.Console.Write($"{(int)((float)(i + 526) / (float)strings.Length) * 100f}%");

                        }
                    }
                    catch (System.IndexOutOfRangeException e)
                    {
                        //ignore that
                    }
                    foreach (var dat in res)
                    {
                        System.IO.File.WriteAllBytes($"./machines_{dat.Key}.csv", dat.Value.ToArray());
                    }

                }
                else
                {
                    System.Console.Write("failed! No file found.");
                }
            }
            Console.WriteLine();
            if (arguments.Contains("-zv"))
            {
                //convert koef file
                Console.WriteLine("Trying to find zv file...");
                //read machine names
                if (System.IO.File.Exists(arguments[arguments.IndexOf("-zv") + 1]))
                {

                    Console.Write("found!");
                    Console.WriteLine();
                    Console.Write("Reading elements : ");
                    System.Text.StringBuilder csv = new System.Text.StringBuilder();
                    byte[] strings = System.IO.File.ReadAllBytes(arguments[arguments.IndexOf("-zv") + 1]);

                    System.Collections.Generic.List<byte> resBytes = new System.Collections.Generic.List<byte>();
                    for (int i = 0; i < strings.Length; i += 288)
                    {
                        try
                        {
                            int count = 0;
                            //first few bytes(it appears as if the number does not actually have two bytes)
                            //id is stored in first 12 bytes
                            //number -> read the value(number is less then or equal to two digits)
                            byte[] idBytes = new byte[12];
                            byte[] codeBytes = new byte[4];
                            byte[] nameBytes = new byte[258];
                            byte[] valueBytes = new byte[12];
                            byte solid = 0x54;
                            for (int j = 0; j < 12; j++)
                            {
                                idBytes[j] = strings[i + j];
                            }

                            //code is stored as next 4 bytes
                            for (int j = 0; j < 4; j++)
                            {
                                codeBytes[j] = strings[i + j + 12];
                            }

                            //name is stored as next 258 bytes
                            //we also need to clean empty space from the name
                            for (int j = 0; j < 258; j++)
                            {
                                nameBytes[j] = strings[i + j + 16];
                            }

                            for (int j = 0; j < 12; j++)
                            {
                                valueBytes[j] = strings[i + j + 273];
                            }

                            //add to string builder
                            csv.Append(
                                $"{System.Text.Encoding.UTF8.GetString(idBytes)};" +
                                $"{System.Text.Encoding.UTF8.GetString(codeBytes)};" +
                                $"{System.Text.Encoding.UTF8.GetString(nameBytes)};" +
                                $"{System.Text.Encoding.UTF8.GetString(valueBytes)}" +
                                $"{solid}\n");
                            resBytes.AddRange(idBytes);
                            resBytes.Add(59);
                            resBytes.AddRange(codeBytes);
                            resBytes.Add(59);
                            resBytes.AddRange(nameBytes);
                            resBytes.Add(59);
                            resBytes.AddRange(valueBytes);
                            resBytes.Add(59);
                            resBytes.Add(strings[i+285]);
                            resBytes.Add(10);
                        }
                        catch (System.IndexOutOfRangeException e)
                        {

                        }
                    }
                    System.IO.File.WriteAllBytes($"./elements.csv", resBytes.ToArray());
                }
            }
        }
    }
}
