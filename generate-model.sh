#!/bin/bash

source ./AuxScripts/DotNetEntityScaffold ## Import DotNetScaffold function

SED_OVERRIDE="false"

## Related to Database Connection
CONNECTION_STRING_Name="GameList"
### I know, I Know, I shouldn't expose that kind of info, But It's safe in this case
### Make some env variables and remove this line, If you wish
CONNECTION_STRING="Data Source="$HOME"/GameLists.db;" 
DataBaseConnector="Microsoft.EntityFrameworkCore.sqlite"

TablesToGenerate=("Backlog" "GamesId")

## Related to Classes Stuff
ContextName="GameListsContext"
ProjectName="gamelistdb-model"
NameSpace="GameListDB.Model"
ModelFolder="Model"

## tmp directory
ModelMakerName=".ModelMaker"

DotNetScaffold