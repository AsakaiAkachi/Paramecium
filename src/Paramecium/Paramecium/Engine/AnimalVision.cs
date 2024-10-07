using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paramecium.Engine
{
    public static class AnimalVision
    {
        public static AnimalVisionOutput Observe(Double2d originPosition, double angle, long originId, long originSpeciesId, int frontViewRange, int frontViewRayCount, double frontViewAngleRange, int backViewRange, int backViewRayCount, double backViewAngleRange)
        {
            if (g_Soup is null || !g_Soup.Initialized) throw new SoupNotCreatedOrInitializedException();

            int soupSizeX = g_Soup.Settings.SizeX;
            int soupSizeY = g_Soup.Settings.SizeY;

            AnimalVisionOutput result = new AnimalVisionOutput();

            double WallAvgAngleDenominator = 0;
            double PlantAvgAngleDenominator = 0;
            double AnimalSameSpeciesAvgAngleDenominator = 0;
            double AnimalOtherSpeciesAvgAngleDenominator = 0;

            double frontViewRangeDouble = frontViewRange;

            for (int i = 0; i < frontViewRayCount; i++)
            {
                double rayAngle = -(frontViewAngleRange / 2d) + frontViewAngleRange / (frontViewRayCount - 1) * i;
                if (rayAngle > 0.5d) rayAngle -= 1;
                if (rayAngle < -0.5d) rayAngle += 1;
                Double2d rayVector = Double2d.FromAngle(angle + rayAngle);

                for (int j = 0; j < frontViewRange; j++)
                {
                    Double2d rayPosition = originPosition + rayVector * (j + 1);
                    bool WallHitFlag = false;

                    for (int x = 0; x <= 1; x++)
                    {
                        for (int y = 0; y <= 1; y++)
                        {
                            Double2d rayScanPosition = rayPosition + new Double2d(-0.5d, -0.5d) + new Double2d(x, y);

                            if (rayScanPosition.X < 0d || rayScanPosition.X > soupSizeX || rayScanPosition.Y < 0d || rayScanPosition.Y > soupSizeY)
                            {
                                result.WallAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRangeDouble);
                                result.WallProximity = double.Max(result.WallProximity, ((frontViewRange - j) / frontViewRangeDouble));
                                WallAvgAngleDenominator += ((frontViewRange - j) / frontViewRangeDouble);
                                WallHitFlag = true;
                            }
                            else
                            {
                                int rayScanPositionIntegerizedX = int.Max(0, int.Min(soupSizeX - 1, (int)double.Floor(rayScanPosition.X)));
                                int rayScanPositionIntegerizedY = int.Max(0, int.Min(soupSizeY - 1, (int)double.Floor(rayScanPosition.Y)));

                                Tile targetTile = g_Soup.Tiles[rayScanPositionIntegerizedY * soupSizeX + rayScanPositionIntegerizedX];

                                if (targetTile.Type == TileType.Wall)
                                {
                                    result.WallAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRangeDouble);
                                    result.WallProximity = double.Max(result.WallProximity, ((frontViewRange - j) / frontViewRangeDouble));
                                    WallAvgAngleDenominator += ((frontViewRange - j) / frontViewRangeDouble);
                                    WallHitFlag = true;
                                }
                            }
                        }
                    }

                    for (int x = 0; x <= 1; x++)
                    {
                        for (int y = 0; y <= 1; y++)
                        {
                            if (
                                !WallHitFlag ||
                                (rayPosition.X >= originPosition.X && rayPosition.Y >= originPosition.Y && x == 0 && y == 0) ||
                                (rayPosition.X >= originPosition.X && rayPosition.Y < originPosition.Y && x == 0 && y == 1) ||
                                (rayPosition.X < originPosition.X && rayPosition.Y >= originPosition.Y && x == 1 && y == 0) ||
                                (rayPosition.X < originPosition.X && rayPosition.Y < originPosition.Y && x == 1 && y == 1)
                            )
                            {
                                Double2d rayScanPosition = rayPosition + new Double2d(-0.5d, -0.5d) + new Double2d(x, y);

                                int rayScanPositionIntegerizedX = int.Max(0, int.Min(soupSizeX - 1, (int)double.Floor(rayScanPosition.X)));
                                int rayScanPositionIntegerizedY = int.Max(0, int.Min(soupSizeY - 1, (int)double.Floor(rayScanPosition.Y)));

                                Tile targetTile = g_Soup.Tiles[rayScanPositionIntegerizedY * soupSizeX + rayScanPositionIntegerizedX];

                                if (targetTile.LocalPlantPopulation > 0)
                                {
                                    for (int k = 0; k < targetTile.LocalPlantPopulation; k++)
                                    {
                                        Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[k]];

                                        if (Double2d.DistanceSquared(rayPosition, targetPlant.Position) < 0.5d * 0.5d)
                                        {
                                            result.PlantAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRangeDouble);
                                            result.PlantProximity = double.Max(result.PlantProximity, ((frontViewRange - j) / frontViewRangeDouble));
                                            PlantAvgAngleDenominator += ((frontViewRange - j) / frontViewRangeDouble);
                                        }
                                    }
                                }
                                if (targetTile.LocalAnimalPopulation > 0)
                                {
                                    for (int k = 0; k < targetTile.LocalAnimalPopulation; k++)
                                    {
                                        Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[k]];

                                        if (targetAnimal.Id != originId)
                                        {
                                            if (targetAnimal.SpeciesId == originSpeciesId)
                                            {
                                                if (Double2d.DistanceSquared(rayPosition, targetAnimal.Position) < 0.5d * 0.5d)
                                                {
                                                    result.AnimalSameSpeciesAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRangeDouble);
                                                    result.AnimalSameSpeciesProximity = double.Max(result.AnimalSameSpeciesProximity, ((frontViewRange - j) / frontViewRangeDouble));
                                                    AnimalSameSpeciesAvgAngleDenominator += ((frontViewRange - j) / frontViewRangeDouble);
                                                }
                                            }
                                            else
                                            {
                                                if (Double2d.DistanceSquared(rayPosition, targetAnimal.Position) < 0.5d * 0.5d)
                                                {
                                                    result.AnimalOtherSpeciesAvgAngle += rayAngle * 2d * ((frontViewRange - j) / frontViewRangeDouble);
                                                    result.AnimalOtherSpeciesProximity = double.Max(result.AnimalOtherSpeciesProximity, ((frontViewRange - j) / frontViewRangeDouble));
                                                    AnimalOtherSpeciesAvgAngleDenominator += ((frontViewRange - j) / frontViewRangeDouble);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (WallHitFlag) break;
                }
            }
            for (int i = 0; i < backViewRayCount; i++)
            {
                double rayAngle = 0.5d + -(backViewAngleRange / 2d) + backViewAngleRange / (backViewRayCount - 1) * i;
                if (rayAngle > 0.5d) rayAngle -= 1;
                if (rayAngle < -0.5d) rayAngle += 1;
                Double2d rayVector = Double2d.FromAngle(angle + rayAngle);

                double backViewRangeDouble = backViewRange;

                for (int j = 0; j < backViewRange; j++)
                {
                    Double2d rayPosition = originPosition + rayVector * (j + 1);
                    bool WallHitFlag = false;

                    for (int x = 0; x <= 1; x++)
                    {
                        for (int y = 0; y <= 1; y++)
                        {
                            Double2d rayScanPosition = rayPosition + new Double2d(-0.5d, -0.5d) + new Double2d(x, y);

                            if (rayScanPosition.X < 0d || rayScanPosition.X > soupSizeX || rayScanPosition.Y < 0d || rayScanPosition.Y > soupSizeY)
                            {
                                result.WallAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRangeDouble);
                                result.WallProximity = double.Max(result.WallProximity, ((backViewRange - j) / backViewRangeDouble));
                                WallAvgAngleDenominator += ((backViewRange - j) / backViewRangeDouble);
                                WallHitFlag = true;
                            }
                            else
                            {
                                int rayScanPositionIntegerizedX = int.Max(0, int.Min(soupSizeX - 1, (int)double.Floor(rayScanPosition.X)));
                                int rayScanPositionIntegerizedY = int.Max(0, int.Min(soupSizeY - 1, (int)double.Floor(rayScanPosition.Y)));

                                Tile targetTile = g_Soup.Tiles[rayScanPositionIntegerizedY * soupSizeX + rayScanPositionIntegerizedX];

                                if (targetTile.Type == TileType.Wall)
                                {
                                    result.WallAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRangeDouble);
                                    result.WallProximity = double.Max(result.WallProximity, ((backViewRange - j) / backViewRangeDouble));
                                    WallAvgAngleDenominator += ((backViewRange - j) / backViewRangeDouble);
                                    WallHitFlag = true;
                                }
                            }
                        }
                    }

                    for (int x = 0; x <= 1; x++)
                    {
                        for (int y = 0; y <= 1; y++)
                        {
                            if (
                                !WallHitFlag ||
                                (rayPosition.X >= originPosition.X && rayPosition.Y >= originPosition.Y && x == 0 && y == 0) ||
                                (rayPosition.X >= originPosition.X && rayPosition.Y < originPosition.Y && x == 0 && y == 1) ||
                                (rayPosition.X < originPosition.X && rayPosition.Y >= originPosition.Y && x == 1 && y == 0) ||
                                (rayPosition.X < originPosition.X && rayPosition.Y < originPosition.Y && x == 1 && y == 1)
                            )
                            {
                                Double2d rayScanPosition = rayPosition + new Double2d(-0.5d, -0.5d) + new Double2d(x, y);

                                int rayScanPositionIntegerizedX = int.Max(0, int.Min(soupSizeX - 1, (int)double.Floor(rayScanPosition.X)));
                                int rayScanPositionIntegerizedY = int.Max(0, int.Min(soupSizeY - 1, (int)double.Floor(rayScanPosition.Y)));

                                Tile targetTile = g_Soup.Tiles[rayScanPositionIntegerizedY * soupSizeX + rayScanPositionIntegerizedX];

                                if (targetTile.LocalPlantPopulation > 0)
                                {
                                    for (int k = 0; k < targetTile.LocalPlantPopulation; k++)
                                    {
                                        Plant targetPlant = g_Soup.Plants[targetTile.LocalPlantIndexes[k]];

                                        if (Double2d.DistanceSquared(rayPosition, targetPlant.Position) < 0.5d * 0.5d)
                                        {
                                            result.PlantAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRangeDouble);
                                            result.PlantProximity = double.Max(result.PlantProximity, ((backViewRange - j) / backViewRangeDouble));
                                            PlantAvgAngleDenominator += ((backViewRange - j) / backViewRangeDouble);
                                        }
                                    }
                                }
                                if (targetTile.LocalAnimalPopulation > 0)
                                {
                                    for (int k = 0; k < targetTile.LocalAnimalPopulation; k++)
                                    {
                                        Animal targetAnimal = g_Soup.Animals[targetTile.LocalAnimalIndexes[k]];

                                        if (targetAnimal.Id != originId)
                                        {
                                            if (targetAnimal.SpeciesId == originSpeciesId)
                                            {
                                                if (Double2d.DistanceSquared(rayPosition, targetAnimal.Position) < 0.5d * 0.5d)
                                                {
                                                    result.AnimalSameSpeciesAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRangeDouble);
                                                    result.AnimalSameSpeciesProximity = double.Max(result.AnimalSameSpeciesProximity, ((backViewRange - j) / backViewRangeDouble));
                                                    AnimalSameSpeciesAvgAngleDenominator += ((backViewRange - j) / backViewRangeDouble);
                                                }
                                            }
                                            else
                                            {
                                                if (Double2d.DistanceSquared(rayPosition, targetAnimal.Position) < 0.5d * 0.5d)
                                                {
                                                    result.AnimalOtherSpeciesAvgAngle += rayAngle * 2d * ((backViewRange - j) / backViewRangeDouble);
                                                    result.AnimalOtherSpeciesProximity = double.Max(result.AnimalOtherSpeciesProximity, ((backViewRange - j) / backViewRangeDouble));
                                                    AnimalOtherSpeciesAvgAngleDenominator += ((backViewRange - j) / backViewRangeDouble);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (WallHitFlag) break;
                }
            }

            if (WallAvgAngleDenominator > 0) result.WallAvgAngle /= WallAvgAngleDenominator;
            if (PlantAvgAngleDenominator > 0) result.PlantAvgAngle /= PlantAvgAngleDenominator;
            if (AnimalSameSpeciesAvgAngleDenominator > 0) result.AnimalSameSpeciesAvgAngle /= AnimalSameSpeciesAvgAngleDenominator;
            if (AnimalOtherSpeciesAvgAngleDenominator > 0) result.AnimalOtherSpeciesAvgAngle /= AnimalOtherSpeciesAvgAngleDenominator;

            double pheromoneRed = 0;
            double pheromoneGreen = 0;
            double pheromoneBlue = 0;
            Double2d pheromoneRedAvgVector = Double2d.Zero;
            Double2d pheromoneGreenAvgVector = Double2d.Zero;
            Double2d pheromoneBlueAvgVector = Double2d.Zero;

            for (int x = 0; x <= 1; x++)
            {
                for (int y = 0; y <= 1; y++)
                {
                    Double2d pheromoneScanPosition = originPosition + new Double2d(-0.5d, -0.5d) + new Double2d(x, y);
                    pheromoneScanPosition = new Double2d(double.Max(0, double.Min(soupSizeX, pheromoneScanPosition.X)), double.Max(0, double.Min(soupSizeY, pheromoneScanPosition.Y)));

                    int pheromoneScanPositionIntegerizedX = int.Max(0, int.Min(soupSizeX - 1, (int)double.Floor(pheromoneScanPosition.X)));
                    int pheromoneScanPositionIntegerizedY = int.Max(0, int.Min(soupSizeY - 1, (int)double.Floor(pheromoneScanPosition.Y)));

                    double tileRedPheromoneAmount = g_Soup.Tiles[pheromoneScanPositionIntegerizedY * soupSizeX + pheromoneScanPositionIntegerizedX].PheromoneRed;
                    double tileGreenPheromoneAmount = g_Soup.Tiles[pheromoneScanPositionIntegerizedY * soupSizeX + pheromoneScanPositionIntegerizedX].PheromoneGreen;
                    double tileBluePheromoneAmount = g_Soup.Tiles[pheromoneScanPositionIntegerizedY * soupSizeX + pheromoneScanPositionIntegerizedX].PheromoneBlue;

                    if (tileRedPheromoneAmount >= 0.0005d)
                    {
                        pheromoneRed += tileRedPheromoneAmount;
                        pheromoneRedAvgVector += (new Double2d(-0.5d, -0.5d) + new Double2d(x, y)) * tileRedPheromoneAmount;
                    }
                    if (tileGreenPheromoneAmount >= 0.0005d)
                    {
                        pheromoneGreen += tileGreenPheromoneAmount;
                        pheromoneGreenAvgVector += (new Double2d(-0.5d, -0.5d) + new Double2d(x, y)) * tileGreenPheromoneAmount;
                    }
                    if (tileBluePheromoneAmount >= 0.0005d)
                    {
                        pheromoneBlue += tileBluePheromoneAmount;
                        pheromoneBlueAvgVector += (new Double2d(-0.5d, -0.5d) + new Double2d(x, y)) * tileBluePheromoneAmount;
                    }
                }
            }

            result.PheromoneRed = pheromoneRed / 4d;
            result.PheromoneGreen = pheromoneGreen / 4d;
            result.PheromoneBlue = pheromoneBlue / 4d;
            if (pheromoneRed > 0)
            {
                result.PheromoneRedAvgAngle = Double2d.ToAngle(pheromoneRedAvgVector) - angle;
                if (result.PheromoneRedAvgAngle < -0.5d) result.PheromoneRedAvgAngle += 1d;
                if (result.PheromoneRedAvgAngle > 0.5d) result.PheromoneRedAvgAngle -= 1d;
            }
            if (pheromoneGreen > 0)
            {
                result.PheromoneGreenAvgAngle = Double2d.ToAngle(pheromoneGreenAvgVector) - angle;
                if (result.PheromoneGreenAvgAngle < -0.5d) result.PheromoneGreenAvgAngle += 1d;
                if (result.PheromoneGreenAvgAngle > 0.5d) result.PheromoneGreenAvgAngle -= 1d;
            }
            if (pheromoneBlue > 0)
            {
                result.PheromoneBlueAvgAngle = Double2d.ToAngle(pheromoneBlueAvgVector) - angle;
                if (result.PheromoneBlueAvgAngle < -0.5d) result.PheromoneBlueAvgAngle += 1d;
                if (result.PheromoneBlueAvgAngle > 0.5d) result.PheromoneBlueAvgAngle -= 1d;
            }

            return result;
        }
    }
}
