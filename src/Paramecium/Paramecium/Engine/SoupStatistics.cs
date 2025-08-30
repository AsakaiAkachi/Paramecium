using static Paramecium.Engine.SoupStatistics.ChangeOverTimeDatas;

namespace Paramecium.Engine
{
    public class SoupStatistics
    {
        public ChangeOverTimeDatas ChangeOverTimeData { get; set; } = new ChangeOverTimeDatas();
        public DistributionDatas DistributionData { get; set; } = new DistributionDatas();

        public SoupStatistics() { }

        public class ChangeOverTimeDatas
        {
            public DataEntries[] Datas { get; set; } = new DataEntries[11]
            {
                new DataEntries(1),
                new DataEntries(2),
                new DataEntries(4),
                new DataEntries(8),
                new DataEntries(16),
                new DataEntries(32),
                new DataEntries(64),
                new DataEntries(128),
                new DataEntries(256),
                new DataEntries(512),
                new DataEntries(1024)
            };

            public ChangeOverTimeDatas() { }

            public void UpdateChangeOverTimeData(Soup soup)
            {
                DataEntry dataEntry = new DataEntry(soup);

                for (int i = 0; i < Datas.Length; i++)
                {
                    Datas[i].RegisterDataEntry(dataEntry, soup.ElapsedTimeSteps);
                }
            }

            public class DataEntries
            {
                public static readonly int MaxEntriesCountByTimespan = 10000;

                public List<DataEntry> DataEntryList { get; set; } = new List<DataEntry>();
                public int DataEntryListIndexPointer { get; set; } = 0;
                public object DataEntryListLockObject = new object();

                public int TimeSpan { get; set; } = 1;

                public DataEntries() { }
                public DataEntries(int timeSpan)
                {
                    TimeSpan = timeSpan;
                }

                public void RegisterDataEntry(DataEntry dataEntry, long timeSteps)
                {
                    lock (DataEntryListLockObject)
                    {
                        if (timeSteps % TimeSpan == 0)
                        {
                            if (DataEntryList.Count >= MaxEntriesCountByTimespan)
                            {
                                DataEntryListIndexPointer++;
                                if (DataEntryListIndexPointer >= MaxEntriesCountByTimespan) DataEntryListIndexPointer = 0;
                                if (DataEntryListIndexPointer < 0) DataEntryListIndexPointer = 0;

                                DataEntryList[DataEntryListIndexPointer] = dataEntry;
                            }
                            else
                            {
                                DataEntryList.Add(dataEntry);
                                DataEntryListIndexPointer = DataEntryList.Count - 1;
                            }
                        }
                    }
                }

                public long GetStartTimestep()
                {
                    long result = long.MaxValue;

                    lock (DataEntryListLockObject)
                    {
                        for (int i = 0; i < DataEntryList.Count; i++)
                        {
                            result = long.Min(result, DataEntryList[i].TimeStep);
                        }
                    }

                    return result;
                }
                public long GetEndTimestep()
                {
                    long result = long.MinValue;

                    lock (DataEntryListLockObject)
                    {
                        for (int i = 0; i < DataEntryList.Count; i++)
                        {
                            result = long.Max(result, DataEntryList[i].TimeStep);
                        }
                    }

                    return result;
                }

                public List<DataEntry> GetDataEntries()
                {
                    lock (DataEntryListLockObject)
                    {
                        if (DataEntryList.Count >= MaxEntriesCountByTimespan)
                        {
                            List<DataEntry> result = new List<DataEntry>();

                            for (int i = 0; i < DataEntryList.Count; i++)
                            {
                                result.Add(DataEntryList[(i + DataEntryListIndexPointer + 1) % DataEntryList.Count]);
                            }

                            return result;
                        }
                        else
                        {
                            List<DataEntry> result = new List<DataEntry>();

                            for (int i = 0; i < DataEntryList.Count; i++)
                            {
                                result.Add(DataEntryList[i]);
                            }

                            return result;
                        }
                    }
                }
            }

            public class DataEntry
            {
                public long TimeStep { get; set; }

                public int AnimalPopulation { get; set; }
                public long LatestGeneration { get; set; }
                public long MaxMutationCount { get; set; }

