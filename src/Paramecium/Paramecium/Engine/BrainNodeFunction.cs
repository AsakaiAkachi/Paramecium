namespace Paramecium.Engine
{
    public enum BrainNodeFunction
    {
        // その他
        NonOperation,                           // 何もしない


        // 入力
        Input_Bias,                             // バイアス(常に1)

        Input_Velocity,                         // 現在の速度
        Input_AngularVelocity,                  // 現在の角速度
        Input_Satiety,                          // 現在のエレメント量
        Input_Attacked,                         // 攻撃を受けてからしばらくの間のみ0より大きい値になる

        Input_WallWAvgAngle,                    // 自身から見た壁の平均角度
        Input_WallWAvgProximity,                // 自身から見た壁までの平均距離(近いほど値が大きくなる)
        Input_WallWAvgDistance,                 // 自身から見た壁までの平均距離(遠いほど値が大きくなる)
        Input_PlantWAvgAngle,                   // 植物
        Input_PlantWAvgProximity,
        Input_PlantWAvgDistance,
        Input_AnimalWAvgAngle,                  // 動物
        Input_AnimalWAvgProximity,
        Input_AnimalWAvgDistance,
        Input_AnimalWAvgSpeciesSigDiff,

        Input_PheromoneRedConcentration,        // 赤フェロモンの濃度
        Input_PheromoneRedGradAngle,            // 赤フェロモンの濃度勾配角度×濃度
        Input_PheromoneGreenConcentration,      // 緑フェロモン
        Input_PheromoneGreenGradAngle,
        Input_PheromoneBlueConcentration,       // 青フェロモン
        Input_PheromoneBlueGradAngle,

        /**
        Input_Memory0,                          // メモリ
        Input_Memory1,
        Input_Memory2,
        Input_Memory3,
        Input_Memory4,
        Input_Memory5,
        Input_Memory6,
        Input_Memory7,

        Input_InheritedMemory0,                 // 継承メモリ
        Input_InheritedMemory1,
        Input_InheritedMemory2,
        Input_InheritedMemory3,
        Input_InheritedMemory4,
        Input_InheritedMemory5,
        Input_InheritedMemory6,
        Input_InheritedMemory7,
        **/


        // 隠れ層
        Hidden_ReLU,                            // y = max(0, x)
        Hidden_LimitedReLU,                     // y = max(0, min(1, x))
        Hidden_Step,                            // xが0より大きいならy = 1、そうでなければy = 0
        Hidden_Sigmoid,                         // y = tanh(x)
        Hidden_Identity,                        // y = x
        Hidden_Absolute,                        // y = abs(x)
        Hidden_Sine,                            // y = sin(x)
        Hidden_Cosine,                          // y = cos(x)
        Hidden_Tangent,                         // y = tan(x)
        Hidden_LimitedTangent,                  // y = max(-1, min(1, tan(x)))
        Hidden_Frac,                            // y = abs(x % 1)


        // 出力
        Output_Acceleration,                    // 加速
        Output_Rotation,                        // 回転
        Output_Eat,                             // 植物の摂食
        Output_Attack,                          // 動物への攻撃

        Output_PheromoneRedProduction,          // フェロモンの生成
        Output_PheromoneGreenProduction,
        Output_PheromoneBlueProduction,

        /**
        Output_Memory0,                         // メモリ
        Output_Memory1,
        Output_Memory2,
        Output_Memory3,
        Output_Memory4,
        Output_Memory5,
        Output_Memory6,
        Output_Memory7,

        Output_InheritedMemory0,                // 継承メモリ
        Output_InheritedMemory1,
        Output_InheritedMemory2,
        Output_InheritedMemory3,
        Output_InheritedMemory4,
        Output_InheritedMemory5,
        Output_InheritedMemory6,
        Output_InheritedMemory7,
        **/
    }
}
