// using NUnit.Framework;
// using B3.PlayerBuffSystem;
// using B3.ResourcesSystem;
// using UnityEngine;
// using System.Collections.Generic;
//
// namespace B3.Tests
// {
//     public class PlayerBuffsTests
//     {
//         private PlayerBuffs playerBuffs;
//
//         [SetUp]
//         public void SetUp()
//         {
//             // Inițializăm PlayerBuffs pentru fiecare test
//             playerBuffs = new GameObject().AddComponent<PlayerBuffs>();
//         }
//
//         [Test]
//         public void AddBuff_ShouldAddTrade4_1Buff_WhenNoBuffExists()
//         {
//             playerBuffs.AddBuff(ResourceType.Ore, PlayerBuff.Trade4_1);
//
//             // Verificăm dacă Trade4_1 a fost adăugat pentru Ore
//             Assert.AreEqual(PlayerBuff.Trade4_1, playerBuffs.GetBuffForResource(ResourceType.Ore));
//         }
//
//         [Test]
//         public void AddBuff_ShouldNotOverwriteWithWeakerBuff()
//         {
//             playerBuffs.AddBuff(ResourceType.Ore, PlayerBuff.Trade3_1);
//             playerBuffs.AddBuff(ResourceType.Ore, PlayerBuff.Trade4_1); // Trade3_1 este mai bun decât Trade4_1
//
//             // Verificăm că buff-ul Trade3_1 nu a fost înlocuit
//             Assert.AreEqual(PlayerBuff.Trade3_1, playerBuffs.GetBuffForResource(ResourceType.Ore));
//         }
//
//         [Test]
//         public void AddBuff_ShouldReplaceWithStrongerBuff()
//         {
//             playerBuffs.AddBuff(ResourceType.Ore, PlayerBuff.Trade4_1);
//             playerBuffs.AddBuff(ResourceType.Ore, PlayerBuff.Trade2_1); // Trade2_1 este mai puternic decât Trade4_1
//
//             // Verificăm că Trade4_1 a înlocuit Trade3_1
//             Assert.AreEqual(PlayerBuff.Trade2_1, playerBuffs.GetBuffForResource(ResourceType.Ore));
//         }
//
//         [Test]
//         public void AddBuff_ShouldAddBuffForNewResource()
//         {
//             playerBuffs.AddBuff(ResourceType.Ore, PlayerBuff.Trade4_1);
//             playerBuffs.AddBuff(ResourceType.Wheat, PlayerBuff.Trade3_1); // Adăugăm un buff pentru o resursă nouă
//
//             // Verificăm că Trade3_1 a fost adăugat pentru Wheat
//             Assert.AreEqual(PlayerBuff.Trade3_1, playerBuffs.GetBuffForResource(ResourceType.Wheat));
//         }
//
//         [Test]
//         public void AddBuff_ShouldHandleMultipleBuffs()
//         {
//             // Adăugăm un buff pentru fiecare resursă
//             playerBuffs.AddBuff(ResourceType.Ore, PlayerBuff.Trade4_1);
//             playerBuffs.AddBuff(ResourceType.Wheat, PlayerBuff.Trade3_1);
//             playerBuffs.AddBuff(ResourceType.Wood, PlayerBuff.Trade2_1);
//
//             // Verificăm dacă buff-urile sunt corecte
//             Assert.AreEqual(PlayerBuff.Trade4_1, playerBuffs.GetBuffForResource(ResourceType.Ore));
//             Assert.AreEqual(PlayerBuff.Trade3_1, playerBuffs.GetBuffForResource(ResourceType.Wheat));
//             Assert.AreEqual(PlayerBuff.Trade2_1, playerBuffs.GetBuffForResource(ResourceType.Wood));
//         }
//     }
// }
