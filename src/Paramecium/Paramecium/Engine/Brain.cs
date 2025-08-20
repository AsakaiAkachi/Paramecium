using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Paramecium.Engine
{
    // 動物のニューラルネットが格納されているクラス
    public class Brain
    {

        public List<BrainNode> Nodes { get; set; } = new List<BrainNode>();                             // 脳のノード
        public List<BrainNodeConnection> Connections { get; set; } = new List<BrainNodeConnection>();   // 脳のノード間接続

        public BrainInput Input { get; set; } = new BrainInput();       // 入力
        public BrainOutput Output { get; set; } = new BrainOutput();    // 出力

        // Nodesに入っているノードの内特定の種類のノードのみを列挙する
        [JsonIgnore]
        public int[] InputNodeIndexes
        {
            get
            {
                List<int> targetNodeIndexes = new List<int>();
                for (int i = 0; i < Nodes.Count; i++) if (Nodes[i].IsInput) targetNodeIndexes.Add(i);
                return targetNodeIndexes.ToArray();
            }
        }
        [JsonIgnore]
        public int[] HiddenNodeIndexes
        {
            get
            {
                List<int> targetNodeIndexes = new List<int>();
                for (int i = 0; i < Nodes.Count; i++) if (Nodes[i].IsHidden) targetNodeIndexes.Add(i);
                return targetNodeIndexes.ToArray();
            }
        }
        [JsonIgnore]
        public int[] OutputNodeIndexes
        {
            get
            {
                List<int> targetNodeIndexes = new List<int>();
                for (int i = 0; i < Nodes.Count; i++) if (Nodes[i].IsOutput) targetNodeIndexes.Add(i);
                return targetNodeIndexes.ToArray();
            }
        }
        [JsonIgnore]
        public int[] InputAndHiddenNodeIndexes
        {
            get
            {
                List<int> targetNodeIndexes = new List<int>();
                for (int i = 0; i < Nodes.Count; i++) if (Nodes[i].IsInput || Nodes[i].IsHidden) targetNodeIndexes.Add(i);
                return targetNodeIndexes.ToArray();
            }
        }
        [JsonIgnore]
        public int[] OutputAndHiddenNodeIndexes
        {
            get
            {
                List<int> targetNodeIndexes = new List<int>();
                for (int i = 0; i < Nodes.Count; i++) if (Nodes[i].IsOutput || Nodes[i].IsHidden) targetNodeIndexes.Add(i);
                return targetNodeIndexes.ToArray();
            }
        }

        [JsonIgnore]
        public static readonly int IndexOfFirstInputTypeBrainNodeFunction = (int)BrainNodeFunction.Input_Bias;
        [JsonIgnore]
        public static readonly int IndexOfLastInputTypeBrainNodeFunction = (int)BrainNodeFunction.Input_PheromoneBlueGradAngle;
        [JsonIgnore]
        public static readonly int IndexOfFirstHiddenTypeBrainNodeFunction = (int)BrainNodeFunction.Hidden_ReLU;
        [JsonIgnore]
        public static readonly int IndexOfLastHiddenTypeBrainNodeFunction = (int)BrainNodeFunction.Hidden_Frac;
        [JsonIgnore]
        public static readonly int IndexOfFirstOutputTypeBrainNodeFunction = (int)BrainNodeFunction.Output_Acceleration;
        [JsonIgnore]
        public static readonly int IndexOfLastOutputTypeBrainNodeFunction = (int)BrainNodeFunction.Output_PheromoneBlueProduction;

        // デフォルトの脳を取得する
        [JsonIgnore]
        public static Brain DefaultBrain
        {
            get
            {
                Brain result = new Brain();

                result.Nodes.Add(new BrainNode() { Function = BrainNodeFunction.Input_Bias });
                result.Nodes.Add(new BrainNode() { Function = BrainNodeFunction.Input_PlantWAvgAngle });
                result.Nodes.Add(new BrainNode() { Function = BrainNodeFunction.Input_PlantWAvgProximity });
                result.Nodes.Add(new BrainNode() { Function = BrainNodeFunction.Output_Acceleration });
                result.Nodes.Add(new BrainNode() { Function = BrainNodeFunction.Output_Rotation });
                result.Nodes.Add(new BrainNode() { Function = BrainNodeFunction.Output_Eat });
                result.Nodes.Add(new BrainNode() { Function = BrainNodeFunction.Input_Element });
                result.Nodes.Add(new BrainNode() { Function = BrainNodeFunction.Output_Reproduction });

                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 0, TargetIndex = 3, Weight = 1 });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 0, TargetIndex = 5, Weight = -0.5 });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 1, TargetIndex = 4, Weight = 1 });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 2, TargetIndex = 3, Weight = -0.95 });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 2, TargetIndex = 5, Weight = 1 });
                result.Connections.Add(new BrainNodeConnection() { OriginIndex = 6, TargetIndex = 7, Weight = 0.5 });

                return result;
            }
        }

        public Brain() { }
        public Brain(Brain parentBrain)     // 動物が生殖した際に親から脳の構造を継承する用のコンストラクター
        {
            for (int i = 0; i < parentBrain.Nodes.Count; i++)
            {
                Nodes.Add(new BrainNode() { Function = parentBrain.Nodes[i].Function });
            }
            for (int i = 0; i < parentBrain.Connections.Count; i++)
            {
                Connections.Add(new BrainNodeConnection() { OriginIndex = parentBrain.Connections[i].OriginIndex, TargetIndex = parentBrain.Connections[i].TargetIndex, Weight = parentBrain.Connections[i].Weight });
            }
        }

        // 脳を突然変異させる処理
        public int TryMutation(SoupSettings settings)
        {
            Random rand = new Random();

            int mutationCount = 0;

            if (rand.NextDouble() < settings.AnimalMutationMutationRate)
            {
                for (int i = 0; i < settings.AnimalMutationMaximumMutationCount; i++)
                {
                    bool mutationSuccessful = Mutate(settings);
                    if (mutationSuccessful) mutationCount++;

                    if (rand.NextDouble() > settings.AnimalMutationMutationCountFactor) break;
                }
            }

            RemoveInvalidNode();

            return mutationCount;
        }

        public bool Mutate(SoupSettings settings)
        {
            int mutationType = WeightedSelector(settings.AnimalMutationTypeWeights);
            bool mutationSuccessful = false;

            if (mutationType == 0) mutationSuccessful = AddNodeMutation(settings);
            else if (mutationType == 1) mutationSuccessful = RemoveNodeMutation();
            else if (mutationType == 2) mutationSuccessful = ChangeNodeTypeMutation(settings);
            else if (mutationType == 3) mutationSuccessful = AddConnectionMutation(settings);
            else if (mutationType == 4) mutationSuccessful = RemoveConnectionMutation();
            else if (mutationType == 5) mutationSuccessful = ChangeConnectionOriginMutation();
            else if (mutationType == 6) mutationSuccessful = ChangeConnectionTargetMutation();
            else if (mutationType == 7) mutationSuccessful = ChangeConnectionWeightMutation();

            return mutationSuccessful;
        }

        public bool AddNodeMutation(SoupSettings settings)  // ランダムなノードを追加する突然変異
        {
            if (Nodes.Count < settings.AnimalBrainMaximumNodeCount)
            {
                if (InputNodeIndexes.Length > 0 && OutputNodeIndexes.Length > 0)
                {
                    Random rand = new Random();

                    BrainNode targetNode = new BrainNode();
                    int targetNodeIndex = Nodes.Count;

                    int nodeType = WeightedSelector(settings.AnimalMutationNodeTypeWeights);
                    if (nodeType == 0) targetNode.Function = (BrainNodeFunction)rand.Next(IndexOfFirstInputTypeBrainNodeFunction, IndexOfLastInputTypeBrainNodeFunction + 1);
                    if (nodeType == 1) targetNode.Function = (BrainNodeFunction)rand.Next(IndexOfFirstHiddenTypeBrainNodeFunction, IndexOfLastHiddenTypeBrainNodeFunction + 1);
                    if (nodeType == 2) targetNode.Function = (BrainNodeFunction)rand.Next(IndexOfFirstOutputTypeBrainNodeFunction, IndexOfLastOutputTypeBrainNodeFunction + 1);

                    if (nodeType == 0 || nodeType == 1)
                    {
                        int[] outputAndHiddenNodeIndexes = OutputAndHiddenNodeIndexes;
                        if (outputAndHiddenNodeIndexes.Length == 0) return false;

                        BrainNodeConnection targetConnection = new BrainNodeConnection() { OriginIndex = targetNodeIndex, TargetIndex = outputAndHiddenNodeIndexes[rand.Next(0, outputAndHiddenNodeIndexes.Length)], Weight = rand.NextDouble() * 4d - 2d };

                        if (targetConnection.OriginIndex != targetConnection.TargetIndex) Connections.Add(targetConnection);
                        else return false;
                    }
                    if (nodeType == 1 || nodeType == 2)
                    {
                        int[] inputAndHiddenNodeIndexes = InputAndHiddenNodeIndexes;
                        if (inputAndHiddenNodeIndexes.Length == 0) return false;

                        BrainNodeConnection targetConnection = new BrainNodeConnection() { OriginIndex = inputAndHiddenNodeIndexes[rand.Next(0, inputAndHiddenNodeIndexes.Length)], TargetIndex = targetNodeIndex, Weight = rand.NextDouble() * 4d - 2d };

                        if (targetConnection.OriginIndex != targetConnection.TargetIndex) Connections.Add(targetConnection);
                        else return false;
                    }

                    Nodes.Add(targetNode);
                    return true;
                }
                else return false;
            }
            else
            {
                // ノード数が上限に達している場合はRemoveNodeMutationかChangeNodeTypeMutationにフォールバックする
                int fallbackMutationType = WeightedSelector(settings.AnimalMutationTypeWeightsAddNodeMutationFallback);

                if (fallbackMutationType == 0) return RemoveNodeMutation();
                if (fallbackMutationType == 1) return ChangeNodeTypeMutation(settings);
            }

            return false;
        }
        public bool RemoveNodeMutation()    // ランダムなノードを削除する突然変異
        {
            if (Nodes.Count > 0)
            {
                Random rand = new Random();

                int targetNodeIndex = rand.Next(0, Nodes.Count);

                Nodes.RemoveAt(targetNodeIndex);
                for (int i = Connections.Count - 1; i >= 0; i--)
                {
                    if (Connections[i].OriginIndex == targetNodeIndex || Connections[i].TargetIndex == targetNodeIndex) Connections.RemoveAt(i);
                    else
                    {
                        if (Connections[i].OriginIndex > targetNodeIndex) Connections[i].OriginIndex--;
                        if (Connections[i].TargetIndex > targetNodeIndex) Connections[i].TargetIndex--;
                    }
                }

                return true;
            }
            
            return false;
        }
        public bool ChangeNodeTypeMutation(SoupSettings settings)   // ランダムなノードの種類を変更する突然変異
        {
            if (Nodes.Count > 0)
            {
                Random rand = new Random();

                int targetNodeIndex = rand.Next(0, Nodes.Count);
                BrainNode targetNode = Nodes[targetNodeIndex];

                int targetNodePrevNodeType = -1;
                if (targetNode.IsInput) targetNodePrevNodeType = 0;
                if (targetNode.IsHidden) targetNodePrevNodeType = 1;
                if (targetNode.IsOutput) targetNodePrevNodeType = 2;

                int nodeType = WeightedSelector(settings.AnimalMutationNodeTypeWeights);

                // ノードの種類を変更する
                if (nodeType == 0) targetNode.Function = (BrainNodeFunction)rand.Next(IndexOfFirstInputTypeBrainNodeFunction, IndexOfLastInputTypeBrainNodeFunction + 1);
                if (nodeType == 1) targetNode.Function = (BrainNodeFunction)rand.Next(IndexOfFirstHiddenTypeBrainNodeFunction, IndexOfLastHiddenTypeBrainNodeFunction + 1);
                if (nodeType == 2) targetNode.Function = (BrainNodeFunction)rand.Next(IndexOfFirstOutputTypeBrainNodeFunction, IndexOfLastOutputTypeBrainNodeFunction + 1);

                // 入力ノードから隠れノードに変異した際の処理
                if (targetNodePrevNodeType == 0 && nodeType == 1)
                {
                    if (InputAndHiddenNodeIndexes.Length > 0 && Connections.Count < Nodes.Count * settings.AnimalBrainMaximumConnectionCountPerNode)
                    {
                        int[] inputAndHiddenNodeIndexes = InputAndHiddenNodeIndexes;
                        Connections.Add(new BrainNodeConnection() { OriginIndex = inputAndHiddenNodeIndexes[rand.Next(0, inputAndHiddenNodeIndexes.Length)], TargetIndex = targetNodeIndex, Weight = rand.NextDouble() * 4d - 2d });
                    }
                    else if (InputAndHiddenNodeIndexes.Length == 0)     // 突然変異によって入力ノードが存在しなくなる場合
                    {
                        targetNode.Function = (BrainNodeFunction)rand.Next((int)BrainNodeFunction.Input_Bias, (int)BrainNodeFunction.Input_PheromoneBlueGradAngle + 1);
                    }
                    else if (Connections.Count >= Nodes.Count * settings.AnimalBrainMaximumConnectionCountPerNode)  // 接続数がすでに上限に達している場合
                    {
                        nodeType = WeightedSelector(settings.AnimalMutationNodeTypeWeightsWithoutHidden);
                        if (nodeType == 0) targetNode.Function = (BrainNodeFunction)rand.Next(IndexOfFirstInputTypeBrainNodeFunction, IndexOfLastInputTypeBrainNodeFunction + 1);
                        if (nodeType == 1) targetNode.Function = (BrainNodeFunction)rand.Next(IndexOfFirstOutputTypeBrainNodeFunction, IndexOfLastOutputTypeBrainNodeFunction + 1);
                    }
                }
                // 出力ノードから隠れノードに変異した際の処理
                if (targetNodePrevNodeType == 2 && nodeType == 1)
                {
                    if (OutputAndHiddenNodeIndexes.Length > 0 && Connections.Count < Nodes.Count * settings.AnimalBrainMaximumConnectionCountPerNode)
                    {
                        int[] outputAndHiddenNodeIndexes = OutputAndHiddenNodeIndexes;
                        Connections.Add(new BrainNodeConnection() { OriginIndex = targetNodeIndex, TargetIndex = outputAndHiddenNodeIndexes[rand.Next(0, outputAndHiddenNodeIndexes.Length)], Weight = rand.NextDouble() * 4d - 2d });
                    }
                    else if (OutputAndHiddenNodeIndexes.Length == 0)    // 突然変異によって出力ノードが存在しなくなる場合
                    {
                        targetNode.Function = (BrainNodeFunction)rand.Next((int)BrainNodeFunction.Output_Acceleration, (int)BrainNodeFunction.Output_PheromoneBlueProduction + 1);
                    }
                    else if (Connections.Count >= Nodes.Count * settings.AnimalBrainMaximumConnectionCountPerNode)  // 接続数がすでに上限に達している場合
                    {
                        nodeType = WeightedSelector(settings.AnimalMutationNodeTypeWeightsWithoutHidden);
                        if (nodeType == 0) targetNode.Function = (BrainNodeFunction)rand.Next(IndexOfFirstInputTypeBrainNodeFunction, IndexOfLastInputTypeBrainNodeFunction + 1);
                        if (nodeType == 1) targetNode.Function = (BrainNodeFunction)rand.Next(IndexOfFirstOutputTypeBrainNodeFunction, IndexOfLastOutputTypeBrainNodeFunction + 1);
                    }
                }

                // 入力ノード以外から入力ノードに変異した際の処理
                if (targetNodePrevNodeType != 0 && nodeType == 0)
                {
                    for (int i = Connections.Count - 1; i >= 0; i--)
                    {
                        if (Connections[i].TargetIndex == targetNodeIndex) Connections.RemoveAt(i);
                    }
                    if (targetNodePrevNodeType == 2)    // 元のノードが出力ノードだった場合の処理
                    {
                        int[] outputAndHiddenNodeIndexes = OutputAndHiddenNodeIndexes;
                        Connections.Add(new BrainNodeConnection() { OriginIndex = targetNodeIndex, TargetIndex = outputAndHiddenNodeIndexes[rand.Next(0, outputAndHiddenNodeIndexes.Length)], Weight = rand.NextDouble() * 4d - 2d });
                    }
                }
                // 出力ノード以外から出力ノードに変異した際の処理
                if (targetNodePrevNodeType != 2 && nodeType == 2)
                {
                    for (int i = Connections.Count - 1; i >= 0; i--)
                    {
                        if (Connections[i].OriginIndex == targetNodeIndex) Connections.RemoveAt(i);
                    }
                    if (targetNodePrevNodeType == 0)    // 元のノードが入力ノードだった場合の処理
                    {
                        int[] inputAndHiddenNodeIndexes = InputAndHiddenNodeIndexes;
                        Connections.Add(new BrainNodeConnection() { OriginIndex = inputAndHiddenNodeIndexes[rand.Next(0, inputAndHiddenNodeIndexes.Length)], TargetIndex = targetNodeIndex, Weight = rand.NextDouble() * 4d - 2d });
                    }
                }

                return true;
            }
            
            return false;
        }
        public bool AddConnectionMutation(SoupSettings settings)    // ランダムな接続を追加する突然変異
        {
            if (Nodes.Count >= 2)
            {
                if (Connections.Count < Nodes.Count * settings.AnimalBrainMaximumConnectionCountPerNode)
                {
                    Random rand = new Random();

                    int[] inputAndHiddenNodeIndexes = InputAndHiddenNodeIndexes;
                    int[] outputAndHiddenNodeIndexes = OutputAndHiddenNodeIndexes;

                    int connectionOriginIndex = inputAndHiddenNodeIndexes[rand.Next(0, inputAndHiddenNodeIndexes.Length)];
                    int connectionTargetIndex = outputAndHiddenNodeIndexes[rand.Next(0, outputAndHiddenNodeIndexes.Length)];

                    if (connectionOriginIndex != connectionTargetIndex && !ContainConnection(connectionOriginIndex, connectionTargetIndex))
                    {
                        Connections.Add(new BrainNodeConnection() { OriginIndex = connectionOriginIndex, TargetIndex = connectionTargetIndex, Weight = rand.NextDouble() * 4d - 2d });
                        return true;
                    }
                    else return false;
                }
                else
                {
                    // 接続数が上限に達している場合はRemoveConnectionMutation、ChangeConnectionOriginMutation、ChangeConnectionTargetMutation、ChangeConnectionWeightMutationのいずれかにフォールバックする
                    int fallbackMutationType = WeightedSelector(settings.AnimalMutationTypeWeightsAddConnectionMutationFallback);

                    if (fallbackMutationType == 0) return RemoveConnectionMutation();
                    if (fallbackMutationType == 1) return ChangeConnectionOriginMutation();
                    if (fallbackMutationType == 2) return ChangeConnectionTargetMutation();
                    if (fallbackMutationType == 3) return ChangeConnectionWeightMutation();
                }
            }
            
            return false;
        }
        public bool RemoveConnectionMutation()  // ランダムな接続を削除する突然変異
        {
            if (Connections.Count > 0)
            {
                Random rand = new Random();

                int targetConnectionIndex = rand.Next(0, Connections.Count);
                Connections.RemoveAt(targetConnectionIndex);

                return true;
            }
            
            return false;
        }
        public bool ChangeConnectionOriginMutation()    // ランダムな接続のOriginIndexを変更する突然変異
        {
            if (Connections.Count > 0)
            {
                Random rand = new Random();

                int targetConnectionIndex = rand.Next(0, Connections.Count);
                BrainNodeConnection targetConnection = Connections[targetConnectionIndex];

                int[] inputAndHiddenNodeIndexes = InputAndHiddenNodeIndexes;
                int targetConnectionNewOriginIndex = inputAndHiddenNodeIndexes[rand.Next(0, inputAndHiddenNodeIndexes.Length)];

                if (targetConnection.OriginIndex != targetConnectionNewOriginIndex && !ContainConnection(targetConnectionNewOriginIndex, targetConnection.TargetIndex))
                {
                    targetConnection.OriginIndex = targetConnectionNewOriginIndex;
                    return true;
                }
                else return false;
            }
            
            return false;
        }
        public bool ChangeConnectionTargetMutation()    // ランダムな接続のTargetIndexを変更する突然変異
        {
            if (Connections.Count > 0)
            {
                Random rand = new Random();

                int targetConnectionIndex = rand.Next(0, Connections.Count);
                BrainNodeConnection targetConnection = Connections[targetConnectionIndex];

                int[] outputAndHiddenNodeIndexes = OutputAndHiddenNodeIndexes;
                int targetConnectionNewTargetIndex = outputAndHiddenNodeIndexes[rand.Next(0, outputAndHiddenNodeIndexes.Length)];

                if (targetConnection.TargetIndex != targetConnectionNewTargetIndex && !ContainConnection(targetConnection.OriginIndex, targetConnectionNewTargetIndex))
                {
                    targetConnection.TargetIndex = targetConnectionNewTargetIndex;
                    return true;
                }
                else return false;
            }
            
            return false;
        }
        public bool ChangeConnectionWeightMutation()    // ランダムな接続の重みを変更する突然変異
        {
            if (Connections.Count > 0)
            {
                Random rand = new Random();

                int targetConnectionIndex = rand.Next(0, Connections.Count);
                BrainNodeConnection targetConnection = Connections[targetConnectionIndex];

                targetConnection.Weight = rand.NextDouble() * 4d - 2d;

                return true;
            }
            
            return false;
        }

        // 無効なノードを削除する処理
        public void RemoveInvalidNode()
        {
            List<int>[] nodeIncomingConnections = new List<int>[Nodes.Count];
            List<int>[] nodeOutgoingConnections = new List<int>[Nodes.Count];
            List<int> invalidNodeIndexes = new List<int>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                nodeIncomingConnections[i] = new List<int>();
                nodeOutgoingConnections[i] = new List<int>();
            }
            for (int i = 0; i < Connections.Count; i++)
            {
                nodeIncomingConnections[Connections[i].TargetIndex].Add(Connections[i].OriginIndex);
                nodeOutgoingConnections[Connections[i].OriginIndex].Add(Connections[i].TargetIndex);
            }

            List<int> connectedFromInputNodeIndexes = new List<int>();
            List<int> connectedFromOutputNodeIndexes = new List<int>();

            // 入力ノード及び入力ノードから接続を順に(OriginからTargetに向かって)たどってたどり着くルートが存在するノードを列挙する
            List<int> exploreNodeIndexes = InputNodeIndexes.ToList<int>();
            while (exploreNodeIndexes.Count > 0)
            {
                List<int> nextStepExploreNodeIndexes = new List<int>();

                for (int i = 0; i < exploreNodeIndexes.Count; i++)
                {
                    connectedFromInputNodeIndexes.Add(exploreNodeIndexes[i]);

                    for (int j = 0; j < nodeOutgoingConnections[exploreNodeIndexes[i]].Count; j++)
                    {
                        int targetConnectionTargetIndex = nodeOutgoingConnections[exploreNodeIndexes[i]][j];

                        if (!nextStepExploreNodeIndexes.Contains(targetConnectionTargetIndex) && !connectedFromInputNodeIndexes.Contains(targetConnectionTargetIndex)) nextStepExploreNodeIndexes.Add(targetConnectionTargetIndex);
                    }
                }

                exploreNodeIndexes = nextStepExploreNodeIndexes;
            }

            // 出力ノード及び出力ノードから接続を逆に(TargetからOriginに向かって)たどってたどり着くルートが存在するノードを列挙する
            exploreNodeIndexes = OutputNodeIndexes.ToList<int>();
            while (exploreNodeIndexes.Count > 0)
            {
                List<int> nextStepExploreNodeIndexes = new List<int>();

                for (int i = 0; i < exploreNodeIndexes.Count; i++)
                {
                    connectedFromOutputNodeIndexes.Add(exploreNodeIndexes[i]);

                    for (int j = 0; j < nodeIncomingConnections[exploreNodeIndexes[i]].Count; j++)
                    {
                        int targetConnectionOriginIndex = nodeIncomingConnections[exploreNodeIndexes[i]][j];

                        if (!nextStepExploreNodeIndexes.Contains(targetConnectionOriginIndex) && !connectedFromOutputNodeIndexes.Contains(targetConnectionOriginIndex)) nextStepExploreNodeIndexes.Add(targetConnectionOriginIndex);
                    }
                }

                exploreNodeIndexes = nextStepExploreNodeIndexes;
            }

            // 無効なノード(入力ノードから接続を順にたどって到達するルートか出力ノードから接続を逆にたどって到達するルートのいずれかまたは両方が存在しないノード)を列挙する
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (!(connectedFromInputNodeIndexes.Contains(i) && connectedFromOutputNodeIndexes.Contains(i))) invalidNodeIndexes.Add(i);
            }
            
            // 無効なノードをすべて削除する
            for (int i = invalidNodeIndexes.Count - 1; i >= 0; i--)
            {
                Nodes.RemoveAt(invalidNodeIndexes[i]);

                for (int j = Connections.Count - 1; j >= 0; j--)
                {
                    if (Connections[j].OriginIndex == invalidNodeIndexes[i] || Connections[j].TargetIndex == invalidNodeIndexes[i]) Connections.RemoveAt(j);
                    else
                    {
                        if (Connections[j].OriginIndex > invalidNodeIndexes[i]) Connections[j].OriginIndex--;
                        if (Connections[j].TargetIndex > invalidNodeIndexes[i]) Connections[j].TargetIndex--;
                    }
                }
            }
        }

        // 重みづけを考慮して複数の選択肢の中からランダムに一つ選ぶためのメソッド
        private int WeightedSelector(double[] weight)
        {
            double totalWeight = 0d;
            for (int i = 0; i < weight.Length; i++) totalWeight += weight[i];

            double weightedSelectedIndex = new Random().NextDouble() * totalWeight;

            for (int i = 0; i < weight.Length - 1; i++)
            {
                if (weightedSelectedIndex < weight[i]) return i;
                else weightedSelectedIndex -= weight[i];
            }
            return weight.Length - 1;
        }

        // 指定されたOriginとTargetを持つ接続が存在するか調べるメソッド
        private bool ContainConnection(int originIndex, int targetIndex)
        {
            for (int i = 0; i < Connections.Count; i++)
            {
                if (Connections[i].OriginIndex == originIndex && Connections[i].TargetIndex == targetIndex) return true;
            }

            return false;
        }
        
        // 指定されたBrainNodeFunctionを持つノードが存在するか調べるメソッド
        public bool ContainNodeFunction(BrainNodeFunction function)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Function == function) return true;
            }

            return false;
        }
        public bool ContainNodeFunction(BrainNodeFunction[] functions)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (functions.Contains(Nodes[i].Function)) return true;
            }

            return false;
        }

        // 脳のニューラルネットの更新処理
        public void UpdateBrain(BrainInput brainInput)
        {
            Input = brainInput;
            Input.PrevStepOutput = Output;

            BrainOutput brainOutput = new BrainOutput();

            for (int i = 0; i < Nodes.Count; i++) Nodes[i].ApplyBrainInput(this, Input);
            for (int i = 0; i < Nodes.Count; i++) Nodes[i].CalculateNodeOutput();
            for (int i = 0; i < Connections.Count; i++) Connections[i].UpdateConnection(Nodes);
            for (int i = 0; i < Nodes.Count; i++) Nodes[i].ApplyBrainOutput(ref brainOutput);

            Output = brainOutput;
        }
    }
}
