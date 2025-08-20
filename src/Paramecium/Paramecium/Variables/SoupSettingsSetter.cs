using Paramecium.Engine;

namespace Paramecium.Variables
{
    // FormMainとFormSoupSettingsの間でSoupSettingsをやり取りする用のクラス
    public class SoupSettingsSetter
    {
        public SoupSettings? SoupSettings;

        public SoupSettingsSetter(SoupSettings? soupSettings)
        {
            SoupSettings = soupSettings;
        }
    }
}
