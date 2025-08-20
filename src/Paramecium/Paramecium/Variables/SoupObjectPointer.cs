using Paramecium.Engine;

namespace Paramecium.Variables
{
    // スープ上の特定のオブジェクトを指定するためのクラス
    public class SoupObjectPointer
    {
        public SoupObjectType ObjectType;   // オブジェクトの種類
        public int ObjectIndex;             // オブジェクトのインデックス
        public long ObjectId;               // オブジェクトのID (指定しない場合は-1、またタイルはIDを持たないためIDの指定は意味を持たない)

        public SoupObjectPointer()
        {
            ObjectType = SoupObjectType.None;
            ObjectIndex = -1;
            ObjectId = -1;
        }

        public SoupObjectPointer(SoupObjectType objectType, int objectIndex, long objectId)
        {
            ObjectType = objectType;
            ObjectIndex = objectIndex;
            ObjectId = objectId;
        }

        // 指定されたデータをもとにスープ内のオブジェクトを取得する (該当するオブジェクトが存在しない場合はnullを返す)
        public object? GetSoupObject()
        {
            Soup? soup = Globals.Soup;

            if (soup is not null)
            {
                if (ObjectType == SoupObjectType.None) return null;
                else if (ObjectType == SoupObjectType.Tile)
                {
                    if (ObjectIndex >= 0 && ObjectIndex < soup.Tiles.Length) return soup.Tiles[ObjectIndex];
                    else return null;
                }
                else if (ObjectType == SoupObjectType.Plant)
                {
                    try
                    {
                        if (ObjectIndex >= 0 && ObjectIndex < soup.Plants.Count)
                        {
                            Plant? targetPlant = soup.Plants[ObjectIndex];
                            if (targetPlant is not null)
                            {
                                if (ObjectId == -1 || ObjectId == targetPlant.Id)
                                {
                                    return soup.Plants[ObjectIndex];
                                }
                                else return null;
                            }
                            else return null;
                        }
                        else return null;
                    }
                    catch { return null; }
                }
                else if (ObjectType == SoupObjectType.Animal)
                {
                    try
                    {
                        if (ObjectIndex >= 0 && ObjectIndex < soup.Animals.Count)
                        {
                            Animal? targetAnimal = soup.Animals[ObjectIndex];
                            if (targetAnimal is not null)
                            {
                                if (ObjectId == -1 || ObjectId == targetAnimal.Id)
                                {
                                    return soup.Animals[ObjectIndex];
                                }
                                else return null;
                            }
                            else return null;
                        }
                        else return null;
                    }
                    catch { return null; }
                }
            }
            
            return null;
        }
    }
}
