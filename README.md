# Paramecium

<!-- Parameciumは、連続的な二次元仮想空間(「スープ」と呼ばれます)をシミュレートする人工生命/生態系シミュレーターです。 -->
Paramecium is an artificial life/ecosystem simulator that simulates a continuous two-dimensional virtual space (called a “soup”).

<!-- スープには多数の相互作用する仮想生物が含まれており、それらの生存、繁栄、進化、絶滅などを何千世代にもわたってシミュレートできます。 -->
The soup contains numerous interacting virtual organisms, which can simulate their survival, flourishing, evolution, and extinction over thousands of generations.

## Virtual Organism

<!-- スープに含まれる仮想生物は「植物」と「動物」の二種類に分けられます。植物と動物は、両方に共通の特徴とそれぞれに固有の特性を持っています。 -->
The virtual organisms in the soup can be divided into two categories: plants and animals. Both plants and animals share some characteristics common to both and unique to each.

<!-- 植物も動物も、「エレメント」と呼ばれる栄養素を一定量蓄えることでのみ増殖することができます。また、スープ内で他の物体に押されたり、あるいは自ら加速することで移動することができます。 -->
Both plants and animals can only multiply by storing a certain amount of nutrients, called “Element”. They can also move by being pushed by other objects in the soup or by accelerating themselves.

### Plant

<!-- 植物は、Parameciumにおいて生産者の役割を担う仮想生物です。タイルに含まれているエレメントを収集することで成長し、一定量のエレメントを集めると複数の植物に分裂することで増殖します。また、エレメントが枯渇した場所でも生存できますが、その場合は成長することはできません。 -->
Plants are virtual organisms that play the role of producers in Paramecium. They grow by collecting elements contained in tiles and multiply by splitting into multiple plants when a certain amount of elements are collected. They can also survive where elements are depleted, in which case they cannot grow.

### Animal

<!-- 動物は、Parameciumにおいて消費者の役割を担う仮想生物です。他の植物や動物を攻撃することでエレメントを得ることができますが、タイルからエレメントを得ることはできません。また、常にエレメントを消費し続け、エレメントがなくなると死んでしまいます。逆に、十分な量のエレメントを集めることができれば二つに分裂することで増殖します。 -->
Animals are virtual creatures that play the role of consumers in Paramecium. They can gain elements by attacking other plants and animals, but they cannot gain elements from tiles. They also constantly consume elements and die when they run out. Conversely, if it collects enough elements, it will multiply by splitting in two. 

<!-- すべての動物は、自らの行動を決定するためのニューラルネットワーク(Parameciumでは単純に「脳」と呼ばれます)を持っていて、これは子孫に遺伝していきます。脳の構造は子孫に遺伝する際に変異を起こす可能性があり、これが進化の原動力となります。 -->
Every animal has a neural network (called simply “brain” in Paramecium) to determine its own behavior, which is inherited by its offspring. Brain structures can mutate as they are inherited by offspring, and this is what drives evolution.

## Element

<!-- エレメントは、Parameciumにおいてすべての生物が必要とする栄養素です。スープ全体でのエレメントの量は常に一定であり、増減することはありません。動物はエレメントを消費しますが、消費されたエレメントは消滅するのではなくタイルに放出され、植物が成長のためにそのエレメントを再び使えるようになります。また、エレメントは量が多いタイルから少ないタイルへとゆっくりと流れる性質があります。 -->
Elements are nutrients needed by all organisms in Paramecium. The amount of elements throughout the soup is always constant and does not increase or decrease. Animals consume elements, but the consumed elements are not extinguished but released into the tiles so that plants can use them again for growth. Also, the elements have a tendency to flow slowly from tiles with higher quantities to tiles with lower quantities.

## Key Binding

### Window
- `F1`: Show/Hide overlay
- `Crtl + F`: Enable/Disable full screen

### Simulation
- `Space`: Run/Pause Simulation
- `Period`: Increase the number of threads by 1
- `Comma`: Decrease the number of threads by 1
- `Question Mark`: Advance the simulation by one step

### Camera
- `Right Drag`: Move the camera
- `C`: Reset the camera position
- `Left Click`: Select an object
- `T`: Make the camera follow the selected object
- `Crtl + T`: Enables auto-tracking of the camera
- `Crtl + Left Click / Mouse Wheel Up`: Zoom in
- `Crtl + Right Click / Mouse Wheel Down`: Zoom out

### Edit
- `Shift + Left Drag`: Move selected objects
- `Alt + Left Drag`: Create wall
- `Alt + Right Drag`: Delete Wall
- `Crtl + D`: Open Object Inspector **(WARNING: FOR ADVANCED USER FEATURE)**

## Requirements

<!-- ParameciumはWindows 11で開発されているため、それ以外でのOSの動作は保証されません。 -->
Since Paramecium is developed on Windows 11, operation on other operating systems is not guaranteed. 

<!-- CPU及びGPUの最低要件はありませんが、大規模なシミュレーションを実行するには高性能なCPUが必要になる場合があります。 -->
There are no minimum CPU or GPU requirements, but high-performance CPUs may be needed to run large simulations.

<br>

---

<!-- - このプロジェクトの製作者は英語圏の人ではなく(日本人です)、英語を読み書きできないため、READNE.mdなどを書くのにはDeelL翻訳などを使用しています。そのため、文章が一部間違っている可能性があります。 -->
The creator of this project is not an English speaker (I'm Japanese) and cannot read or write English, so I use DeelL translation, etc. to write `README.md` and other documents. Therefore, some sentences may be incorrect.
