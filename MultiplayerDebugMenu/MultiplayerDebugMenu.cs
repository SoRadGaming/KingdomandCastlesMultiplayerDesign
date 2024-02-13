using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Assets.Code;
using Assets.Code.Jobs;
using PixelCrushers;
using Rewired;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace MultiplayerDebugMenu {
    public class MultiplayerDebugMenu : MonoBehaviour {
        private static KCModHelper helper;
        
        public static int offset = 5;
        
        private void Preload(KCModHelper helper) {
            MultiplayerDebugMenu.helper = helper;
            helper.Log("Hello World, I am at " + helper.modPath);

            var harmony = new Harmony("MultiplayerDebugMenu");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static Building GetBuildingClone(Building original)
        {
            for (int index = 0; index < Player.inst.Buildings.Count; ++index)
            {
                if (Player.inst.Buildings.data[index].guid == original.guid && 
                    Player.inst.Buildings.data[index].GetCell().Position.x == (original.GetCell().Position.x + offset))
                {
                    return Player.inst.Buildings.data[index];
                }
            }
            return null;
        }
        
        public static int Count = 0;
        public static int Count2 = 0;
        public static int Count3 = 0;
        public static int Count4 = 0;
        public static int Count5 = 0;
        public static int Count6 = 0;
        public static int Count7 = 0;
        public static int Count8 = 0;
        public static int Count9 = 0;
        public static int Count10 = 0;
        public static int Count11 = 0;
        public static int Count12 = 0;
        public static int Count13 = 0;
        public static int Count14 = 0;
        public static int Count15 = 0;
        public static int Count16 = 0;
        public static int Count17 = 0;
        public static int Count18 = 0;
        public static int Count19 = 0;
        public static int Count20 = 0;
        public static int Count21 = 0;

        // Data that needs to be listened to
        // public void CompleteBuild() -> this.built = true; // TODO - Not Needed
        // public void OnPlacement(bool byAI) -> this.placed = true; // TODO - Done
        // public bool Open -> this.open = value;
        // public void EnterPausedConstruction() -> this.constructionPaused = true; || public void ExitPausedConstruction() -> this.constructionPaused = false; // TODO - Done
        // private void UpdateConstruction(float dt) -> this.constructionProgress = this.resourceProgress; // TODO - Done
        // private void RepairInternal(float amount) -> this.Life = Mathf.Clamp(this.Life, 0.0f, this.ModifiedMaxLife); // TODO - Done

        [HarmonyPatch(typeof(Building))]
        [HarmonyPatch("OnPlacement")]
        public static class DataPatch2 {
            //A function to run after target function invocation
            private static void Postfix(Building __instance, bool byAI) {
                // Only for SP testing - This is to prevent the building from being placed twice and crashing the game
                if (__instance.UniqueName == "keep") return;
                
                // Prevent infinite loop
                if (Count2 > 0) { Count2--; return; }
                Count2++;
                
                // Send Data to Client
                AddBuilding(__instance);
            }
        }

        public static void AddBuilding(Building host_data)
        {
            // Data used is UniqueName, guid, position, rotation - This is enough to create the building on the client (Useful to reduce data sent to client)
            
            // Create a new building
            Building building = Instantiate(GameState.inst.GetPlaceableByUniqueName(host_data.UniqueName));
            building.Init();
            
            // Force Same Guid - This is needed for the client to know what building to update
            building.guid = host_data.guid;
            
            // Ensure the building is placed in the correct location and rotation - TODO remove offset
            building.transform.position = new Vector3(host_data.GetCell().Position.x + offset, 0.0f, host_data.GetCell().Position.z);
            building.transform.GetChild(0).rotation = host_data.transform.GetChild(0).rotation; // This is the original rotation
            
            // Place the building in the world - 4 Different Types to Call this function
            World.inst.PlaceByAI(building);
            
            // In order to add to list, we need to call this function
            //Player.inst.AddBuilding(building);
        }

        [HarmonyPatch(typeof(Building))]
        [HarmonyPatch("OnDisable")]
        public static class DataPatch3 {
            //A function to run after target function invocation
            private static void Postfix(Building __instance) {
                // Prevent infinite loop
                if (Count3 > 0) { Count3--; return;}
                Count3++;
                
                // Send Data to Client
                Building clone = GetBuildingClone(__instance);
                if (clone != null)
                {
                    World.inst.DemolishBuildingByPlayer(clone, false);
                } else
                {
                    Count3--;
                }
            }
        }

        [HarmonyPatch(typeof(Building))]
        [HarmonyPatch("EnterPausedConstruction")]
        public static class DataPatch6 {
            //A function to run after target function invocation
            private static void Postfix(Building __instance) {
                
                // Prevent infinite loop
                if (Count6 > 0) { Count6--; return; }
                Count6++;
                
                // Send Data to Client
                Building clone = GetBuildingClone(__instance);
                if (clone != null)
                {
                    clone.EnterPausedConstruction();
                } else
                {
                    Count6--;
                }
            }
        }
        
        [HarmonyPatch(typeof(Building))]
        [HarmonyPatch("ExitPausedConstruction")]
        public static class DataPatch5 {
            //A function to run after target function invocation
            private static void Postfix(Building __instance) {
                
                // Prevent infinite loop
                if (Count5 > 0) { Count5--; return; }
                Count5++;
                
                // Send Data to Client
                Building clone = GetBuildingClone(__instance);
                if (clone != null)
                {
                    clone.ExitPausedConstruction();
                } else
                {
                    Count5--;
                }
            }
        }
        
        [HarmonyPatch(typeof(Building))]
        [HarmonyPatch("AddBuildResource")]
        public static class DataPatch4 {
            //A function to run after target function invocation
            private static void Postfix(Building __instance, ResourceAmount amount) {
                // Prevent infinite loop
                if (Count4 > 0) { Count4--; return; }
                Count4++;
                
                // Send Data to Client
                Building clone = GetBuildingClone(__instance);
                if (clone != null)
                {
                    clone.AddBuildResource(amount);
                } else
                {
                    Count4--;
                }
            }
        }
        
        // Called every frame
        [HarmonyPatch(typeof(Building))]
        [HarmonyPatch("UpdateConstruction")]
        public static class DataPatch7 {
            //A function to run after target function invocation
            private static void Postfix(Building __instance, float dt) {
                if (__instance.UniqueName == "keep") return;
                
                if (Count7 > 0) { Count7--; return; }
                Count7++;
                
                // Send Data to Client
                Building clone = GetBuildingClone(__instance);
                if (clone != null)
                {
                    clone.constructionProgress = __instance.constructionProgress;
                } else
                {
                    Count7--;
                }
            }
        }
        
        [HarmonyPatch(typeof(Building))]
        [HarmonyPatch("Repair")]
        public static class DataPatch9 {
            //A function to run after target function invocation
            private static void Postfix(Building __instance, float amount) {
                // Prevent infinite loop
                if (Count9 > 0) { Count9--; return; }
                Count9++;
                
                // Send Data to Client
                Building clone = GetBuildingClone(__instance);
                if (clone != null)
                {
                    clone.Repair(amount);
                } else
                {
                    Count9--;
                }
            }
        }
        
        //TakeDamage(float dmg, Vector3 pos, string attackerName)
        [HarmonyPatch(typeof(Building))]
        [HarmonyPatch("TakeDamage")]
        public static class DataPatch10 {
            //A function to run after target function invocation
            private static void Postfix(Building __instance, float dmg, Vector3 pos, string attackerName) {
                // Prevent infinite loop
                if (Count10 > 0) { Count10--; return; }
                Count10++;
                
                // Send Data to Client
                Building clone = GetBuildingClone(__instance);
                if (clone != null)
                {
                    clone.TakeDamage(dmg, pos, attackerName);
                } else
                {
                    Count10--;
                }
            }
        }
        
        // [HarmonyPatch(typeof(Building), nameof(Building.Open))]
        // [HarmonyPostfix]
        // public static void Open(Building __instance, bool value) {
        //     // Prevent infinite loop
        //     if (Count11 > 0) { Count11--; return; }
        //     Count11++;
        //     
        //     // Send Data to Client
        //     Building clone = GetBuildingClone(__instance);
        //     if (clone != null)
        //     {
        //         clone.Open = __instance.IsOpen();
        //     } else
        //     {
        //         Count11--;
        //     }
        // }
        
        // [HarmonyPatch(typeof(WorkerUI), "Init")]
        // public static class DataPatch12 {
        //     //A function to run after target function invocation
        //     private static void Postfix(WorkerUI __instance) {
        //         helper.Log("WorkerUI Init");
        //         
        //         // Prevent infinite loop
        //         if (Count12 > 0) { Count12--; return; }
        //         Count12++;
        //         
        //         helper.Log("Pass Counter");
        //
        //         // Send Data to Client
        //         Building building = Traverse.Create(__instance).Field("building").GetValue() as Building;
        //
        //         helper.Log("Building: " + building);
        //         helper.Log("Title Text: " + __instance.titleText.text);
        //         helper.Log("Building Name: " + building.customName);
        //         
        //         
        //         Building clone = GetBuildingClone(building);
        //         if (clone != null)
        //         {
        //             helper.Log("Title Text: " + __instance.titleText.text);
        //             clone.customName = __instance.titleText.text;
        //         } else
        //         {
        //             helper.Log("Fail Building");
        //             Count12--;
        //         }
        //     }
        // }
        
        // TODO - Add more data to listen to
        //     public ResourceAmount CollectForBuild; // Resources for the building, potentially out of scope // TODO - Done
        //     public string customName; // Custom name set by player, Done in WorkerUI - OnNameChanged???
        //     public List<object> components = new List<object>(); // Seems to be related to saving data???
        //     public List<object> jobs = new List<object>(); // A lot of Modified Calls
        
        // private void AddJob(Building b, Job j)
        // JobSystem Class
        // Resoruce Class
        // Villiger Class
    }
}
