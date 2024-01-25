# Paramecium

<!-- Parameciumは、「スープ」と呼ばれる連続的な二次元仮想空間をシミュレートする人工生命/生態系シミュレーターです。 -->
Paramecium is an artificial life/ecosystem simulator that simulates a continuous two-dimensional virtual space called a "soup".

<!-- スープには多数の相互作用する仮想生物が含まれており、仮想生物の生存、繁栄、進化、絶滅などをシミュレートします。 -->
The soup contains numerous interacting virtual organisms, simulating the survival, flourishing, evolution, and extinction of virtual organisms.

<!-- # 仮想生物 -->
# Virtual Organism

<!-- スープ内に生息する仮想生物は、大きく植物と動物に分けられ、二つは全く異なる性質を持っています。<br> -->
The virtual organisms that inhabit the soup are largely divided into plants and animals, and the two have very different properties.<br>

<!-- 植物は土壌からバイオマスを収集して増殖し、動物は植物や他の動物からバイオマスを得て生存・増殖します。<br> -->
Plants collect biomass from the soil and multiply, while animals survive and multiply by obtaining biomass from plants and other animals.<br>
<!-- また、動物が消費したバイオマスは動物がいる場所の土壌に還元されるため、バイオマスは全体としては総量が変化することなく循環します。 -->
In addition, the biomass consumed by the animals is returned to the soil where the animals are located, so the biomass as a whole circulates without any change in total amount.

<!-- ### 植物 -->
### Plant

<!-- 植物は周囲の土壌に含まれるバイオマスを消費して成長することが可能です。<br> -->
Plants can grow by consuming the biomass in the surrounding soil.<br>
<!-- (バイオマスを含む土壌は含むバイオマスの量に応じて色が変わり、バイオマスの量が多いほど緑っぽくなります)<br> -->
(Soil containing biomass changes color depending on the amount of biomass it contains; the greater the amount of biomass, the greener the soil.)<br>
<!-- 土壌から十分な量のバイオマスを集めた植物は、いくつかの小さい断片に分裂することで増殖します。<br> -->
Plants that have collected enough biomass from the soil multiply by splitting into several smaller fragments.<br>
<!-- 植物は周囲の土壌にバイオマスがなくても生存できますが、その場合成長や増殖することはできません。 -->
Plants can survive without biomass in the surrounding soil, but they cannot grow or multiply in that case.

<!-- ### 動物 -->
### Animal

<!-- 動物はスープ内に生息するもうもう一種類の仮想生物です。<br> -->
Animals are another type of virtual organism that lives within the soup.<br>
<!-- 動物は生存するために常にバイオマスを消費し、バイオマスが尽きるとすぐに死んでしまうため、生存と増殖のために食料を摂取し続ける必要があります。 -->
Animals constantly consume biomass to survive and die as soon as they run out of biomass, so they must continue to ingest food to survive and multiply.

<!-- 動物はいくつかパラメーターを持っており、そのうち食性のパラメーターによって大きく草食、雑食、肉食に分けられます。<br> -->
Animals have several parameters, of which they are largely divided into herbivores, omnivores, and carnivores according to their dietary parameters.<br>
<!-- 表示モードを食性表示にした場合、草食は緑色、雑食はオリーブ色、肉食は赤色で表示されます。 -->
When the display mode is set to diet display, herbivores are displayed in green, omnivores in olive, and carnivores in red.

<!-- 草食動物は植物からバイオマスを直接得ることができますが、肉食動物は被食者となる動物と戦闘をして勝利することで初めて被食者が持っていたバイオマスを得ることができます。<br> -->
Herbivores can obtain biomass directly from plants, but carnivores can only obtain biomass held by their prey by fighting and winning a battle with the prey animal.<br>
<!-- また、雑食動物は植物と動物のどちらを食べてもバイオマスを得られますが、効率はあまりよくありません。 -->
Also, omnivores can obtain biomass by eating both plants and animals, but less efficiently.

<!-- 動物は自らの食性に合うものを食べると食べたものが含んでいるバイオマスの大部分またはすべてを自身のバイオマスにできます。<br> -->
When an animal eats something that fits its diet, it can convert most or all of the biomass contained in the food it eats into its own biomass.<br>
<!-- しかし、自らの食性に合わないものを食べた場合はバイオマスの一部または大部分が吸収できずに失われます。<br> -->
However, if they eat something that does not match their own dietary characteristics, some or most of the biomass will be lost without being absorbed.<br>
<!-- この時失われたバイオマスは消滅するわけではなく、その動物がいる場所の土壌のバイオマスに加算されます。 -->
The biomass lost at this time does not disappear, but is added to the biomass of the soil where the animal is located.

<!-- 動物は増殖の際にパラメーターが少し変化して遺伝し、また一定の確率(初期設定では10%)でパラメーターが大きく変化する突然変異を起こして別種に変化します。 -->
Animals are inherited with slight changes in parameters during propagation, and with a certain probability (10% by default) they will mutate into a different species with a large change in parameters.

<!-- # 操作方法 -->
# Key Binding
<!-- - 左クリック:生物を選択する -->
<!-- - Shift+左ドラッグ:選択されている生物をカーソルの位置に移動させる -->
<!-- - Ctrl+左ドラッグ:カーソルの位置に壁を生成 -->
<!-- - 右ドラッグ:視点を移動する -->
<!-- - Ctrl+右ドラッグ:カーソルの位置の壁を削除 -->
<!-- - マウスホイール:拡大/縮小 -->
<!-- - Spaceキー:シミュレーションの一時停止及び再開 -->
<!-- - ピリオドキー:シミュレーションのスレッド数を1増やす -->
<!-- - コンマキー:シミュレーションのスレッド数を1減らす -->
<!-- - 疑問符キー:シミュレーションを1ステップだけ進める -->
<!-- - Rキー:選択された生物に視点を追従させる/追従を解除する -->
<!-- - Cキー:視点をリセットする -->
<!-- - Fキー:フルスクリーン表示の切り替え -->
<!-- - 1キー:描画モードを通常モードにする -->
<!-- - 2キー:描画モードを食性表示(緑色=草食、赤色=肉食)に切り替える -->
- Left click: select organisms
- Shift+left drag: move the selected organism to the cursor position
- Ctrl+left drag: create a wall at the cursor position
- Right drag: Move the viewpoint
- Ctrl+right drag: Delete wall at cursor position
- Mouse wheel: Zoom in/out
- Spacebar: Pause and resume simulation
- Period key: Increase the number of threads in the simulation by 1
- Comma key: Decrease the number of threads in the simulation by 1
- Question mark key: Advance the simulation by one step
- R key: follow/unfollow the viewpoint to/from the selected organism
- C key: Reset the viewpoint
- F key: Toggle full screen view
- 1 key: Switch to normal drawing mode
- 2 key: Toggle drawing mode (green = herbivorous, red = carnivorous)

<!-- # その他 -->
# Others
<!-- - このプロジェクトの製作者は英語圏の人ではなく(日本人です)、英語を読み書きできないため、READNE.mdを書くのにはDeelL翻訳などを使用しています。そのため、文章が一部間違っている可能性があります。 -->
The creator of this project is not an English speaker (he/she is Japanese) and cannot read or write English, so he/she uses DeelL translations, etc. to write READNE.md. Therefore, some sentences may be incorrect.