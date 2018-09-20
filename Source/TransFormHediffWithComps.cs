using TragicTheCluckering;
using Verse;

namespace TragicTheCluckering
{
    public class TransFormHediffWithComps : HediffWithComps
    {
        private ThingDef trueForm;
        private bool didIt;

        public virtual ThingDef transformInto => Constants.TragicChicken;
        public virtual PawnKindDef transformIntoKind => Constants.TragicChickenKind;


//        public override void PostRemoved()
//        {
//            TransformPawn(trueForm);
//            base.PostRemoved();
//        }

        private int tick = 0;
        public override void Tick()
        {
            tick++;
            if(didIt || tick < 7) return;
            
            trueForm = pawn.def;
            TransformPawn(transformInto, transformIntoKind);

            didIt = true;
        }

        private void TransformPawn(ThingDef @into, PawnKindDef intoKind)
        {
            Log.Error("Performing Transform on " + pawn + " to a " + into);
            if (into == null || into == pawn.def) return;

            var map = pawn.Map;
            RegionListersUpdater.DeregisterInRegions(pawn, map);
            pawn.def = into;

            pawn.ChangeKind(intoKind);
            RegionListersUpdater.RegisterInRegions(pawn, map);

            map.mapPawns.UpdateRegistryForPawn(pawn);

            //save the pawn
            pawn.ExposeData();

            //magic to make image change 
            pawn.ageTracker.AgeBiologicalTicks = pawn.ageTracker.AgeBiologicalTicks - 1;
            //pawn.SpawnSetup(map, true);
        }

        public override bool ShouldRemove => didIt;
    }
}