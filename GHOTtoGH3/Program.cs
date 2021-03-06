﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHOTtoGH3 {
    class Program {
        static void Main(string[] args) {
            string inputFileName;
            string outputFileName;

            for (int song = 0; song < args.Length; song++) {
                inputFileName = args[song];
                // Get directory (eg. "Z:\TestData\AllStar_gems_bass_hard.qgm" = "Z:\TestData\AllStar\")
                string directory = inputFileName.Substring(0, inputFileName.IndexOf('_')) + "\\";
                // Get file type (eg. "Z:\TestData\AllStar_gems_bass_hard.qgm" = "gems")
                string fileType = inputFileName.Substring(inputFileName.IndexOf('_') + 1, inputFileName.IndexOf('.') - inputFileName.IndexOf('_') - 1);
                if (fileType.Substring(0, 4) == "gems")
                    fileType = "gems";

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                FileStream infile = new FileStream(inputFileName, FileMode.Open);

                int hexIn;
                String hex = "";

                // Start reading bytes from file
                switch (fileType) {

                    // ~~~Beatlines~~~ //
                    case "frets":
                        // Create directory and file
                        directory = directory + "beatlines\\";
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);
                        outputFileName = directory + "beatlines.array.txt";
                        StreamWriter fretsFile = new StreamWriter(outputFileName);
                        int dec = 0, a = 1;
                        Boolean firstLine = true;

                        // Start reading byte characters from file
                        while ((hexIn = infile.ReadByte()) != -1)
                        {
                            hex = string.Format("{0:X2}", hexIn) + hex;
                            // Check if current hex length is 4 bytes
                            if (a % 4 == 0 && a != 0) {
                                dec = Convert.ToInt32(hex, 16);

                                // If first beatline is not '0', print 0 first
                                if (firstLine == true && dec != 0) {
                                    fretsFile.WriteLine("0");
                                }
                                firstLine = false;

                                // Print beatline
                                Console.WriteLine(dec);
                                fretsFile.WriteLine(dec);
                                hex = "";
                                dec = 0;
                            }
                            a++;
                        }
                        fretsFile.Close();
                        break;
                    // ~~~~~~~~~~~~~~~ //


                    // ~~~Notes~~~ //
                    case "gems":
                        string fullFileType = inputFileName.Substring(inputFileName.IndexOf('_') + 1, inputFileName.IndexOf('.') - inputFileName.IndexOf('_') - 1);

                        // Rename note types to match EOF import
                        switch (fullFileType) {
                            case "gems_easy":
                                fullFileType = "lead_Easy";
                                break;
                            case "gems_med":
                                fullFileType = "lead_Medium";
                                break;
                            case "gems_hard":
                                fullFileType = "lead_Hard";
                                break;
                            case "gems_expert":
                                fullFileType = "lead_Expert";
                                break;
                                
                            case "gems_bass_easy":
                                fullFileType = "bass_Easy";
                                break;
                            case "gems_bass_med":
                                fullFileType = "bass_Medium";
                                break;
                            case "gems_bass_hard":
                                fullFileType = "bass_Hard";
                                break;
                            case "gems_bass_expert":
                                fullFileType = "bass_Expert";
                                break;

                            default:
                                break;
                        }

                        // Create directory and file
                        directory = directory + fullFileType;
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);
                        outputFileName = directory + "\\" + fullFileType + ".array.txt";
                        StreamWriter gemsFile = new StreamWriter(outputFileName);

                        // Initialise note file contents and variables
                        gemsFile.WriteLine("HOPO");
                        int posDec = 0, typeDec = 0, infoDec = 0, lenDec = 0;
                        int startPos = 0, startType = 0, spLen = 0, prevSPms = 0, b = 1;
                        Boolean prevSP = false, hopoFlag = false;

                        while ((hexIn = infile.ReadByte()) != -1)
                        {
                            hex = string.Format("{0:X2}", hexIn) + hex;
                            // Check if current hex length is 8 bytes (0|0|00|0000)
                            if (b % 8 == 0 && b != 0) {

                                // Get hex for each part of chunk and convert to decimal
                                typeDec = Convert.ToInt32(hex.Substring(0, 2), 16);
                                infoDec = Convert.ToInt32(hex.Substring(2, 2), 16);
                                lenDec = Convert.ToInt32(hex.Substring(4, 4), 16);
                                posDec = Convert.ToInt32(hex.Substring(8, 8), 16);

                                // Find HOPO info
                                int hopoInfoDec = infoDec;
                                if (infoDec / 128 >= 1) {
                                    hopoInfoDec = infoDec - 128;
                                }
                                if (hopoInfoDec / 64 >= 1) {
                                    hopoFlag = true;
                                }

                                // Find SP info
                                if (infoDec % 2 == 1 && infoDec != 0) {
                                    prevSPms = posDec;
                                    if (prevSP == false) {
                                        prevSP = true;
                                        startPos = posDec;
                                        startType = typeDec;
                                    }
                                }
                                else {
                                    // If end of SP phase
                                    if (prevSP == true) {
                                        spLen = prevSPms - startPos + 1;

                                        string spDirectory = directory + "_SP\\";
                                        if (!Directory.Exists(spDirectory))
                                            Directory.CreateDirectory(spDirectory);
                                        StreamWriter spFile = new StreamWriter(spDirectory + startPos + ".array.txt");
                                        spFile.WriteLine(startPos);
                                        spFile.WriteLine(spLen);
                                        spFile.WriteLine("5");
                                        spFile.Close();
                                    }
                                    prevSP = false;
                                }

                                // Print note information
                                gemsFile.WriteLine(posDec);
                                if (lenDec == 0)
                                    lenDec = 1;
                                gemsFile.WriteLine(lenDec);
                                if (hopoFlag == true) {
                                    gemsFile.WriteLine("H" + typeDec);
                                    hopoFlag = false;
                                }
                                else
                                {
                                    gemsFile.WriteLine(typeDec);
                                }
                                
                                hex = "";
                                dec = 0;
                            }
                            b++;
                        }
                        gemsFile.Close();
                        break;
                    // ~~~~~~~~~~~ //


                    // ~~~Sections (English)~~~ //
                    case "sections_English":
                        // Create directory and file
                        directory = directory + "sections\\";
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);
                        outputFileName = directory + "sections.array.txt";
                        StreamWriter sectionsFile = new StreamWriter(outputFileName);

                        string stringHex = "";
                        int c = 1;


                        while ((hexIn = infile.ReadByte()) != -1)
                        {
                            hex = string.Format("{0:X2}", hexIn) + hex;
                            stringHex = stringHex + string.Format("{0:X2}", hexIn);

                            // Check if current hex length is 20 bytes
                            if (c % 20 == 0 && c != 0)
                            {
                                PrintSection(hex, stringHex, sectionsFile);
                                hex = "";
                                stringHex = "";
                            }
                            c++;
                        }
                        sectionsFile.Close();
                        break;
                    // ~~~~~~~~~~~~~~~ //


                    // ~~~Timesig~~~ //
                    case "timesig":
                        directory = directory + "timesig\\";
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);

                        outputFileName = directory + "0.array.txt";
                        StreamWriter sigFile = new StreamWriter(outputFileName);
                        sigFile.Write("0\n4\n4");
                        sigFile.Close();
                        break;
                    // ~~~~~~~~~~~~~ //


                    default:
                        break;
                }
            }
        }

        public static string ConvertHex(String hexString)
        {
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    String hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    uint decval = System.Convert.ToUInt32(hs, 16);
                    char character = System.Convert.ToChar(decval);
                    ascii += character;

                }
                return ascii;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return string.Empty;
        }

        // Add section to a file
        public static void PrintSection(string hex, string stringHex, StreamWriter sectionsFile) {
            string posHex = hex.Substring(32, 8);
            string sectionName = "";

            for (int index = 8; stringHex[index] != '0' || stringHex[index + 1] != '0' || sectionName.Length % 2 != 0; index++)
            {
                sectionName = sectionName + stringHex.Substring(index, 1);
            }
            sectionName = ConvertHex(sectionName);
            int posDec = Convert.ToInt32(posHex, 16);

            Console.WriteLine(posDec);
            sectionsFile.WriteLine(posDec);
            sectionsFile.WriteLine(sectionName);
        }
    }
}
