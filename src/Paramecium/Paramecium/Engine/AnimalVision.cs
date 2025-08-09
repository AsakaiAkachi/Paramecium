using Paramecium.Variables;

namespace Paramecium.Engine
{
    public static class AnimalVision
    {
        // 動物の周辺状況の認識(視覚)の処理
        public static AnimalVisionOutput Observe(Soup soup, SoupSettings settings, Double2d position, double angle, int index, Double4d speciesSignature, int rayCount, int raySamplingCount, double rayLength, double viewingAngle)
        {
            AnimalVisionOutput result = new AnimalVisionOutput();

            Double2d frontVector = Double2d.FromAngle01(angle);
            double raySamplingRadius = rayLength / raySamplingCount / 2d;

            double wallTotalWeight = 0d;
            double plantTotalWeight = 0d;
            double animalTotalWeight = 0d;

            for (int i = 0; i < rayCount; i++)
            {
                double rayAngle = double.Lerp(-(viewingAngle / 2d), viewingAngle / 2d, 1d / (rayCount - 1) * i);
                Double2d rayVector = Double2d.Rotate01(frontVector, rayAngle);

                double unweightedAngle = rayAngle / (viewingAngle / 2d);

                Double2d prevSamplingPosition = position;

                for (int j = 1; j <= raySamplingCount; j++)
                {
                    Double2d samplingPosition = Double2d.Lerp(position, position + rayVector * rayLength, (double)j / raySamplingCount);

                    double weight = 1d / (j * j);

                    double proximity = (1d - (double)j / raySamplingCount) * weight;
                    double distance = (double)j / raySamplingCount * weight;

                    if (samplingPosition.X < 0d || samplingPosition.X > settings.SizeX || samplingPosition.Y < 0d || samplingPosition.Y > settings.SizeY)
                    {
                        wallTotalWeight += weight;
                        result.WallWAvgAngle = unweightedAngle * weight;
                        result.WallWAvgProximity += proximity;
                        result.WallWAvgDistance += distance;

                        break;
                    }
                    if (soup.Tiles[soup.GetTileIndexFromPosition(samplingPosition)].Type == TileType.Wall)
                    {
                        wallTotalWeight += weight;
                        result.WallWAvgAngle = unweightedAngle * weight;
                        result.WallWAvgProximity += proximity;
                        result.WallWAvgDistance += distance;

                        break;
                    }

                    bool wallHitFlag = false;

                    for (int x = 0; x <= 1; x++)
                    {
                        for (int y = 0; y <= 1; y++)
                        {
                            int samplingTileIndex = soup.GetTileIndexFromPosition(samplingPosition + new Double2d(-raySamplingRadius, -raySamplingRadius) + new Double2d(raySamplingRadius * 2d * x, raySamplingRadius * 2d * y));
                            Tile targetTile = soup.Tiles[samplingTileIndex];

                            if (targetTile.Type == TileType.Wall)
                            {
                                wallHitFlag = true;

                                wallTotalWeight += weight;
                                result.WallWAvgAngle = unweightedAngle * weight;
                                result.WallWAvgProximity += proximity;
                                result.WallWAvgDistance += distance;

                                continue;
                            }

                            for (int k = 0; k < targetTile.PlantPopulation; k++)
                            {
                                Plant? targetPlant = soup.Plants[targetTile.PlantIndexes[k]];

                                if (targetPlant is not null)
                                {
                                    if (Double2d.DistanceSquared(samplingPosition, targetPlant.Position) < Math.Min(raySamplingRadius * raySamplingRadius, targetPlant.Radius * targetPlant.Radius))
                                    {
                                        plantTotalWeight += weight;
                                        result.PlantWAvgAngle += unweightedAngle * weight;
                                        result.PlantWAvgProximity += proximity;
                                        result.PlantWAvgDistance += distance;
                                    }
                                }
                            }
                            for (int k = 0; k < targetTile.AnimalPopulation; k++)
                            {
                                Animal? targetAnimal = soup.Animals[targetTile.AnimalIndexes[k]];

                                if (targetAnimal is not null)
                                {
                                    if (targetAnimal.Age >= 0)
                                    {
                                        if (Double2d.DistanceSquared(samplingPosition, targetAnimal.Position) < Math.Min(raySamplingRadius * raySamplingRadius, targetAnimal.Radius * targetAnimal.Radius))
                                        {
                                            animalTotalWeight += weight;
                                            result.AnimalWAvgAngle += unweightedAngle * weight;
                                            result.AnimalWAvgProximity += proximity;
                                            result.AnimalWAvgDistance += distance;

                                            if (targetAnimal.SpeciesSignature == speciesSignature)
                                            {
                                                result.AnimalWAvgSpeciesSigDiff += 0d * weight;
                                            }
                                            else
                                            {
                                                result.AnimalWAvgSpeciesSigDiff += 1d * weight;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (wallHitFlag) break;
                }
            }

            if (wallTotalWeight > 0)
            {
                result.WallWAvgAngle /= wallTotalWeight;
                result.WallWAvgProximity /= wallTotalWeight;
                result.WallWAvgDistance /= wallTotalWeight;
            }
            else
            {
                result.WallWAvgAngle = 0d;
                result.WallWAvgProximity = 0d;
                result.WallWAvgDistance = 1d;
            }

            if (plantTotalWeight > 0)
            {
                result.PlantWAvgAngle /= plantTotalWeight;
                result.PlantWAvgProximity /= plantTotalWeight;
                result.PlantWAvgDistance /= plantTotalWeight;
            }
            else
            {
                result.PlantWAvgAngle = 0d;
                result.PlantWAvgProximity = 0d;
                result.PlantWAvgDistance = 1d;
            }

            if (animalTotalWeight > 0)
            {
                result.AnimalWAvgAngle /= animalTotalWeight;
                result.AnimalWAvgProximity /= animalTotalWeight;
                result.AnimalWAvgDistance /= animalTotalWeight;
                result.AnimalWAvgSpeciesSigDiff /= animalTotalWeight;
            }
            else
            {
                result.AnimalWAvgAngle = 0d;
                result.AnimalWAvgProximity = 0d;
                result.AnimalWAvgDistance = 1d;
                result.AnimalWAvgSpeciesSigDiff = 0d;
            }

            double pheromoneRedConcentration = 0d;
            double pheromoneGreenConcentration = 0d;
            double pheromoneBlueConcentration = 0d;
            Double2d pheromoneRedGradAngleVector = Double2d.Zero;
            Double2d pheromoneGreenGradAngleVector = Double2d.Zero;
            Double2d pheromoneBlueGradAngleVector = Double2d.Zero;

            double maximumEffectivePheromoneAmount = settings.MaximumEffectivePheromoneAmount;

            for (int x = 0; x <= 1; x++)
            {
                for (int y = 0; y <= 1; y++)
                {
                    Double2d pheromoneSamplingPosition = position + new Double2d(-0.5d, -0.5d) + new Double2d(x, y);

                    if (pheromoneSamplingPosition.X >= 0d && pheromoneSamplingPosition.X <= settings.SizeX && pheromoneSamplingPosition.Y >= 0d && pheromoneSamplingPosition.Y <= settings.SizeY)
                    {
                        Tile targetTile = soup.Tiles[soup.GetTileIndexFromPosition(pheromoneSamplingPosition)];

                        pheromoneRedConcentration += double.Min(1d, Math.Sqrt(targetTile.PheromoneRed / maximumEffectivePheromoneAmount));
                        pheromoneRedGradAngleVector += new Double2d(x, y) * targetTile.PheromoneRed;
                        pheromoneGreenConcentration += double.Min(1d, Math.Sqrt(targetTile.PheromoneGreen / maximumEffectivePheromoneAmount));
                        pheromoneGreenGradAngleVector += new Double2d(x, y) * targetTile.PheromoneGreen;
                        pheromoneBlueConcentration += double.Min(1d, Math.Sqrt(targetTile.PheromoneBlue / maximumEffectivePheromoneAmount));
                        pheromoneBlueGradAngleVector += new Double2d(x, y) * targetTile.PheromoneBlue;
                    }
                }
            }

            result.PheromoneRedConcentration = pheromoneRedConcentration / 4d;
            result.PheromoneGreenConcentration = pheromoneGreenConcentration / 4d;
            result.PheromoneBlueConcentration = pheromoneBlueConcentration / 4d;
            if (result.PheromoneRedConcentration > 0d) result.PheromoneRedGradAngle = Double2d.ToAngle01(Double2d.Rotate01(pheromoneRedGradAngleVector.Normalized, -angle)) * result.PheromoneRedConcentration;
            if (result.PheromoneGreenConcentration > 0d) result.PheromoneGreenGradAngle = Double2d.ToAngle01(Double2d.Rotate01(pheromoneGreenGradAngleVector.Normalized, -angle)) * result.PheromoneGreenConcentration;
            if (result.PheromoneBlueConcentration > 0d) result.PheromoneBlueGradAngle = Double2d.ToAngle01(Double2d.Rotate01(pheromoneBlueGradAngleVector.Normalized, -angle)) * result.PheromoneBlueConcentration;

            return result;
        }
    }
}
