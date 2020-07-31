# Botty Town

Botty Town is an experiment using the Alloy API and Unity to deliver a 3D gameworld backed by real data in alloy with synchronisation of data in the game world.

Complete inspections and jobs to gain points and have them closed in Alloy. Register defects that appear in the game world and have them reported in Alloy.

[![Botty Town Video](https://img.youtube.com/vi/OLg9N-ogxC4/0.jpg)](https://www.youtube.com/watch?v=OLg9N-ogxC4)

## Getting Started

Download the latest release and unpack the archive, run the game EXE and you will most likely be prompted to go check your config. This can be located on windows at `c:\Users\<name>\AppData\LocalLow\Yotta\Botty Town\bottytown.config`. You will need to replace the `ApiToken` with a valid token (labs has a Botty Town customer or make your own, see: customer requirements below). Run the game again and away you go!

Want more configuration options? Look at the config or read on below.

## Customer Requirements

If you want to setup Botty Town against a new customer, make sure they have Jobs, Defects, Inspections and some Assets installed. Out of the box the config will work against regular Jobs/Inspections/Defects as configured in blueprints, if you have special requirements e.g. custom attributes that are required you can go modify the following keys:

```json
{
  ...
  "DefectCreateDesignCode": "designs_exampleDefects",
  "DefectCreateCollection": "Live",
  "DefectCreateAttributes": {
    "attributes_defectsDefectNumber": 8008135,
    "attributes_exampleDefectsDangerous": true
  },
  "JobFixAttributes": {
    "attributes_tasksStatus": [
      "5bc5bdd281d088d177342c76"
    ]
  },
  "InspectionCompleteAttributes": {
    "attributes_tasksStatus": [
      "5bc5bdd281d088d177342c76"
    ]
  },
}
```

## Scoreboard Support

If you want scoreboard support then make a design with 2 attributes, name (string) and score (number) then setup score related keys in the config e.g.

```json
{
  ...
  "ScoreSaveAllowed": true,
  "ScoreSaveDesignCode": "designs_scoreboard_5f22ffd911e9e4006954bc20",
  "ScoreSaveCollectionCode": "Live",
  "ScoreSaveNameAttributeCode": "attributes_scoreboardPlayerName_5f22ffed11e9e4006954bc60",
  "ScoreSaveScoreAttributeCode": "attributes_scoreboardScore_5f23000211e9e4006954bce0",
}
```

## Mapping Prefabs (in game objects) to Design Codes
