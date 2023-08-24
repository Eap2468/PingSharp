using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Project{
    public class Project
    {
        public static bool IpStringCheck(string str)
        {
            try
            {
                if(!str.Contains('.'))
                {
                    return false;
                }

                string [] numbers = str.Split('.');
                
                if(numbers.Length != 4)
                {
                    return false;
                }

                foreach(string number in numbers)
                {
                    if(number == "x"){continue;}

                    short current = Convert.ToInt16(number);
                    if(current > 255 || current < 0)
                    {
                        return false;
                    }
                }
            }catch(Exception e)
            {
                return false;
            }
            return true;
        }

        public static void PingIP(string ip)
        {
            Ping ping = new();
            PingOptions options = new();
            string data = "Hello World!";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 1000;

            PingReply reply = ping.Send(ip, timeout, buffer, options);

            if(reply.Status == IPStatus.Success)
            {
                Console.WriteLine(ip + " is up, ms: " + reply.RoundtripTime + ", ttl: " + reply.Options.Ttl.ToString());
            }
        }

        public static void AutoScan()
        {
            NetworkInterface [] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            List<string> ipv4Ips = new();

            foreach(NetworkInterface currentInterface in interfaces)
            {
                if(currentInterface.Name == "lo0"){continue;}

                if(!currentInterface.Supports(NetworkInterfaceComponent.IPv4)){continue;}
                foreach (UnicastIPAddressInformation addrInfo in currentInterface.GetIPProperties().UnicastAddresses)
                {
                    if(addrInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        string interfaceIp = addrInfo.Address.ToString();
                        
                        interfaceIp = interfaceIp.Substring(0, interfaceIp.LastIndexOf('.') + 1);
                        interfaceIp += "x";

                        ipv4Ips.Add(interfaceIp);
                    }
                }
            }

            foreach(string ip in ipv4Ips)
            {
                for(int i = 0; i <= 255; i++)
                {
                    Thread thread = new Thread(() => PingIP(ip.Replace("x", i.ToString())));
                    thread.Start();
                }
            }
        }

        //modes are true=aggressive, false=passive
        public static void Portscan(string ip, bool mode)
        {
            for(int i = 0; i <= 65535; i++)
            {
                if(mode)
                {
                    Thread thread = new Thread((object port) => {
                        try
                        {
                            try
                            {
                                TcpClient client = new(ip, (int)port);
                                Console.WriteLine("Port " + port + " is open");
                            } catch(Exception){}

                        } catch (Exception){} 
                    });
                    thread.Start(i);
                }
                else
                {
                    try
                    {
                        TcpClient client = new(ip, i);
                        Console.WriteLine("Port " + i + " is open");
                    } catch(Exception){}
                    Thread.Sleep(50);

                }
            }
        }

        public static void Scan(string ip)
        {
            if(!IpStringCheck(ip))
            {
                Console.WriteLine("Please enter a valid ip address");
                return;
            }

            if(!ip.Contains('x'))
            {
                PingIP(ip);
                return;
            }

            for(int i = 0; i <= 255; i++)
            {
                Thread thread = new Thread(() => PingIP(ip.Replace("x", i.ToString())));
                thread.Start();
            }
        }

        public static string Help()
        {
             return @"
            Usage: dotnet run <Ip Range or Command>
            
            Ip example:
                Single Ip: 127.0.0.1
                Multiple Ips: 127.0.0.x (x is where to bruteforce)

            Commands:
                Help: Shows this page
                Auto: Auto detects connected subnets and checks all ip addresses connected
                Portscan: scans all ports on a given ip, usage: Portscan <IP> opt<scan mode (options are aggressive [a] or passive [p])>
            ";
        }
        public static void Main(string [] args)
        {   
            if(args.Length < 1)
            {
                Console.WriteLine(Help());
                return;
            }

            if(args[0].ToLower() == "help")
            {
                Console.WriteLine(Help());
                return;
            }
            else if(args[0].ToLower() == "auto")
            {
                AutoScan();
                return;
            }
            else if(args[0].ToLower() == "portscan")
            {
                if(args[1] == null)
                {
                    Console.WriteLine("Usage: dotnet run Portscan <IP>");
                    return;
                }
                if(!IpStringCheck(args[1]))
                {
                    Console.WriteLine("Please enter a valid ip");
                    return;
                }

                bool mode = true;
                if(args.Length > 2)
                {
                    if(args[2] == "p" || args[2] == "passive")
                    {
                        mode = false;
                    }
                    else if(args[2] == "a" || args[2] == "aggressive")
                    {
                        mode = true;
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid mode (aggressive [a] or passive [p])");
                        return;
                    }
                }

                Portscan(args[1], mode);
            }
            else
            {
                string ipStr = args[0];
                if(IpStringCheck(ipStr))
                {
                    Scan(ipStr);
                }
                else
                {
                    Console.WriteLine("Please enter a valid ip");
                }
                return;
            }
        }
    }
}