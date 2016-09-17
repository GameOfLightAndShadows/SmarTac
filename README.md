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

The idea behind A Game of Light and Shadows is to build a new version of the AI for the TRPG. This time around, I won't use Unity, but Urho3D to create a prototype for Android. Again, this time around, the project won't be in 3D but will be in 2D. The goal is to do a proof of concept to show that I can develop a hard mode AI to make it impossible for players to win against it. In order to do so, I am planning to use diverse machine learning techniques and genetic algorithms in order to use Darwin's idea in order to select the strongest and most adapted AI for the hard mode.

As of today (27/08/16), it is planned to have an AI able to classify states to understand whether the situation is 
- Good
- Bad 
- Neutral 
- Unknown

Over time, the bot will not only be able to classify game states but also, the both will learn to recognize the different possibilities that are available to it. By knowing what's available to him and using a reward system to be able to understand the game states, the bot will know when a decision was either good or bad. For instance, more XP can be awarded to the bot when a character is defeated but the total points will be reduced if it takes more time than usual to beat an opponent. Not only this, hopefully, I'll be able to implement a system able to react to more defensive/offensive party. Using a reward information

Lately, I've been really into functional programming and F#. For the project, I intend to do it in F#. Since it's a port of AugmentedTactics, the first thing will be to take the implementation and port in a functional style with F# for Urho3D.

## Task list
- [ ] Get to know Urho3D
- [x] Get to know genetic algorithms
- [ ] Get to know neural networks
- [x] Get to know Asynchronous Programming with F#
- [ ] 
- [ ] Learn about reinforcement learning 
- [x] Learn about supervised and unsupervised learning
- [ ] Get to know SARSA algorithm
- [ ] Create Urho3D 2D project for F#
- [x] Port character folder from AT
- [x] Port combat menu from AT
- [x] Port commands from AT
- [x] Port character artificial intelligence from AT
- [ ] Port Pathfinding algorithm from AT
- [ ] Create game states : MenuState, TurnState, AttackState, etc
- [ ] Create server to handle game state
- [ ] Update game world state via server
- [ ] Implement a tactical AI :  looks at the game state and determines a target
- [ ] Implement an operational AI :  receives a target from the Tactical AI, looks at its game state and chooses an optimal action to achieve its goal
- [ ] Create artificial brain 
- [ ] Train artificial brain
- [ ] Implement a genetic algorithm 
- [ ] Add continuous integration with Travis
- [ ] Implement most damageable command sequence 
- [ ] Implement an XP system 
- [ ] Implement money system 
- [ ] Implement a store for both items and equipment
- [ ] Implement time constraint for brain training 
- [ ] Test architecture with NUnit
- [ ] Initialize a database to store result from training over time
- [ ] Create a 64 x 64 map
- [ ] Make character assets move in the map 

## License 

For the project, I have establish a MIT license.
