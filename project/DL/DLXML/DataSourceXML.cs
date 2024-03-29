﻿using DL;
using DO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DLXML
{
    class FileDetails
    {
        public XElement Root { get; set; }
        public string Path { get; set; }
    }
    static class DataSourceXML
    {
        static string FileName = "DataSource.xml";
        const int tryAginIn = 10;
        const int tryNtimes = 10;

        #region files Names
        static string LinesFileName = "Lines.xml";
        static string BusesFileName = "Buses.xml";
        static string AdjacentStationsFileName = "AdjacentStations.xml";
        static string BusTripsFileName = "BusTrips.xml";
        static string StationsFileName = "Stations.xml";
        static string LineStationsFileName = "LineStations.xml";
        static string LineTripsFileName = "LineTrips.xml";
        static string UsersFileName = "Users.xml";
        static string UsersTripsFileName = "UsersTrips.xml";
        #endregion

        #region files paths
        static string LinesFilePath = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), "DL", "DS", "Database", LinesFileName);
        static string BusesFilePath = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), "DL", "DS", "Database", BusesFileName);
        static string AdjacentStationsFilePath = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), "DL", "DS", "Database", AdjacentStationsFileName);
        static string BusTripsFilePath = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), "DL", "DS", "Database", BusTripsFileName);
        static string StationsFilePath = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), "DL", "DS", "Database", StationsFileName);
        static string LineStationsFilePath = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), "DL", "DS", "Database", LineStationsFileName);
        static string LineTripsFilePath = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), "DL", "DS", "Database", LineTripsFileName);
        static string UsersFilePath = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), "DL", "DS", "Database", UsersFileName);
        static string UsersTripsFilePath = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), "DL", "DS", "Database", UsersTripsFileName);
        #endregion

        static Dictionary<string, FileDetails> files = new Dictionary<string, FileDetails>
        {
            ["Buses"] = new FileDetails { Path = BusesFilePath },
            ["AdjacentStations"] = new FileDetails { Path = AdjacentStationsFilePath },
            ["Lines"] = new FileDetails { Path = LinesFilePath },
            ["BusTrips"] = new FileDetails { Path = BusTripsFilePath },
            ["Stations"] = new FileDetails { Path = StationsFilePath },
            ["LineStations"] = new FileDetails { Path = LineStationsFilePath },
            ["LineTrips"] = new FileDetails { Path = LineTripsFilePath },
            ["Users"] = new FileDetails { Path = UsersFilePath },
            ["UserTrips"] = new FileDetails { Path = UsersTripsFilePath }
        };

        #region data Access
        public static XElement Buses
        {
            get
            {
                string index = "Buses";
                try
                {
                    for (int i = 0; i < tryNtimes; i++)
                    {
                        try
                        {
                            if (files[index].Root != null)
                                files[index].Root.Save(files[index].Path);
                            files[index].Root = XElement.Load(files[index].Path);
                            break;//if no exeption was throwed then stop try
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(tryAginIn);
                        }
                    }
                }
                catch (IOException)
                {
                    throw new FileLoadException(files[index].Path);
                }
                return files[index].Root;
            }
        }
        public static XElement AdjacentStations
        {
            get
            {
                string index = "AdjacentStations";
                try
                {
                    for (int i = 0; i < tryNtimes; i++)
                    {
                        try
                        {
                            if (files[index].Root != null)
                                files[index].Root.Save(files[index].Path);
                            files[index].Root = XElement.Load(files[index].Path);
                            break;//if no exeption was throwed then stop try
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(tryAginIn);
                        }
                    }
                }
                catch (IOException)
                {
                    throw new FileLoadException(files[index].Path);
                }
                return files[index].Root;
            }
        }
        public static XElement Lines
        {
            get
            {
                string index = "Lines";
                try
                {
                    for (int i = 0; i < tryNtimes; i++)
                    {
                        try
                        {
                            if (files[index].Root != null)
                                files[index].Root.Save(files[index].Path);
                            files[index].Root = XElement.Load(files[index].Path);
                            break;//if no exeption was throwed then stop try
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(tryAginIn);
                        }
                    }
                }
                catch (IOException)
                {
                    throw new FileLoadException(files[index].Path);
                }
                return files[index].Root;
            }
        }
        public static XElement BusTrips
        {
            get
            {
                string index = "BusTrips";
                try
                {
                    for (int i = 0; i < tryNtimes; i++)
                    {
                        try
                        {
                            if (files[index].Root != null)
                                files[index].Root.Save(files[index].Path);
                            files[index].Root = XElement.Load(files[index].Path);
                            break;//if no exeption was throwed then stop try
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(tryAginIn);
                        }
                    }
                }
                catch (IOException)
                {
                    throw new FileLoadException(files[index].Path);
                }
                return files[index].Root;
            }
        }
        public static XElement Stations
        {
            get
            {
                string index = "Stations";
                try
                {
                    for (int i = 0; i < tryNtimes; i++)
                    {
                        try
                        {
                            if (files[index].Root != null)
                                files[index].Root.Save(files[index].Path);
                            files[index].Root = XElement.Load(files[index].Path);
                            break;//if no exeption was throwed then stop try
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(tryAginIn);
                        }
                    }
                }
                catch (IOException)
                {
                    throw new FileLoadException(files[index].Path);
                }
                return files[index].Root; 
            }
        }
        public static XElement LineStations
        {
            get
            {
                string index = "LineStations";
                try
                {
                    for (int i = 0; i < tryNtimes; i++)
                    {
                        try
                        {
                            if (files[index].Root != null)
                                files[index].Root.Save(files[index].Path);
                            files[index].Root = XElement.Load(files[index].Path);
                            break;//if no exeption was throwed then stop try
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(tryAginIn);
                        }
                    }
                }
                catch (IOException)
                {
                    throw new FileLoadException(files[index].Path);
                }
                return files[index].Root;
            }
        }
        public static XElement LineTrips
        {
            get
            {
                string index = "LineTrips";
                try
                {
                    for (int i = 0; i < tryNtimes; i++)
                    {
                        try
                        {
                            if (files[index].Root != null)
                                files[index].Root.Save(files[index].Path);
                            files[index].Root = XElement.Load(files[index].Path);
                            break;//if no exeption was throwed then stop try
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(tryAginIn);
                        }
                    }
                }
                catch (IOException)
                {
                    throw new FileLoadException(files[index].Path);
                }
                return files[index].Root;
            }
        }
        public static XElement Users
        {
            get
            {
                string index = "Users";
                try
                {
                    for (int i = 0; i < tryNtimes; i++)
                    {
                        try
                        {
                            if (files[index].Root != null)
                                files[index].Root.Save(files[index].Path);
                            files[index].Root = XElement.Load(files[index].Path);
                            break;//if no exeption was throwed then stop try
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(tryAginIn);
                        }
                    }
                }
                catch (IOException)
                {
                    throw new FileLoadException(files[index].Path);
                }
                return files[index].Root;
            }
        }
        public static XElement UserTrips
        {
            get
            {
                string index = "UserTrips";
                try
                {
                    for (int i = 0; i < tryNtimes; i++)
                    {
                        try
                        {
                            if (files[index].Root != null)
                                files[index].Root.Save(files[index].Path);
                            files[index].Root = XElement.Load(files[index].Path);
                            break;//if no exeption was throwed then stop try
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(tryAginIn);
                        }
                    }
                }
                catch (IOException)
                {
                    throw new FileLoadException(files[index].Path);
                }
                return files[index].Root;
            }
        } 
        #endregion

        

        public static int serialLineID;
        static string dataBasePath = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), "DL", "DS", "Database");
        static readonly string FilePath = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), "DL", "DS", "Database", FileName);

        static DataSourceXML()
        {
            serialLineID = 0;
            foreach (var file in files)
            {
                if (!File.Exists(file.Value.Path))//if the file don't exist
                {
                    file.Value.Root = new XElement(file.Key);//create new file with root <elements>
                    file.Value.Root.Save(file.Value.Path);
                }
            }
        }

        static public void Save(string fileName)
        {
            if(!files.ContainsKey(fileName))
            {
                throw new ArgumentException($"file {fileName} dont exist");
            }
            try
            {
                for (int i = 0; i < tryNtimes; i++)
                {
                    try
                    {
                        files[fileName].Root.Save(files[fileName].Path);
                        break;//if no exeption was throwed then stop try
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(tryAginIn);
                    }
                }
            }
            catch (IOException)
            {
                throw new FileLoadException(files[fileName].Path);
            }
        }

        public static void SaveListSerializer<T>(this List<T> list, string typename)
        {
            XMLTools.SaveListToXMLSerializer<T>(list, files[typename].Path);//save the list to the file that accur in files as the file of this intety
        }
        public static void SaveList(this XElement root,string typename)
        {
            switch (typename)
            {
                case "Lines":
                    root.Save(LinesFilePath);
                    break;
                default:
                    break;
            }
            //dsRoot.Save(path);
            throw new NotImplementedException("DataSourceXML.Save");
        }
    }
}
