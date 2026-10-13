using System.IO;
using System.Text.RegularExpressions;
using Game.Core.Utilities;
using Game.PlayerSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Tests
{
    public class PlayerDataTests
    {
        private const string FileName = "player_data_tests.json";

        private static string PathOf => Path.Combine(Application.persistentDataPath, FileName);

        [TearDown]
        public void TearDown()
        {
            JsonFileUtility.Delete(FileName);
            File.Delete(PathOf + ".tmp");
        }

        [Test]
        public void SaveAndLoad_KeepsScoreAndBoard()
        {
            var saved = new PlayerData { Score = 45 };
            saved.Board.Add(new BoardItemData("coffee", 3, new Vector2Int(2, 4)));

            JsonFileUtility.Save(FileName, saved);
            JsonFileUtility.Save(FileName, saved);
            var loaded = new PlayerData();

            Assert.IsTrue(JsonFileUtility.Load(FileName, loaded));
            Assert.AreEqual(45, loaded.Score);
            Assert.AreEqual(1, loaded.Board.Count);
            Assert.AreEqual("coffee", loaded.Board[0].ChainId);
            Assert.AreEqual(3, loaded.Board[0].Level);
            Assert.AreEqual(new Vector2Int(2, 4), loaded.Board[0].Cell);
            Assert.IsFalse(File.Exists(PathOf + ".tmp"));
        }

        [Test]
        public void Load_MissingFile_ReturnsFalse()
        {
            Assert.IsFalse(JsonFileUtility.Load(FileName, new PlayerData()));
        }

        [Test]
        public void Load_CorruptFile_ReturnsFalse()
        {
            File.WriteAllText(PathOf, "{ not json");
            LogAssert.Expect(LogType.Error, new Regex("could not be read"));

            Assert.IsFalse(JsonFileUtility.Load(FileName, new PlayerData()));
        }
    }
}
