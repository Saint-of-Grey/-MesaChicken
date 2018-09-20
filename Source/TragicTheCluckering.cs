using Verse;

namespace TragicTheCluckering
{
    public class TragicTheCluckering : Mod
    {
        public TragicTheCluckering(ModContentPack content) : base(content)
        {
        }
    }

    static class Constants
    {
        public static ThingDef TragicChicken = DefDatabase<ThingDef>.GetNamed("ChickenTragic");
        public static PawnKindDef TragicChickenKind = DefDatabase<PawnKindDef>.GetNamed("ChickenTragic");
    }
}