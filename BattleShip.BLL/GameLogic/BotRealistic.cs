using System;
using System.Collections.Generic;
using BattleShip.BLL.Requests;
using BattleShip.BLL.Responses;
using System.Diagnostics;

namespace BattleShip.BLL.GameLogic
{
    public class BotRealistic : BotBase
    {
        private BotSearchMode searchMode;
        private Coordinate lastHitCoords;
        private Coordinate nextFireCoords;
        private ShipDirection nextSeekDirection;

        public BotRealistic(Board board) : base(board) { }

        /// <summary>
        /// Chooses and fires at a random position on the board.
        /// </summary>
        /// <param name="chosenCoordinates">Will contain the coordinates that the bot chose.</param>
        /// <returns>A FireShotResponse object containing details of the shot's outcome.</returns>
        public override FireShotResponse TakeTurn(out Coordinate chosenCoordinates)
        {
            bool shotDecided = false;

            do
            {
                switch (searchMode)
                {
                    // Get a random location to fire at.
                    case BotSearchMode.Random:
                        nextFireCoords = shootableCoords[0];
                        nextSeekDirection = GetRandomDirection();
                        shotDecided = true;
                        break;
                    // We hit something randomly, so now we'll pick a random direction to fire at until ultimately we hit something.
                    // then we figured out the direction and we can follow it.
                    case BotSearchMode.FindDirection:
                        ShipDirection newSeekDirection = nextSeekDirection;

                        int rotations = 0;

                        bool foundNextCoords = false;
                        Coordinate newCoords;

                        do
                        {
                            // Set our fire coord to the coordinate moving one tile toward the direction we rotated to.
                            newCoords = GetNextCoordinateTowards(nextFireCoords, newSeekDirection);

                            // If the coordinate exists on the board and has not been fired at, we found our coord.
                            foundNextCoords = IsCoordinateInRange(newCoords) && CanFireOnCoordinate(newCoords);

                            // If we didn't find our coord, rotate again and search.
                            if (!foundNextCoords)
                            {
                                newSeekDirection = RotateDirection(newSeekDirection);
                                rotations++;
                            }
                        } while (!foundNextCoords && rotations < TOTAL_DIRECTIONS);

                        if (rotations < TOTAL_DIRECTIONS)
                        {
                            nextFireCoords = newCoords;
                            nextSeekDirection = newSeekDirection;
                            shotDecided = true;
                        }
                        else
                        {
                            searchMode = BotSearchMode.Random;
                        }
                        break;
                    // We figured out the direction to fire at! Let's just keep firing that direction.
                    case BotSearchMode.FollowDirection:
                        newCoords = GetNextCoordinateTowards(nextFireCoords, nextSeekDirection);

                        if (!IsCoordinateInRange(newCoords) || !CanFireOnCoordinate(newCoords))
                        {
                            searchMode = BotSearchMode.FollowDirectionReversed;
                        }
                        else
                        {
                            nextFireCoords = newCoords;
                            shotDecided = true;
                        }
                        break;
                    // We had the right direction, but we unexpectedly missed our last shot, which means when we first hit a ship,
                    // we must have hit it from the middle and followed to the end, so now we need to reverse direction to hit the tiles we missed.
                    case BotSearchMode.FollowDirectionReversed:
                        ShipDirection reversedDirection = GetReverseDirection(nextSeekDirection);
                        newCoords = nextFireCoords;

                        do // We've reversed direction, step over every coord already hit until we find one we did not hit.
                        {
                            newCoords = GetNextCoordinateTowards(newCoords, reversedDirection);
                        } while (IsCoordinateInRange(newCoords) && HasCoordBeenHit(newCoords));

                        // If what we found was a good place to fire, make that our next firing spot.
                        if (IsCoordinateInRange(newCoords) && CanFireOnCoordinate(newCoords))
                        {
                            nextFireCoords = newCoords;
                            shotDecided = true;
                        }

                        searchMode = BotSearchMode.FindDirection;
                        break;
                }
            } while (!shotDecided);
            
            FireShotResponse shotResponse = board.FireShot(nextFireCoords);
            chosenCoordinates = nextFireCoords;

            // Remove it from our shootable coord list so that we don't fire at it again.
            shootableCoords.Remove(chosenCoordinates);

#if DEBUG
            // Gives you a good idea on what the bot is doing.
            Debug.Write($"\t\t{searchMode} {shotResponse.ShotStatus} at {CoordinateToStr(nextFireCoords)}, ");
#endif
            // Our bot's next search is going to act differently on the next turn depending
            // on whether it hit a ship, sunk a ship or missed a ship on the current turn.
            switch (shotResponse.ShotStatus)
            {
                // When the bot sinks a ship, it should pick a random position to fire at on the next turn.
                case ShotStatus.HitAndSunk:
                    searchMode = BotSearchMode.Random;
                    break;
                case ShotStatus.Hit:
                    // If we got a hit while randomly searching, find the direction on next turn.
                    if (searchMode == BotSearchMode.Random)
                    {
                        searchMode = BotSearchMode.FindDirection;
                        // Record the last hit coordinates so that we know how to unwind.
                        lastHitCoords = nextFireCoords;
                    }
                    // If we were finding the direction and hit again, we know the direction, keep following it on the next turn.
                    else if (searchMode == BotSearchMode.FindDirection)
                    {
                        searchMode = BotSearchMode.FollowDirection;
                    }

                    break;
                case ShotStatus.Miss:
                    if (searchMode == BotSearchMode.FindDirection)
                    {
                        // We missed, back out and rotate our next seek direction.
                        nextFireCoords = lastHitCoords;
                    }
                    // We were following a line and suddenly missed!
                    else if (searchMode == BotSearchMode.FollowDirection)
                    {
                        searchMode = BotSearchMode.FollowDirectionReversed;
                    }
                    else if (searchMode == BotSearchMode.FollowDirectionReversed)
                    {
                        searchMode = BotSearchMode.FindDirection;
                    }
                    break;
            }

#if DEBUG
            Debug.WriteLine($"next decision: {searchMode}");
#endif
            return shotResponse;
        }
    }
}
