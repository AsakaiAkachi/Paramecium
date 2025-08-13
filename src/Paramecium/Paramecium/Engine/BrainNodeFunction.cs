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
        Input_Element,                          // 現在のエレメント量
        Input_ReproductionProgress,             // 現在の繁殖の進捗
        Input_Age,                              // 現在の年齢

        Input_Ate,                              // 植物を食べることに成功したかどうか
        Input_Attacked,                         // 攻撃を受けてからしばらくの間のみ0より大きい値になる
        Input_AttackdAngle,                     // 最後に攻撃を受けたときの攻撃方向の相対角度
        Input_AttackSuccessful,                 // 最後に行った攻撃が成功したかどうか

        Input_WallWAvgAngle,                    // 自身から見た壁の加重平均角度
        Input_WallWAvgProximity,                // 自身から見た壁までの加重平均距離(近いほど値が大きくなる)
        Input_WallWAvgDistance,                 // 自身から見た壁までの加重平均距離(遠いほど値が大きくなる)
        Input_PlantWAvgAngle,                   // 植物
        Input_PlantWAvgProximity,
        Input_PlantWAvgDistance,
        Input_AnimalWAvgAngle,                  // 動物
        Input_AnimalWAvgProximity,
        Input_AnimalWAvgDistance,
        Input_AnimalWAvgSpeciesSigDiff,         // 自身から見た視界内にいる動物の種族値の自身の種族値との差の加重平均値(自身と同じ種族値の場合は差は0、そうでなければ差は1として扱う)

        Input_PheromoneRedConcentration,        // 赤フェロモンの濃度
        Input_PheromoneRedGradAngle,            // 赤フェロモンの濃度勾配角度×濃度
        Input_PheromoneGreenConcentration,      // 緑フェロモン
        Input_PheromoneGreenGradAngle,
        Input_PheromoneBlueConcentration,       // 青フェロモン
        Input_PheromoneBlueGradAngle,


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

        Output_Reproduction,                    // 繁殖の進捗を進めるために消費するエレメント量

        Output_PheromoneRedProduction,          // フェロモンの生成
        Output_PheromoneGreenProduction,
        Output_PheromoneBlueProduction
    }
}
