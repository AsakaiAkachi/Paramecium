using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paramecium.Rendering
{
    [Flags]
    public enum OverlayToggles
    {
        AllOverlays = 1 << 0,
        SelectedObject = 1 << 1,
        AnimalBrainDiagram = 1 << 2,
        AnimalBrainInputOutput = 1 << 3,
        FullScreenOverlay = 1 << 4
    }
}
