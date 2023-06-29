# BacklogDatabase-Utils

## Description

It was made for the necessity of automatized some rutinary task for a Database that I already owned (since 2018, I think!) and I used to modified directly.  
`Why you don't use stuff like backloggd or backloggery`  
I have accounts on these sites but I need some necesity that both pages doesn't meet.

- 1st I need some kind of time duration sorting. (Based for example in HLTB)
  - I have a limited time for play things and I need planning which games I want to play during the year.  
  So I have custom SQL query like:  
  "Show me highest rated games of this last X Years which their main+extra duration is less than 15 hours" or  
  "Which Games that I have pending that I can play handheld via retrocompatible handhelds or other devices"

- 2nd I need a way prioritize my backlog between my favourite franchise/genres and stuff I should play because "general culture or knowledge" or just try new things.  

Now, This have some side-effects, Thanks to the integration of IGDB, I can have a list of games that I own that use X Game Engines and try to make some known dll injection or some kind of tinkering things like mods.

Or also trying to make some integrations for other backlogers like the previous one named for the friends/knowns one that use them (for social stuff/stats etc), making some kind of Meta-Backloggery. (wwwwww)

At this moment, The database is single-user (As I didn't expect some kind of public service), But depending of the issues count (Which I doubt there would someone wants, lol), I could attempt to design/make a multi-user topology.

## How could I replicate your database?

Some SQL Create Scripts are avaliable in SqlScripts folder:  
The only tables required are Backlog and GamesId but It's also avaliable EGameSeries, EGameSystems and some views like 'GamesBeatenByYears' and 'V_VRGames'.  
They are thought for SQLite, Maybe It's required some modifications for Transact-SQL (AKA Microsoft SQL Server), MySQL/MariaDB.  
As for example: beaten and completed shoud be boolean but like early versions of C (like ANSI C), It doesn't exist in SQLite

## How Can I make the Database models?

Edit as you like and use the `generate-model.sh` script (It's required DotNet Core or DotNet 6+).  
If you use Windows use WSL or make/create an issue or just adapt the script to PowerShell or Batch, and make a PR. (I would recommend use Powershell as the likeness to UnixShell with C#/.NetFramework).  

If you need some example of Unix Shell to Powershell conversion, You can look up to one of my old projects [UE4-Toolkit](https://github.com/gabuscuv/UE4-Toolkit-Public).  

## Requirements

- A Machine/OS that It can run .Net Core or .Net 6+
- A way of make a SQLite Database and execute SQL Script (DB Browser for SQLite is my GUI multiplataform choice, If You ask me)
- A way of run Unix shell as an Unix Clonic or Certificated System (GNU/Linux, BSD, MacOSX) or some kind of wrapper/port or virtual machine as WSL1/2, Cygwin, git CLI or whatever.

## How build it?

- Clone this repo/run this command: `git clone --recursive --depth 1 https://github.com/gabuscuv/BacklogDatabase-Utils.git`
- Make the database with the sql scripts in SqlScripts folder
- edit `generate-model.sh` and run it.
- `dotnet build`
- done `dotnet run`
