using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Collections.Generic;

namespace Lab1
{
    class Program
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;

        static void Main(string[] args)
        {
            client = new TcpClient();
            try
            {
                client.Connect(host, port); //подключение клиента
                stream = client.GetStream(); // получаем поток

                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //старт потока
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        // отправка сообщений
        static void SendMessage()
        {
            bool flag = true;
            while (flag)
            {
                Console.WriteLine("1. Watch info about all technical.");
                Console.WriteLine("2. Find by cost.");
                Console.WriteLine("3. Sort by weight");
                Console.WriteLine("4. Delete by name");
                Console.WriteLine("5. Edit info.");
                Console.WriteLine("6. Create new.");
                Console.WriteLine("7. Exit");
                string message = Console.ReadLine();
                Command command = new Command();
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Command));
                string commandToSend = string.Empty;
                switch (message)
                {
                    case "1":
                        {
                            command.command = Commands.Get;
                            using (FileStream file = new FileStream("command.json", FileMode.Create))
                            {
                                jsonSerializer.WriteObject(file, command);
                            }
                            using (StreamReader file = new StreamReader("command.json"))
                            {
                                commandToSend += file.ReadToEnd();
                            }
                            byte[] data = Encoding.Unicode.GetBytes(commandToSend.ToString());
                            stream.Write(data, 0, data.Length); //передача данных
                            break;
                        }
                    case "2":
                        {
                            command.command = Commands.Get;
                            Console.Write("Enter cost: ");
                            double cost = Convert.ToInt32(Console.ReadLine());
                            command.infoFirst = cost.ToString();
                            using (FileStream file = new FileStream("command.json", FileMode.Create))
                            {
                                jsonSerializer.WriteObject(file, command);
                            }
                            using (StreamReader file = new StreamReader("command.json"))
                            {
                                commandToSend += file.ReadToEnd();
                            }
                            byte[] data = Encoding.Unicode.GetBytes(commandToSend.ToString());
                            stream.Write(data, 0, data.Length); //передача данных
                            break;
                        }
                    case "3":
                        {
                            command.command = Commands.Get;
                            command.infoSecond = "Sort";
                            using (FileStream file = new FileStream("command.json", FileMode.Create))
                            {
                                jsonSerializer.WriteObject(file, command);
                            }
                            using (StreamReader file = new StreamReader("command.json"))
                            {
                                commandToSend += file.ReadToEnd();
                            }
                            byte[] data = Encoding.Unicode.GetBytes(commandToSend.ToString());
                            stream.Write(data, 0, data.Length); //передача данных
                            break;
                        }
                    case "4":
                        {
                            command.command = Commands.Delete;
                            Console.Write("Enter name: ");
                            string name = Console.ReadLine();
                            command.infoFirst = name;
                            using (FileStream file = new FileStream("command.json", FileMode.Create))
                            {
                                jsonSerializer.WriteObject(file, command);
                            }
                            using (StreamReader file = new StreamReader("command.json"))
                            {
                                commandToSend += file.ReadToEnd();
                            }
                            byte[] data = Encoding.Unicode.GetBytes(commandToSend.ToString());
                            stream.Write(data, 0, data.Length); //передача данных
                            break;
                        }
                    case "5":
                        {
                            command.command = Commands.Edit;
                            Console.Write("Enter name of technical to edit: ");
                            string name = Console.ReadLine();
                            command.infoFirst = name;
                            Console.Write("Enter new name: ");
                            name = Console.ReadLine();
                            Console.Write("Enter new state of producer: ");
                            string state = Console.ReadLine();
                            Console.Write("Enter new cost: ");
                            double cost = Convert.ToDouble(Console.ReadLine());
                            Console.Write("Enter new weight: ");
                            double weight = Convert.ToDouble(Console.ReadLine());
                            Console.Write("Enter new objem: ");
                            double objem = Convert.ToDouble(Console.ReadLine());
                            command.infoSecond = (new Technical(name, state, cost, weight, objem)).ToString();
                            using (FileStream file = new FileStream("command.json", FileMode.Create))
                            {
                                jsonSerializer.WriteObject(file, command);
                            }
                            using (StreamReader file = new StreamReader("command.json"))
                            {
                                commandToSend += file.ReadToEnd();
                            }
                            byte[] data = Encoding.Unicode.GetBytes(commandToSend.ToString());
                            stream.Write(data, 0, data.Length); //передача данных
                            break;
                        }
                    case "6":
                        {
                            command.command = Commands.Create;
                            Console.Write("Enter name: ");
                            string name = Console.ReadLine();
                            Console.Write("Enter state of producer: ");
                            string state = Console.ReadLine();
                            Console.Write("Enter cost: ");
                            double cost = Convert.ToDouble(Console.ReadLine());
                            Console.Write("Enter weight: ");
                            double weight = Convert.ToDouble(Console.ReadLine());
                            Console.Write("Enter objem: ");
                            double objem = Convert.ToDouble(Console.ReadLine());
                            command.infoFirst = (new Technical(name, state, cost, weight, objem)).ToString();
                            using (FileStream file = new FileStream("command.json", FileMode.Create))
                            {
                                jsonSerializer.WriteObject(file, command);
                            }
                            using (StreamReader file = new StreamReader("command.json"))
                            {
                                commandToSend += file.ReadToEnd();
                            }
                            byte[] data = Encoding.Unicode.GetBytes(commandToSend.ToString());
                            stream.Write(data, 0, data.Length); //передача данных
                            break;
                        }
                    case "7":
                        flag = false;
                        break;
                    default:
                        break;
                }
            }
        }
        // получение сообщений
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    string message = string.Empty;
                    do
                    {
                        byte[] data = new byte[1048576];
                        stream.Read(data, 0, data.Length);
                        message += Encoding.Unicode.GetString(data);
                    } while (stream.DataAvailable);
                    Result result = new Result();
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Result));
                    using (StreamWriter file = new StreamWriter("result.json", false))
                    {
                        file.Write(message);
                    }
                    using (FileStream file = new FileStream("result.json", FileMode.Create))
                    {
                        result = (Result)serializer.ReadObject(file);
                    }
                    if (result.exitCode != 0)
                    {
                        Console.WriteLine("Error while complitting.");
                    }
                    else
                    {
                        if (result.outputInfo is string)
                        {
                            Console.WriteLine(result.outputInfo);
                        }
                        else
                        {
                            foreach (Technical technical in (result.outputInfo as List<Technical>))
                            {
                                Console.WriteLine(technical.ToString());
                            }
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }
        static void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }
    }
}
