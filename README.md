# PingSharp
Simplifies the process of scanning for active IPs on the local network as well as basic port scanning

# Usage
dotnet run on linux

.\PingSharp.exe on windows

PingSharp.exe [IP or Command] [Extra]/(optional) [Extra]/(optional)

# Commands
**Help**: Displays the help menu, Usage PingSharp.exe help

**Auto**: Automatically finds and scans subnets from Network Interfaces on the computer, Usage PingSharp.exe auto

**Portscan**: Finds open ports on a given ip, can be ran in aggressive or passive mode (aggressive is default) to quicken or limit the amount of network traffic going out at a time, Usage PingSharp.exe portscan [IP] [aggressive [a]/passive [p]]/(optional)

**Regular Scan**: Scans the given subnet for active IP addresses, put an x on the octal to scan (example: 192.168.1.x to scan IPs in the 192.168.1.0/24 range), Usage PingSharp.exe [IP]

Project is created for .NET 7.0, some modification might be required to run earlier versions, I can convert the code to work with lower version if needed

This project is **NOT FINISHED**, theres still a ton of functionality I plan on adding in the future
