---
layout: default
title: Game Design
nav_order: 2
has_children: true
---

## FA Icons
<https://fontawesome.com/v5/cheatsheet>

## Points

| Action | Points |
| --- | --- |
|Card Completed | +40|
|Card Uncompleted | -40|
|Card Validated | +60|
|Card Unvalidated | -60|
| | |
| Diary | +20 |
| 2048 | 64 (if makes 2048)|
| TicTacToe | 1 (if wins) |
| Unblock | 1 (if new level completed) |
| Fast Reaction | 2 |

## Activity end

Activities can be of `ScoreType`:

### Highscore
you play to beat your highscore.
infinite playes.

Activity **Intro**:  
`Your Hi-Score is: n`  

`PLAY`

Activity **End**:  
`Your score is n`  
`Your hi-score is n`  
`NEW HIGHSCORE!` (eventually)

EXIT / PLAY AGAIN

### LevelReached
you play to reach new levels.

Activity **Intro**:  
`Level: n`  

`PLAY`

if **WIN**:

Activity **End**:  
`Level: n`
`COMPLETED`  

EXIT / PLAY NEXT LEVEL

if **LOST**:

Activity **End**:  
`Level: n`
`FAILED`  

EXIT / PLAY AGAIN

### NumberOfPlays
Activity **Intro**:  
introduction

Activity **End**:  
game_end

`EXIT`
