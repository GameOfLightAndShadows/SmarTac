# A Game of Light and Shadows

[![Join the chat at https://gitter.im/A-game-of-light-and-shadows/Lobby](https://badges.gitter.im/A-game-of-light-and-shadows/Lobby.svg)](https://gitter.im/A-game-of-light-and-shadows/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Master Branch [![Build status](https://ci.appveyor.com/api/projects/status/irnoo2kc6yxv8oia/branch/master?svg=true)](https://ci.appveyor.com/project/Kavignon/a-game-of-light-and-shadows/branch/master)

Augmented Tactic Branch : ![Build status] (https://travis-ci.org/Kavignon/A-game-of-light-and-shadows.svg?branch=FromAugmentedTactics)

## Synopsis

Previously, in the fall of 2015, as a credited project for my software engineering undergrad studies, I developed a tactical role playing game prototype using augmented reality and voice recognition as core parts of the game mechanics. There are still some issues to complete in the prototype, but the core elements were implemented. I took inspiration from games such as 
- Final Fantasy Tactics
- Fire Emblem Awakening 
- Golden Sun

The AugmentedTactics has three basic types of characters : 
- Warrior
- Magician
- Sniper 

The idea behind creating roles for characters was to create a tactical advantage/disavantage. With this, the players and the AI could use this as a basis for their strategy for example.

The game was developed with the idea of giving the player simple commads to perform in order to beat the AI.The commands were the following : 
- Move
- Attack
- Defend 
- Rotate 
- Skip turn 
- End Turn

The AI was developed by create different sequences of actions to do and evaluate which one would be best using a greedy algorithm that I had personally designed. Depending of the situation, certain things can have more weight such as going to your healer if your health is too low or attacking the character which has a role disavantage to yours such as a warrior will make more damage to a magician since its physical defense is lower.

## Motivation

The idea behind SmarTac was to develop a new TRPG based on the work I previously completed on Augmented Tactics. With time, it became apparent that this project should not be a game but a framework that other developers could use as a plugin in their game to develop their own games. To show how to use the framework, the project, ATRPG, under the GameOfLigntAndShadows community, will use it in a near future to give a more complete view of an AR-based TRPG prototype.

#Contributing

Take a look at the project and the code I've written in the past. There some issues I've listed that I haven't had the chance to work on during the 3 months period I was given to develop the project in the fall of 2016. You may even open new issues if you see things that you'd like to add as features to the framework. What I'd would love to see is either patches or unit tests on what's already there. Thanks !

## License 

For the project, I have establish a MIT license.
