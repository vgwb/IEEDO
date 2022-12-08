# Minigame Math Order

![](img/minigame_math_order.excalidraw2.png)

**Description**
tap the number in ascending (or descending) order before the time finishes.

**Difficulty** 
the level (1-100) changes automatically these parameters
- the grid size
- how many numbers are shown
- the "distance" between numbers

**Skills**
- Processing Speed
- Visual Scanning
- Working Memory

**Result**
high score and level progression

**UI**
- Board: grid dynamic (starts at 4\*4 -> 10\*10)
- Elements: coloured square with number (from - 999 to 999)
- some gfx when a cell is done right (+1) or wrong (-1)
- timer
- an arrow to show if to play in ascending or descending

**UX**
tap on a grid cell

**Gameplay:**
- the board is filled with n random numbers
- you are given a direction.
- timer is set to 60 seconds.
- START panel
- you click the numbers
- if you click a correct number, it disappears with a +1 gfx, and score gets +1
- if you click a wrong number, a gfx with a -1 appears and score gets -1
- if you finish the board, you go to next level. restart timer.
- when the timers ends, the game ends and get score
