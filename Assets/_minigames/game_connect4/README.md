# Unity-game-Connect4

original game: https://github.com/lmiletic/Unity-game-Connect4

Connect Four is a two-player connection board game, in which the players choose a color and then take turns dropping colored discs into a seven-column, six-row vertically suspended grid. The pieces fall straight down, occupying the lowest available space within the column. The objective of the game is to be the first to form a horizontal, vertical, or diagonal line of four of one's own discs. In this project a player is playing against AI implemented with Minimax Algorithm with Alpha-beta pruning.


Game is fully built in Unity with one scene featuring table, board and discs. If a player hover the column the disk will be summoned above hovered column, and by clicking on it, disk will fall straight down and occupy lowest part of column. Board state is represented by 6x7 matrix which can contain 0, 1 or 2 (0 - empty filed, 1 - red disk, 2 - blue disk).


In the game tree, nodes(states) are represented by matrix(board) and branches(players moves) are represented by columns that are not full. Player can choose on which difficulty he wants to play easy, medium or hard. Picking harder difficulty will let AI to go deeper in game tree and find better move.
