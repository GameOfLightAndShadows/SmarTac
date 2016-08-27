# SmartTactics
Smart bot for 2D version AugmentedTactics

## Synopsis

Previously, in the fall of 2015, as a credited project for my software engineering undergrad studies, I developed a tactical role playing game prototype using augmented reality and voice recognition as core parts of the game mechanics. There are still some issues to complete in the prototype, but the core elements were implemented. I took game such as 
- Final Fantasy Tactics
- Fire Emblem Awakening 
- Fire Emblem

The AugmentedTactics has three types of characters : 
- Warrior
- Magician
- Sniper 

The game was developed with the idea of giving the player simple commads to perform in order to beat the AI.The commands were the following : 
- Move
- Attack
- Defend 
- Rotate 
- Skip turn 
- End Turn

The AI was developed by create different sequences of actions to do and evaluate which one would be best using a greedy algorithm that I had personally designed. Depending of the situation, certain things can have more weight such as going to your healer if your health is too low or attacking the character which has a role disavantage to yours such as a warrior will make more damage to a magician since its physical defense is lower.

## Motivation

The idea behind SmartTactics is to build a new version of the AI for the TRPG. This time around, I won't use Unity, but Urho3D to create a prototype for Android. Again, this time around, the project won't be in 3D but will be in 2D. The goal is to do a proof of concept to show that I can develop a hard mode AI to make it impossible for players to win against it. In order to do so, I am planning to use diverse machine learning techniques and genetic algorithms in order to use Darwin's idea in order to select the strongest and most adapted AI for the hard mode.

Lately, I've been really into functional programming and F#. For the project, I intend to do it in F#. Since it's a port of AugmentedTactics, the first thing will be to take the implementation and port in a functional style with F# for Urho3D.


## License 

For the project, I have establish a MIT license.
