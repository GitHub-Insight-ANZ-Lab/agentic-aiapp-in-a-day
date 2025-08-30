

## PSR Game Process

# Starting game
- run server side here #file:apps/psr-game-server in dotnet, and make sure the frontend webpage is accessable
- run #file:apps/psr-game-client/main.py multiple times to simulate 8 players in the game, the clients should all be able register themselves to the tournament
- once all 8 players are registered, psr-game-server's web page should be ready to start the round. [Start Round] button should be enabled.

# Round 1 - The Opening Battles (8 Players → 4 Winners)
- Round 1 consists of 4 simultaneous matches where each pair of players competes in a best-of-3 series
- Match 1: Player 1 vs Player 2
- Match 2: Player 3 vs Player 4  
- Match 3: Player 5 vs Player 6
- Match 4: Player 7 vs Player 8

## Best-of-3 Series Process for each Match
- click [Start Round 1] button to start Round 1. [Start Match] button should be enabled
- then click [Start Match] button to start Game 1 of Match 1
- once [Start Match] button is clicked, the players can send their moves for Game 1
- when all players have sent their moves, the [Release Match Results] button should be enabled
- click [Release Match Results] will then display the winner of Game 1

### Continuing the Best-of-3:
- repeat the same process for Game 2 of Match 1
- repeat the same process for Game 3 of Match 1 (if needed)
- the player who wins 2 out of 3 games wins Match 1

### Completing All Matches in Round 1:
- repeat the entire best-of-3 process for Match 2 (Player 3 vs Player 4)
- repeat the entire best-of-3 process for Match 3 (Player 5 vs Player 6) 
- repeat the entire best-of-3 process for Match 4 (Player 7 vs Player 8)
- once all 4 matches are completed, [Release Round Results] button should be enabled
- click [Release Round Results] will show the 4 winners advancing to Round 2

# Round 2 - The Semi-Finals (4 Players → 2 Winners)
- the player clients need to know if they are going into next round of the tournament. if they are not winner, just stop the client
- for the winners of Round 1, they will go into the Round 2 of the tournament
- Round 2 consists of 2 simultaneous matches where the Round 1 winners compete
- Match 1: Round 1 Winner A vs Round 1 Winner B
- Match 2: Round 1 Winner C vs Round 1 Winner D

## Best-of-3 Series Process for Round 2
- click [Start Round 2] button to start Round 2
- follow the same best-of-3 process as Round 1 for Match 1
- follow the same best-of-3 process as Round 1 for Match 2
- the 2 match winners advance to the finals (Round 3)

# Round 3 - The Championship Final (2 Players → 1 Champion)
- Round 3 consists of 1 final match between the 2 remaining players
- Championship Match: Round 2 Winner 1 vs Round 2 Winner 2

## Best-of-3 Championship Series
- click [Start Round 3] button to begin the final round
- follow the same best-of-3 process as previous rounds
- the winner of this final match becomes the tournament champion
- the winner should be displayed prominently at the end of Round 3

