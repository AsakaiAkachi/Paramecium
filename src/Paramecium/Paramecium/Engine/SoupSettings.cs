using System.Text.Json.Serialization;

namespace Paramecium.Engine
{
    // スープの設定を保存する用のクラス
    public class SoupSettings
    {
        // スープの基本設定
        public int SoupSizeX { get; set; } = 256;   // スープの横方向の大きさ
        public int SoupSizeY { get; set; } = 256;   // スープの縦方向の大きさ
        [JsonIgnore]
        public int SoupArea { get { return SoupSizeX * SoupSizeY; } }   // スープの面積

        public bool SoupWallEnabled { get; set; } = true;       // 壁が有効であるかどうか
        public double SoupWallNoiseX { get; set; } = 0d;        // 壁生成用ノイズのオフセット (X,Y,Z)
        public double SoupWallNoiseY { get; set; } = 0d;
        public double SoupWallNoiseZ { get; set; } = 0d;
        public double SoupWallNoiseSamplingInterval { get; set; } = 0.03d;  // 壁生成用ノイズのサンプリング間隔
        public int SoupWallNoiseOctave { get; set; } = 4;           // 壁生成用ノイズのオクターブ
        public double SoupWallThickness { get; set; } = 0.01125d;   // 壁の厚さ

        public double SoupTotalElementAmount { get; set; } = 65536d;    // スープ全体のエレメント総量
        public double SoupElementFlowRate { get; set; } = 0.01d;        // タイル間でエレメントが移動する速度
        [JsonIgnore]
        public double SoupElementPerTile { get { return SoupTotalElementAmount / SoupArea; } }  // タイル当たりのエレメント量

        public double SoupMaximumEffectivePheromoneAmount { get; set; } = 1d;       // 有効なフェロモン濃度の上限値、これよりフェロモン濃度が高い場合はこのフェロモン濃度であるものとして扱う
        public double SoupMinimumEffectivePheromoneAmount { get; set; } = 0.0001d;  // 有効なフェロモン濃度の下限値、これよりフェロモン濃度が低い場合は0として扱う
        public double SoupPheromoneFlowRate { get; set; } = 0.4d;                   // タイル間でフェロモンが移動する速度
        public double SoupPheromoneDecayRate { get; set; } = 0.01d;                 // フェロモン量が減少する速度

        public double SoupDrag { get; set; } = 0.1d;                            // 速度に対して毎ステップかかる抵抗の大きさ
        public double SoupAngularVelocityDrag { get; set; } = 0.1d;             // 角速度に対して毎ステップかかる抵抗の大きさ
        public double SoupMaximumEffectiveVelocity { get; set; } = 0.1d;        // 有効な速度の上限値、これよりも速度が速い場合はこの速度であるものとして扱う
        public double SoupMaximumEffectiveAngularVelocity { get; set; } = 0.1d; // 有効な角速度の上限値、これよりも角速度が速い場合はこの角速度であるものとして扱う
        public double SoupRestitutionCoefficient { get; set; } = 0.1d;          // 反発係数


        // 植物
        public int PlantInitialPopulation { get; set; } = 4096;             // 植物の初期個体数

        public double PlantMaximumElementAmount { get; set; } = 4d;         // 植物の最大エレメント量、このエレメント量を超えると分裂する
        public double PlantElementCollectRate { get; set; } = 0.1d;         // 植物がエレメントを収集する速度
        public int PlantUnderAttackTime { get; set; } = 10;                 // 植物が攻撃を受けた際に「Under Attack」状態でいる時間、この間はその植物は一切成長しない

        public int PlantDivisionCountMin { get; set; } = 4;                 // 植物が分裂する際の分裂数の最小値
        public int PlantDivisionCountMax { get; set; } = 8;                 // 植物が分裂する際の分裂数の最大値
        public int PlantSpreadingTime { get; set; } = 10;                   // 植物が分裂した直後の抵抗が適用されない期間の長さ


        // 動物 (基本設定)
        public int AnimalInitialPopulation { get; set; } = 16;                      // 動物の初期個体数

        public double AnimalMaximumElementAmount { get; set; } = 16d;               // 動物が持てるエレメント量の上限値
        public double AnimalElementBaseCost { get; set; } = 0.004d;                 // 動物が生存するのに必要な基礎エレメント量(ステップ当たり)
        public double AnimalElementAccelerationCost { get; set; } = 0.008d;         // 動物が加速するのに必要な最大エレメント量
        public double AnimalElementRotationCost { get; set; } = 0.004d;             // 動物が回転するのに必要な最大エレメント量
        public double AnimalElementEatCost { get; set; } = 0.008d;                  // 動物が他の植物を食べるのに必要なエレメント量
        public double AnimalElementAttackCost { get; set; } = 0.024d;               // 動物が他の動物を攻撃するのに必要なエレメント量の基本値
        public double AnimalElementPheromoneProductionCost { get; set; } = 0.002d;  // 動物がフェロモンを生成するのに必要なエレメント量(色毎)

        public double AnimalPheromoneProductionRate { get; set; } = 0.1d;       // 動物がフェロモンを生成する速度

        public double AnimalMaximumAcceleration { get; set; } = 0.01d;          // 動物の最大加速度
        public double AnimalMaximumAngularAcceleration { get; set; } = 0.01d;   // 動物の最大角加速度

