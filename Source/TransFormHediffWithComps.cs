using System;
using RimWorld;
using RimWorld.Planet;
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
            if (Rand.Bool)
                TransformPawn(transformInto, transformIntoKind);
            else ExplosiveTransformation(transformInto, transformIntoKind);

        }

        private void ExplosiveTransformation(ThingDef thingDef, PawnKindDef pawnKindDef)
        {
            TransformPawn(transformInto, transformIntoKind);
            var chicken = DoBirthSpawn();

            if (chicken != null && !pawn.Dead)
            {
                //blood mess
                
                pawn.filth.GainFilth(ThingDefOf.Filth_CorpseBile);
                pawn.filth.GainFilth(ThingDefOf.Filth_Blood);
                pawn.filth.GainFilth(ThingDefOf.Filth_Ash);

                FilthMaker.MakeFilth(pawn.Position, pawn.Map, ThingDefOf.Filth_CorpseBile, 4);
                FilthMaker.MakeFilth(pawn.Position, pawn.Map, ThingDefOf.Filth_Blood, 20);
                FilthMaker.MakeFilth(pawn.Position, pawn.Map, ThingDefOf.Filth_Ash, 10);
                
                GenExplosion.DoExplosion(pawn.Position, pawn.Map, 4f,DamageDefOf.Smoke, chicken);
                
                HealthUtility.DamageUntilDowned(pawn, true);
            }


            didIt = true;
            
        }

        private Pawn DoBirthSpawn()
        {
            var mother = ((Hediff) this).pawn;
            PawnGenerationRequest request = new PawnGenerationRequest(transformIntoKind, mother.Faction, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true, false, 1f, false, true, true, false, false, false, false, (Predicate<Pawn>) null, (Predicate<Pawn>) null, new float?(), new float?(), new float?(), new Gender?(), new float?(), (string) null);
            Pawn pawn = (Pawn) null;
           
                pawn = PawnGenerator.GeneratePawn(request);
                if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, (Thing) mother))
                {
                    if (pawn.playerSettings != null && mother.playerSettings != null)
                        pawn.playerSettings.AreaRestriction = mother.playerSettings.AreaRestriction;
                    if (pawn.RaceProps.IsFlesh)
                    {
                        pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, pawn);
                    }
                }
                else
                    Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
                TaleRecorder.RecordTale(TaleDefOf.GaveBirth, (object) mother, (object) pawn);
            
            if (!pawn.Spawned)
                return pawn;
            FilthMaker.MakeFilth(mother.Position, mother.Map, ThingDefOf.Filth_AmnioticFluid, mother.LabelIndefinite(), 5);
            mother.caller?.DoCall();
            pawn.caller?.DoCall();
            return pawn;
        }


        private void TransformPawn(ThingDef @into, PawnKindDef intoKind)
        {
            Log.Message("Performing Transform on " + pawn + " to a " + into + "with a kind of "+ intoKind);
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
            
            
            didIt = true;
        }

        public override bool ShouldRemove => didIt;
    }
}