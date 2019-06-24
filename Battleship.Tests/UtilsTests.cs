using NUnit.Framework;
using BattleShip.UI;
using System.Collections.Generic;
using BattleShip.BLL.GameLogic;
using BattleShip.BLL.Requests;
using System.Collections;
using System;

namespace Battleship.Tests
{
    [TestFixture]
    public class UtilsTests
    {
        public static IEnumerable StrToCoordsTestCases
        {
            get
            {
                yield return new TestCaseData("A1", new Coordinate(1, 1));
                yield return new TestCaseData("J10", new Coordinate(10, 10));
                yield return new TestCaseData("E5", new Coordinate(5, 5));
                yield return new TestCaseData("J1", new Coordinate(1, 10));
                yield return new TestCaseData("K1", null);
                yield return new TestCaseData("A11", null);
                yield return new TestCaseData("AA1", null);
                yield return new TestCaseData("A-1", null);
                yield return new TestCaseData("Z-5", null);
                yield return new TestCaseData("Pancakes", null);
                yield return new TestCaseData("", null);
                yield return new TestCaseData(null, null);
            }
        }

        [TestCaseSource("StrToCoordsTestCases")]
        public void StrToCoordsTest(string coordsToConvert, Coordinate expected)
        {
            Coordinate result = Utils.StrToCoordinate(coordsToConvert);
            Assert.AreEqual(expected, result);
        }


        public static IEnumerable CoordsToStrTestCases
        {
            get
            {
                yield return new TestCaseData(new Coordinate(1, 1), "A1");
                yield return new TestCaseData(new Coordinate(10, 10), "J10");
                yield return new TestCaseData(new Coordinate(5, 5), "E5");
                yield return new TestCaseData(new Coordinate(1, 10), "J1");
            }
        }

        [TestCaseSource("CoordsToStrTestCases")]
        public void CoordsToStrTest(Coordinate coordsToConvert, string expected)
        {
            string result = Utils.CoordinateToStr(coordsToConvert);
            Assert.AreEqual(expected, result);
        }

        public static IEnumerable IsValidCoordinatesTestCases
        {
            get
            {
                yield return new TestCaseData(new Coordinate(0, 1), false);
                yield return new TestCaseData(new Coordinate(1, 1), true);
                yield return new TestCaseData(new Coordinate(0, 11), false);
                yield return new TestCaseData(new Coordinate(10, 10), true);
                yield return new TestCaseData(new Coordinate(5, 8), true);
                yield return new TestCaseData(new Coordinate(1, 10), true);
                yield return new TestCaseData(new Coordinate(-1, 8), false);
                yield return new TestCaseData(new Coordinate(1, 14), false);
            }
        }

        [TestCaseSource("IsValidCoordinatesTestCases")]
        public void IsValidCoordinatesTest(Coordinate coords, bool expected)
        {
            bool result = Utils.IsValidCoordinate(coords);
            Assert.AreEqual(expected, result);
        }
    }
}