        public bool AnimalDisableSameSpeciesAttack { get; set; } = false;       // 同種間での攻撃を禁止するかどうか
        public int AnimalUnderAttackTime { get; set; } = 25;                    // 動物が攻撃された際に「Under Attack」状態でいる時間
        public double AnimalPlantIngestionRate { get; set; } = 0.04d;           // 動物が植物を攻撃した際にステップ毎に奪い取るエレメントの量
        public double AnimalAnimalIngestionRate { get; set; } = 0.32d;          // 動物が動物を攻撃した際にステップ毎に奪い取るエレメントの量

        public double AnimalReproductionCost { get; set; } = 16d;               // 動物が子孫を残すのに必要なエレメント量、動物が持てるエレメントの量はこれの2倍になる
        public double AnimalMaximumReproductionRate { get; set; } = 0.032d;     // 動物が繁殖の進捗を進める最大速度
        public int AnimalEggHatchingTime { get; set; } = 500;                   // 動物が卵から孵化するのにかかる時間
        public double AnimalEggRadiusRatio { get; set; } = 0.25d;               // ふ化した動物の大きさに対する卵の初期の大きさの比率

        public int AnimalLifespan { get; set; } = 10000;                        // 動物の寿命


        // 動物 (ニューラルネット)
        public int AnimalBrainMaximumNodeCount { get; set; } = 32;                  // 最大ノード数
        public int AnimalBrainMaximumConnectionCountPerNode { get; set; } = 2;      // 最大接続数(ノード1個毎)

        // 動物 (突然変異)
        public double AnimalMutationMutationRate { get; set; } = 0.1d;                      // 繁殖時の突然変異率
        public int AnimalMutationMaximumMutationCount { get; set; } = 8;                    // 最大突然変異数
        public double AnimalMutationMutationCountFactor { get; set; } = 0.5d;               // 突然変異数係数
        public bool AnimalMutationDisableSpeciesSigChangeByMutation { get; set; } = false;  // 突然変異による種族シグネチャの変化の無効化

        public double AnimalMutationMutationTypeAddNodeWeight { get; set; } = 1d;                       // 突然変異のタイプ毎の重み / AddNode (ランダムなノードと付随する接続の追加)
        public double AnimalMutationMutationTypeRemoveNodeWeight { get; set; } = 0.25d;                 // RemoveNode (ランダムなノードと関連する接続の削除)
        public double AnimalMutationMutationTypeChangeNodeTypeWeight { get; set; } = 0.5d;              // ChangeNodeType (ノードの種類の変更)
        public double AnimalMutationMutationTypeAddConnectionWeight { get; set; } = 1d;                 // AddConnection (ランダムな接続の追加)
        public double AnimalMutationMutationTypeRemoveConnectionWeight { get; set; } = 0.25d;           // RemoveConnection (ランダムな接続の削除)
        public double AnimalMutationMutationTypeChangeConnectionOriginWeight { get; set; } = 0.25d;     // ChangeConnectionOrigin (ランダムな接続の接続元の変更)
        public double AnimalMutationMutationTypeChangeConnectionTargetWeight { get; set; } = 0.25d;     // ChangeConnectionTarget (ランダムな接続の接続先の変更)
        public double AnimalMutationMutationTypeChangeConnectionWeightWeight { get; set; } = 1d;        // ChangeConnectionWeight (ランダムな接続の重みの変更)
        [JsonIgnore]
        public double[] AnimalMutationTypeWeights { get { return new double[] { AnimalMutationMutationTypeAddNodeWeight, AnimalMutationMutationTypeRemoveNodeWeight, AnimalMutationMutationTypeChangeNodeTypeWeight, AnimalMutationMutationTypeAddConnectionWeight, AnimalMutationMutationTypeRemoveConnectionWeight, AnimalMutationMutationTypeChangeConnectionOriginWeight, AnimalMutationMutationTypeChangeConnectionTargetWeight, AnimalMutationMutationTypeChangeConnectionWeightWeight }; } }
        public double[] AnimalMutationTypeWeightsAddNodeMutationFallback { get { return new double[] { AnimalMutationMutationTypeRemoveNodeWeight, AnimalMutationMutationTypeChangeNodeTypeWeight }; } }
        public double[] AnimalMutationTypeWeightsAddConnectionMutationFallback { get { return new double[] { AnimalMutationMutationTypeRemoveConnectionWeight, AnimalMutationMutationTypeChangeConnectionOriginWeight, AnimalMutationMutationTypeChangeConnectionTargetWeight, AnimalMutationMutationTypeChangeConnectionWeightWeight }; } }

        public double AnimalMutationNodeTypeInputWeight { get; set; } = 1d;             // AddNode突然変異とChangeNodeType突然変異でランダムなFunctionが選ばれる際のタイプ毎の重み / Input (入力層)
        public double AnimalMutationNodeTypeHiddenWeight { get; set; } = 1d;            // Hidden (隠れ層)
        public double AnimalMutationNodeTypeOutputWeight { get; set; } = 1d;            // Output (出力層)
        [JsonIgnore]
        public double[] AnimalMutationNodeTypeWeights { get { return new double[] { AnimalMutationNodeTypeInputWeight, AnimalMutationNodeTypeHiddenWeight, AnimalMutationNodeTypeOutputWeight }; } }
        [JsonIgnore]
        public double[] AnimalMutationNodeTypeWeightsWithoutHidden { get { return new double[] { AnimalMutationNodeTypeInputWeight, AnimalMutationNodeTypeOutputWeight }; } }
    }
}