                public DataEntry() { }
                public DataEntry(Soup soup)
                {
                    TimeStep = soup.ElapsedTimeSteps;

                    AnimalPopulation = soup.AnimalPopulation;
                    LatestGeneration = soup.LatestGeneration;
                    MaxMutationCount = 0;

                    for (int i = 0; i < soup.Animals.Count; i++)
                    {
                        Animal? targetAnimal = soup.Animals[i];

                        if (targetAnimal is not null)
                        {
                            MaxMutationCount = long.Max(MaxMutationCount, targetAnimal.MutationCount);
                        }
                    }
                }

                public static List<long> GetAnimalPopulationData(List<DataEntry> dataEntries)
                {
                    List<long> result = new List<long>();

                    for (int i = 0; i < dataEntries.Count; i++)
                    {
                        result.Add(dataEntries[i].AnimalPopulation);
                    }

                    return result;
                }
                public static List<long> GetLatestGenerationData(List<DataEntry> dataEntries)
                {
                    List<long> result = new List<long>();

                    for (int i = 0; i < dataEntries.Count; i++)
                    {
                        result.Add(dataEntries[i].LatestGeneration);
                    }

                    return result;
                }
                public static List<long> GetMaxMutationCountData(List<DataEntry> dataEntries)
                {
                    List<long> result = new List<long>();

                    for (int i = 0; i < dataEntries.Count; i++)
                    {
                        result.Add(dataEntries[i].MaxMutationCount);
                    }

                    return result;
                }
            }
        }

        public class DistributionDatas
        {
            public List<long> MutationCount { get; set; } = new List<long>();
            public List<long> Generation { get; set; } = new List<long>();
            public List<long> Age { get; set; } = new List<long>();
            public List<long> OffspringCount { get; set; } = new List<long>();
            public List<double> VelocityMagnitude { get; set; } = new List<double>();
            public List<double> AngularVelocity { get; set; } = new List<double>();
            public List<double> Element { get; set; } = new List<double>();
            public List<double> ElementInOutflow { get; set; } = new List<double>();
            public List<double> ReproductionProgress { get; set; } = new List<double>();
            public List<double> ReproductionRate { get; set; } = new List<double>();
            public List<double> BrainEatOutput { get; set; } = new List<double>();
            public List<double> BrainAttackOutput { get; set; } = new List<double>();

            public void UpdateDistributionData(Soup soup)
            {
                List<long> mutationCount = new List<long>();
                List<long> generation = new List<long>();
                List<long> age = new List<long>();
                List<long> offspringCount = new List<long>();
                List<double> velocityMagnitude = new List<double>();
                List<double> angularVelocity = new List<double>();
                List<double> element = new List<double>();
                List<double> elementInOutflow = new List<double>();
                List<double> reproductionProgress = new List<double>();
                List<double> reproductionRate = new List<double>();
                List<double> brainEatOutput = new List<double>();
                List<double> brainAttackOutput = new List<double>();

                for (int i = 0; i < soup.Animals.Count; i++)
                {
                    Animal? targetAnimal = soup.Animals[i];

                    if (targetAnimal is not null)
                    {
                        mutationCount.Add(targetAnimal.MutationCount);
                        generation.Add(targetAnimal.Generation);
                        age.Add(targetAnimal.Age);
                        offspringCount.Add(targetAnimal.OffspringCount);
                        velocityMagnitude.Add(targetAnimal.Velocity.Magnitude);
                        angularVelocity.Add(targetAnimal.AngularVelocity);
                        element.Add(targetAnimal.Element);
                        elementInOutflow.Add(targetAnimal.ElementGainLoss + targetAnimal.ElementCostPerStep - targetAnimal.ReproductionRate);
                        reproductionProgress.Add(targetAnimal.ReproductionProgress);
                        reproductionRate.Add(targetAnimal.ReproductionRate);
                        brainEatOutput.Add(targetAnimal.Brain.Output.Eat);
                        brainAttackOutput.Add(targetAnimal.Brain.Output.Attack);
                    }
                }

                MutationCount = mutationCount;
                Generation = generation;
                Age = age;
                OffspringCount = offspringCount;
                VelocityMagnitude = velocityMagnitude;
                AngularVelocity = angularVelocity;
                Element = element;
                ElementInOutflow = elementInOutflow;
                ReproductionProgress = reproductionProgress;
                ReproductionRate = reproductionRate;
                BrainEatOutput = brainEatOutput;
                BrainAttackOutput = brainAttackOutput;
            }
        }
    }
}
